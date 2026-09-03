using UnityEngine;

public class GameManagerBob : MonoBehaviour
{
    [SerializeField] PlayerData playerData;
    public static GameManagerBob instance;
    public int money;
    public bool isPlayerInBank = false;
    public int playerHealth = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        money = playerData.coin;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
