using UnityEngine;

public class GameManagerBob : MonoBehaviour
{
    [SerializeField] PlayerData playerData;
    [SerializeField] GoutouData goutouData;
    public static GameManagerBob instance;
    public int money;
    public bool isPlayerInBank = false;
    public int playerHealth = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        money = playerData.coin;
        playerHealth = 80 + goutouData.maxHealthLevel * 20;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
