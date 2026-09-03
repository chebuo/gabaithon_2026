using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotData", menuName = "Scriptable Objects/SlotData")]
public class SlotData : ScriptableObject
{
    [Header("柄がそろう確率")]
    public int symbolCount = 6;
    [Range(0f, 1f)] public float baseAlignChance = 0.05f;
    [Range(0f, 1f)] public float boostedAlignChance = 0.35f;
    [Tooltip("レベルが1つ上がるごとに確率へ掛ける倍率。レベル1=そのまま、レベル2=この値そのものを掛けた値（0.2なら0.2倍）、レベル3=この値の2乗…と段階的に変化します")]
    [Range(0.01f, 1f)] public float alignChanceLevelFactor = 0.2f;
    public int alignChanceLevel = 1; // 1がデフォルト

    [Header("1コインのスピン数")]
    public int baseSpinsPerCoin = 15;
    [Tooltip("レベルが1つ上がるごとに加算されるスピン数")]
    public int spinsPerCoinLevelStep = 5;
    public int spinsPerCoinLevel = 1; // 1がデフォルト

    [Header("赤ランプが点滅するまでの回数（レベル調整対象外）")]
    public int redLampSpinCount = 15;

    [Header("赤ランプが点滅するまでの時間")]
    public float baseRedLampTimeSeconds = 50f;
    [Tooltip("レベルが1つ上がるごとに加算される秒数（例：1.5なら レベル3で+3秒）")]
    public float redLampTimeLevelStep = 1.5f;
    public int redLampTimeLevel = 1; // 1がデフォルト

    [Header("赤ランプ点滅から暗転までの時間")]
    public float baseBlackoutDelaySeconds = 3f;
    [Tooltip("レベルが1つ上がるごとに加算される秒数")]
    public float blackoutDelayLevelStep = 1f;
    public int blackoutDelayLevel = 1; // 1がデフォルト

    [Header("柄ごとのコイン増減（そろったとき）")]
    [Tooltip("インデックスが絵柄番号に対応。symbolCountと同じ数だけ要素を用意してください（正の値で増加、負の値で減少）")]
    public List<int> symbolCoinRewards = new List<int>();

    public int GetCoinReward(int symbolIndex)
    {
        if (symbolIndex < 0 || symbolIndex >= symbolCoinRewards.Count) return 0;
        return symbolCoinRewards[symbolIndex];
    }

    public float AlignChanceMultiplier => Mathf.Pow(alignChanceLevelFactor, Mathf.Max(0, alignChanceLevel - 1));
    public float FinalBaseAlignChance => Mathf.Clamp01(baseAlignChance * AlignChanceMultiplier);
    public float FinalBoostedAlignChance => Mathf.Clamp01(boostedAlignChance * AlignChanceMultiplier);

    public int FinalSpinsPerCoin => baseSpinsPerCoin + spinsPerCoinLevelStep * Mathf.Max(0, spinsPerCoinLevel - 1);

    public float FinalRedLampTimeSeconds => baseRedLampTimeSeconds + redLampTimeLevelStep * Mathf.Max(0, redLampTimeLevel - 1);

    public float FinalBlackoutDelaySeconds => baseBlackoutDelaySeconds + blackoutDelayLevelStep * Mathf.Max(0, blackoutDelayLevel - 1);
}
