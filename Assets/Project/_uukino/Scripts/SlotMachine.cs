using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // ←追加：新しい入力システムを使う宣言

public class SlotMachine : MonoBehaviour
{
    [Header("UI設定")]
    public Text reel1Text;
    public Text reel2Text;
    public Text reel3Text;
    public Text coinText;
    public Text messageText;

    [Header("ゲーム設定")]
    public int currentCoins = 10;

    private int currentBet = 0;
    private bool isSpinning1 = false;
    private bool isSpinning2 = false;
    private bool isSpinning3 = false;
    private bool gameActive = false;

    private string[] symbols = { "7", "A", "B", "C", "BAR" };

    void Start()
    {
        UpdateUI();
        messageText.text = "スペースキーでコイン投入！";
    }

    void Update()
    {
        // キーボードが接続されていない場合は何もしない（エラー防止）
        if (Keyboard.current == null) return;

        // 【変更点】Input.GetKeyDown から Keyboard.current.〇〇Key.wasPressedThisFrame に変更
        // スペースキーでコイン投入
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !gameActive && currentCoins > 0)
        {
            InsertCoin();
        }

        // エンターキーで回転開始
        if (Keyboard.current.enterKey.wasPressedThisFrame && currentBet > 0 && !gameActive)
        {
            StartSpin();
        }

        // A, S, Dキーでストップ
        if (gameActive)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame && isSpinning1) StopReel(1);
            if (Keyboard.current.sKey.wasPressedThisFrame && isSpinning2) StopReel(2);
            if (Keyboard.current.dKey.wasPressedThisFrame && isSpinning3) StopReel(3);
        }
    }

    void InsertCoin()
    {
        currentCoins--;
        currentBet++;
        UpdateUI();
        messageText.text = "エンターキーでスピン！";
    }

    void StartSpin()
    {
        gameActive = true;
        messageText.text = "A・S・Dキーでストップ！";

        isSpinning1 = isSpinning2 = isSpinning3 = true;

        StartCoroutine(SpinRoutine(1, reel1Text));
        StartCoroutine(SpinRoutine(2, reel2Text));
        StartCoroutine(SpinRoutine(3, reel3Text));
    }

    IEnumerator SpinRoutine(int reelId, Text reelText)
    {
        while (IsReelSpinning(reelId))
        {
            reelText.text = symbols[Random.Range(0, symbols.Length)];
            yield return new WaitForSeconds(0.05f);
        }
    }

    bool IsReelSpinning(int reelId)
    {
        if (reelId == 1) return isSpinning1;
        if (reelId == 2) return isSpinning2;
        return isSpinning3;
    }

    void StopReel(int reelId)
    {
        if (reelId == 1) isSpinning1 = false;
        if (reelId == 2) isSpinning2 = false;
        if (reelId == 3) isSpinning3 = false;

        if (!isSpinning1 && !isSpinning2 && !isSpinning3)
        {
            CheckWin();
        }
    }

    void CheckWin()
    {
        gameActive = false;

        if (reel1Text.text == reel2Text.text && reel2Text.text == reel3Text.text)
        {
            int payout = currentBet * 2;
            currentCoins += payout;
            messageText.text = $"大当たり！ {payout}コイン獲得！\n(スペースで再挑戦)";
        }
        else
        {
            messageText.text = "ハズレ...\n(スペースで再挑戦)";
        }

        currentBet = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        coinText.text = $"所持コイン: {currentCoins} \n投入中: {currentBet}";
    }
}