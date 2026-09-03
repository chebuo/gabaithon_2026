using System.Collections.Generic;
using UnityEngine;

public class SlotReel : MonoBehaviour
{
    [Header("参照")]
    public RectTransform reelContainer;
    public List<int> symbolOrder;      // ReelSetupで生成した並び順
    public List<Sprite> symbolSprites; // インデックス→スプライト対応

    [Header("設定")]
    public float symbolHeight = 150f;
    public float spinSpeed = 1500f;
    public float snapDuration = 0.25f;

    private bool isSpinning = false;
    private float currentOffset = 0f;
    private int targetSymbolIndex = -1; // 止めたい絵柄の「symbolOrder内インデックス」

    public bool IsSpinning => isSpinning;

    public void StartSpin()
    {
        isSpinning = true;
        targetSymbolIndex = -1;
    }

    // resultSymbol: 止めたい絵柄の種類（例：0=チェリー, 1=ベル...）
    public void RequestStop(int resultSymbol)
    {
        // symbolOrderの中から、今の位置より先にある該当絵柄の位置を探す
        targetSymbolIndex = FindNextIndexOf(resultSymbol);
    }

    int FindNextIndexOf(int resultSymbol)
    {
        int currentIndex = Mathf.FloorToInt(currentOffset / symbolHeight) % symbolOrder.Count;
        // 現在位置より後ろ（次に窓の正面に来る側）から探す。最低でも数コマは回してから止まるように余裕を持たせる
        int searchStart = currentIndex + 3; // 3コマ分は必ず回す
        for (int i = searchStart; i < searchStart + symbolOrder.Count; i++)
        {
            int idx = i % symbolOrder.Count;
            if (symbolOrder[idx] == resultSymbol)
                return i; // ループ数を含めた絶対インデックスで返す
        }
        return searchStart; // 念のため
    }

    void Update()
    {
        if (!isSpinning) return;

        if (targetSymbolIndex < 0)
        {
            // 通常回転中
            currentOffset += spinSpeed * Time.deltaTime;
            WrapOffset();
            reelContainer.anchoredPosition = new Vector2(0, currentOffset);
        }
        else
        {
            // 目標が決まったのでスナップ処理へ
            StartCoroutine(SnapToTarget());
            isSpinning = false; // Update内の通常回転は停止（コルーチンに移譲）
        }
    }

    void WrapOffset()
    {
        float total = symbolHeight * symbolOrder.Count;
        if (currentOffset >= total) currentOffset -= total;
    }

    System.Collections.IEnumerator SnapToTarget()
    {
        float targetOffset = targetSymbolIndex * symbolHeight;
        float total = symbolHeight * symbolOrder.Count;

        // targetOffsetがcurrentOffsetより手前にならないよう正規化
        while (targetOffset < currentOffset) targetOffset += total;

        float decelStartDistance = symbolHeight * 2f; // 止まる2コマ手前から減速開始

        // 減速しながら目標へ近づける（WrapOffsetはここでは呼ばない。targetOffsetとの差分計算が崩れるため）
        while (currentOffset < targetOffset - 0.5f)
        {
            float remaining = targetOffset - currentOffset;
            float t = Mathf.Clamp01(1f - remaining / decelStartDistance);
            float easedSpeed = Mathf.Lerp(spinSpeed, spinSpeed * 0.05f, t);
            currentOffset += easedSpeed * Time.deltaTime;
            if (currentOffset > targetOffset) currentOffset = targetOffset;

            reelContainer.anchoredPosition = new Vector2(0, currentOffset % total);
            yield return null;
        }

        currentOffset = targetOffset % total;
        reelContainer.anchoredPosition = new Vector2(0, currentOffset);
        targetSymbolIndex = -1;
    }
}