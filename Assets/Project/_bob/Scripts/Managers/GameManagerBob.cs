using UnityEngine;

public class GameManagerBob : MonoBehaviour
{
    [SerializeField] PlayerData playerData;
    [SerializeField] GoutouData goutouData;
    public static GameManagerBob instance;
    public int money;
    public bool isPlayerInBank = false;
    public int playerHealth = 100;
    public int MaxPlayerHealth => 80 + goutouData.maxHealthLevel * 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        money = playerData.coin;
        UIManagerBob.Instance.SetHealth(MaxPlayerHealth);
        playerHealth = MaxPlayerHealth;
    }

    public void RestorePlayerHealth()
    {
        playerHealth = MaxPlayerHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
