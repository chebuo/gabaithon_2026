using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(SlotReel))]
public class ReelSetup : MonoBehaviour
{
    public RectTransform reelContainer;
    public List<Sprite> symbolSprites; // 例: チェリー、ベル、7など6種類
    public int visibleCount = 3;       // 窓に見せるコマ数
    public float symbolHeight = 150f;

    [HideInInspector] public List<int> symbolOrder = new List<int>(); // 実際の並び順(絵柄インデックス)

    void Awake()
    {
        GenerateReel();
    }

    public void GenerateReel(int loopMultiplier = 4)
    {
        // symbolSpritesをシャッフルまたは固定順でloopMultiplier回繰り返して並べる
        symbolOrder.Clear();
        for (int loop = 0; loop < loopMultiplier; loop++)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < symbolSprites.Count; i++) indices.Add(i);
            Shuffle(indices); // 好みで固定配列でもOK

            foreach (var idx in indices)
            {
                symbolOrder.Add(idx);
                GameObject go = new GameObject("Symbol", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(reelContainer, false);
                var img = go.GetComponent<Image>();
                img.sprite = symbolSprites[idx];
                var rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(symbolHeight, symbolHeight);
            }
        }
        ArrangeVertically();

        var slotReel = GetComponent<SlotReel>();
        slotReel.symbolOrder = symbolOrder;
        slotReel.symbolSprites = symbolSprites;
    }

    void ArrangeVertically()
    {
        for (int i = 0; i < reelContainer.childCount; i++)
        {
            var rt = reelContainer.GetChild(i) as RectTransform;
            rt.anchoredPosition = new Vector2(0, -i * symbolHeight);
        }
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}
