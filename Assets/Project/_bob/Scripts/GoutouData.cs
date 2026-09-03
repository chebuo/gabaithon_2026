using UnityEngine;

[CreateAssetMenu(fileName = "GoutouData",menuName = "ScriptableObject/GoutouData")]
public class GoutouData : ScriptableObject
{
    public int moveSpeedLevel;
    public int maxItemLevel;
    public int maxHealthLevel;
    public int attackCoolDownLevel;
    public int gunLevel;
    public int bulletLeft;
}