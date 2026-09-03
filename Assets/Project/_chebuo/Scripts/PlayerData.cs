using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData",menuName = "ScriptableObject/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int gem;
    public int coin;
    public bool isGameOver=false;
    public bool isRevive=false;
    public int ReviveCost=10;


}