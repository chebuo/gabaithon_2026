using UnityEngine;
using System.Collections;

public class SlotMachine : MonoBehaviour
{
    public SlotReel[] reels;
    public float delayBetweenStops = 0.4f; // 左→中→右で止めるタイミングをずらす

    private bool isPlaying = false;

    public void OnSpinButtonPressed()
    {
        if (isPlaying) return;
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        isPlaying = true;

        foreach (var reel in reels) reel.StartSpin();

        yield return new WaitForSeconds(1.0f); // 最低回転時間を確保

        // 結果を先に決める（抽選ロジックは別途）
        int[] results = DecideResults();

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

    bool AllReelsStopped()
    {
        foreach (var reel in reels)
            if (reel.IsSpinning) return false;
        return true;
    }

    int[] DecideResults()
    {
        // 仮のランダム抽選（実際は確率テーブルなどで管理）
        int[] results = new int[reels.Length];
        for (int i = 0; i < results.Length; i++)
            results[i] = Random.Range(0, 6); // 絵柄種類数に合わせる
        return results;
    }

    void CheckWinCondition(int[] results)
    {
        // 揃ったかどうかの判定はここで
        Debug.Log($"結果: {string.Join(",", results)}");
    }
}
