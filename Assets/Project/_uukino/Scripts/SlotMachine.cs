using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class SlotMachine : MonoBehaviour
{
    // Key.Digit0はKey.Digit9の直後にあるため、Digit0+iのような連番計算はできない。配列で明示的に対応させる
    private static readonly Key[] digitKeys =
    {
        Key.Digit0, Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4,
        Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9
    };

    public SlotReel[] reels;
    public float delayBetweenStops = 0.4f; // 左→中→右で止めるタイミングをずらす

    [Header("演出")]
    public GameObject normalSpotlight;   // 通常時のスポットライト
    public GameObject rainbowSpotlight;  // 大当たり時に光る虹色スポットライト（RainbowSpotlightをアタッチしたオブジェクト）
    public int jackpotSymbolIndex = 5;   // この絵柄が全リールで揃ったら大当たり

    [Header("小当たり演出（7柄以外で揃ったとき）")]
    public float smallWinBlinkDuration = 2f;   // 通常スポットライトを点滅させる秒数
    public float smallWinBlinkInterval = 0.15f; // 点滅の間隔

    [Header("設定（確率・コイン・タイムリミット、レベル調整込み）")]
    public SlotData slotData; // symbolCount・確率・1コインのスピン数・赤ランプ発動条件・暗転までの時間を管理するアセット

    [Header("デバッグ")]
    public bool debugNumberKeysEnabled = true; // 数字キーで絵柄を強制的に揃えるデバッグ機能

    [Header("タイムリミット演出")]
    public GameObject redSpotlight;           // 用意されている赤いスポットライト（点滅させる）
    public GameObject transitionButtonObject; // シーン遷移用ボタン（普段は非表示）
    public CanvasGroup fadeCanvasGroup;       // 画面全体を覆う黒いCanvasGroup（暗転用）
    public string caughtSceneName = "caught";
    public string selectSceneName = "SelectScene"; // 遷移ボタンを押したときに戻るシーン
    public float redBlinkInterval = 0.3f;     // 赤いスポットライトの点滅間隔
    public float fadeDuration = 1f;           // 暗転にかける秒数

    private bool isPlaying = false;
    private bool winBoostActive = false; // 7柄が揃った後、揃いやすいボーナス状態かどうか

    private int spinCount = 0;
    private float firstSpinTime = -1f;
    private bool escapeTriggered = false;
    private bool transitionButtonPressed = false;
    private Coroutine redBlinkCoroutine;
    private int spinsRemaining;

    // slotDataが未設定でもエラーにならないようフォールバック値を用意
    int SymbolCount => slotData != null ? slotData.symbolCount : 6;
    float BaseAlignChance => slotData != null ? slotData.FinalBaseAlignChance : 0.05f;
    float BoostedAlignChance => slotData != null ? slotData.FinalBoostedAlignChance : 0.35f;
    int SpinsPerCoin => slotData != null ? slotData.FinalSpinsPerCoin : 15;
    int RedLampSpinCount => slotData != null ? slotData.redLampSpinCount : 15;
    float RedLampTimeSeconds => slotData != null ? slotData.FinalRedLampTimeSeconds : 50f;
    float BlackoutDelaySeconds => slotData != null ? slotData.FinalBlackoutDelaySeconds : 3f;

    void Start()
    {
        ValidateSymbolConsistency();
        spinsRemaining = SpinsPerCoin;
    }

    // 各リールのSymbol Spritesが同じ並び順になっているか確認する（ズレていると同じ番号でも違う絵柄が揃ってしまう）
    void ValidateSymbolConsistency()
    {
        if (reels == null || reels.Length == 0) return;

        var baseSetup = reels[0] != null ? reels[0].GetComponent<ReelSetup>() : null;
        if (baseSetup == null) return;

        for (int i = 1; i < reels.Length; i++)
        {
            if (reels[i] == null) continue;
            var setup = reels[i].GetComponent<ReelSetup>();
            if (setup == null) continue;

            if (setup.symbolSprites.Count != baseSetup.symbolSprites.Count)
            {
                Debug.LogWarning($"[SlotMachine] {reels[i].name} の Symbol Sprites の数({setup.symbolSprites.Count})が {reels[0].name}({baseSetup.symbolSprites.Count}) と違います。全リールで同じ絵柄リストを同じ順番で設定してください。", reels[i]);
                continue;
            }

            for (int s = 0; s < baseSetup.symbolSprites.Count; s++)
            {
                if (setup.symbolSprites[s] != baseSetup.symbolSprites[s])
                {
                    Debug.LogWarning($"[SlotMachine] {reels[i].name} のインデックス{s}番の絵柄({setup.symbolSprites[s]})が {reels[0].name}({baseSetup.symbolSprites[s]}) と違います。数字キーで揃えても絵柄が一致しません。全リールのSymbol Spritesの並び順を揃えてください。", reels[i]);
                }
            }
        }
    }

    void Update()
    {
        if (!debugNumberKeysEnabled || isPlaying) return;
        if (Keyboard.current == null) return;

        for (int i = 0; i < digitKeys.Length; i++)
        {
            if (Keyboard.current[digitKeys[i]].wasPressedThisFrame)
            {
                StartCoroutine(PlaySequence(i));
                break;
            }
        }
    }

    void LateUpdate()
    {
        // スピン回数超過、または1回目のスピンから一定時間の経過を継続的にチェックする
        if (escapeTriggered || firstSpinTime < 0f) return;

        bool overSpinLimit = spinCount > RedLampSpinCount;
        bool overTimeLimit = Time.time - firstSpinTime >= RedLampTimeSeconds;

        if (overSpinLimit || overTimeLimit)
        {
            escapeTriggered = true;
            StartCoroutine(EscapeSequence());
        }
    }

    public void OnSpinButtonPressed()
    {
        if (isPlaying || spinsRemaining <= 0) return;
        spinsRemaining--;
        StartCoroutine(PlaySequence());
    }

    // forcedSymbol を指定すると、抽選を無視して全リールをその絵柄で揃える（デバッグ用）
    IEnumerator PlaySequence(int? forcedSymbol = null)
    {
        isPlaying = true;

        if (firstSpinTime < 0f) firstSpinTime = Time.time;
        spinCount++;

        foreach (var reel in reels) reel.StartSpin();

        yield return new WaitForSeconds(1.0f); // 最低回転時間を確保

        // 結果を先に決める（抽選ロジックは別途）
        int[] results = forcedSymbol.HasValue ? ForceResults(forcedSymbol.Value) : DecideResults();

        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].RequestStop(results[i]);
            yield return new WaitForSeconds(delayBetweenStops);
        }

        // 全リールが止まるまで待機
        yield return new WaitUntil(() => AllReelsStopped());

        isPlaying = false;
        CheckWinCondition(results);
    }

    int[] ForceResults(int symbolIndex)
    {
        int[] results = new int[reels.Length];
        for (int i = 0; i < results.Length; i++)
            results[i] = symbolIndex;
        return results;
    }

    bool AllReelsStopped()
    {
        foreach (var reel in reels)
            if (reel.IsSpinning) return false;
        return true;
    }

    int[] DecideResults()
    {
        // ボーナス中(winBoostActive)は揃う確率を上げる
        float alignChance = winBoostActive ? BoostedAlignChance : BaseAlignChance;

        int[] results = new int[reels.Length];
        if (Random.value < alignChance)
        {
            int symbol = Random.Range(0, SymbolCount);
            for (int i = 0; i < results.Length; i++)
                results[i] = symbol;
        }
        else
        {
            for (int i = 0; i < results.Length; i++)
                results[i] = Random.Range(0, SymbolCount);
        }
        return results;
    }

    void CheckWinCondition(int[] results)
    {
        // 揃ったかどうかの判定はここで
        Debug.Log($"結果: {string.Join(",", results)}");

        bool isAligned = true;
        for (int i = 1; i < results.Length; i++)
        {
            if (results[i] != results[0])
            {
                isAligned = false;
                break;
            }
        }
        bool isJackpot = isAligned && results[0] == jackpotSymbolIndex;

        if (isJackpot)
        {
            // 7柄が揃った→虹色スポットライトへ切り替え、以降揃いやすくする
            winBoostActive = true;
            SetRainbowActive(true);
        }
        else if (isAligned)
        {
            // 7柄以外で揃った→虹色演出中でなければ通常スポットライトを点滅させる
            if (rainbowSpotlight == null || !rainbowSpotlight.activeSelf)
                StartCoroutine(BlinkNormalSpotlight());
        }
        else
        {
            // 揃わなかった→ボーナス状態を終了し、虹色から通常スポットライトへ戻す
            if (winBoostActive)
            {
                winBoostActive = false;
                SetRainbowActive(false);
            }
        }
    }

    void SetRainbowActive(bool active)
    {
        if (normalSpotlight != null) normalSpotlight.SetActive(!active);
        if (rainbowSpotlight != null) rainbowSpotlight.SetActive(active);
    }

    IEnumerator BlinkNormalSpotlight()
    {
        if (normalSpotlight == null) yield break;

        float elapsed = 0f;
        while (elapsed < smallWinBlinkDuration)
        {
            normalSpotlight.SetActive(!normalSpotlight.activeSelf);
            yield return new WaitForSeconds(smallWinBlinkInterval);
            elapsed += smallWinBlinkInterval;
        }
        normalSpotlight.SetActive(true);
    }

    // シーン遷移用ボタンのOnClickにこのメソッドを登録しておくこと
    public void OnTransitionButtonPressed()
    {
        transitionButtonPressed = true;

        if (redBlinkCoroutine != null)
        {
            StopCoroutine(redBlinkCoroutine);
            redBlinkCoroutine = null;
        }
        if (redSpotlight != null) redSpotlight.SetActive(false);
        if (transitionButtonObject != null) transitionButtonObject.SetActive(false);

        SceneManager.LoadScene(selectSceneName);
    }

    IEnumerator EscapeSequence()
    {
        if (transitionButtonObject != null) transitionButtonObject.SetActive(true);

        if (redSpotlight != null)
        {
            redSpotlight.SetActive(true);
            redBlinkCoroutine = StartCoroutine(BlinkRedSpotlight());
        }

        float elapsed = 0f;
        while (elapsed < BlackoutDelaySeconds)
        {
            if (transitionButtonPressed) yield break; // ボタンが押されたので暗転・シーン遷移をキャンセル
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (transitionButtonPressed) yield break;

        yield return StartCoroutine(FadeToBlackAndLoadScene(caughtSceneName));
    }

    IEnumerator BlinkRedSpotlight()
    {
        while (true)
        {
            redSpotlight.SetActive(!redSpotlight.activeSelf);
            yield return new WaitForSeconds(redBlinkInterval);
        }
    }

    IEnumerator FadeToBlackAndLoadScene(string sceneName)
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        SceneManager.LoadScene(sceneName);
    }
}
