using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class PlayerBagManagerBob : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [Header("スコア・バッグ設定")]
    public int value = 0;
    public int bagCapacity = 5;
    public int goalScore = 100;
    private int score = 0;
    private float delTimer = 0f;
    private bool isCleared = false;
    private bool isInvincible = false;
    [SerializeField, Range(0f, 1f)] private float gameOverMoneyLossRate = 0.5f;
    [SerializeField] private float hitKnockbackForce = 3f;
    [SerializeField] private float deathKnockbackForce = 12f;
    [SerializeField] private GoutouData goutouData;

    private PlayerMovementBob playerMovement;
    [SerializeField] private GameObject goldBar;
    [SerializeField] private Vector3 throwpower;

    SceneChanger sceneChanger = new SceneChanger();

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovementBob>();
    }
    private void Start()
    {
        playerData.isGameOver = false;
        UIManagerBob.Instance.SetGoalScore(goalScore);
        bagCapacity = 8 + goutouData.maxItemLevel;
    }
    private void Update()
    {
        if (playerData.isGameOver)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                Revive();
            }
            else if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
            {
                ReturnToMap();
            }

            return;
        }

        if (isCleared && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GameClear();
        }
    }

    // 貴重品取得確認（衝突した時）
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "valuables")
        {
            if (value >= bagCapacity)
            {
                UIManagerBob.Instance.SetBag(value, Color.red);
                return;
            }
            value += 1;
            UIManagerBob.Instance.SetBag(value, Color.white);
            playerMovement.moveSpeed -= 0.2f;
            Destroy(collision.gameObject);
        }
    }

    // 納品処理（トリガー内に留まっている時）
    private void OnTriggerStay(Collider other)
    {
        if (playerData.isGameOver)
        {
            return;
        }

        if (other.gameObject.tag == "valuables")
        {
            if (value >= bagCapacity)
            {
                UIManagerBob.Instance.SetBag(value, Color.red);
                return;
            }
            value += 1;
            UIManagerBob.Instance.SetBag(value, Color.white);
            playerMovement.moveSpeed -= 0.2f;
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "track")
        {
            delTimer += Time.deltaTime;
            if (delTimer >= 0.2f && value > 0)
            {
                GameObject gold;
                gold = Instantiate(goldBar);
                gold.transform.localScale = gold.transform.localScale * 0.7f;
                gold.transform.position = transform.position + transform.up * 1.5f;
                Rigidbody goldRb = gold.GetComponent<Rigidbody>();
                goldRb.AddForce(throwpower + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0f, 0f), ForceMode.Impulse);
                value -= 1;
                UIManagerBob.Instance.SetBag(value, Color.white);
                GameManagerBob.instance.money += 5;
                score += 5;
                if (score >= goalScore)
                {
                    UIManagerBob.Instance.SetScore(score, Color.green);
                }
                else
                {
                    UIManagerBob.Instance.SetScore(score, Color.white);
                }
                playerMovement.moveSpeed += 0.2f;
                delTimer = 0f;
            }
        }
        TryGetDamage(other);
    }
    private void OnTriggerEnter(Collider other)
    {
        TryGetDamage(other);

        if (other.gameObject.tag == "gopoint")
        {
            if (score >= goalScore)
            {
                UIManagerBob.Instance.ShowDialog("スペースキーを押して逃げる", Color.green);
                isCleared = true;
            } else
            {
                UIManagerBob.Instance.ShowDialog("もっと盗んでください", Color.red);
            }
        }
        if (other.gameObject.tag == "Bank")
        {
            GameManagerBob.instance.isPlayerInBank = true;
        }
    }

    private void TryGetDamage(Collider other)
    {
        if (!playerData.isGameOver && !isInvincible && other.CompareTag("damageArea"))
        {
            GetDamage(other);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "gopoint")
        {
            UIManagerBob.Instance.ShowDialog("", Color.white);
            isCleared = false;
        }
        if (other.gameObject.tag == "Bank")
        {
            GameManagerBob.instance.isPlayerInBank = false;
        }
    }
    public void GameClear()
    {
        // ゲームクリア処理を実装する
        Debug.Log("Game Clear!");
        playerData.coin = GameManagerBob.instance.money;
        playerData.isClearBank = true;
        sceneChanger.ChangeScene("SelectScene", 0);
    }

    private void Revive()
    {
        if (playerData.gem < playerData.ReviveCost)
        {
            UIManagerBob.Instance.ShowDialog("ジェムが足りません", Color.red);
            return;
        }

        playerData.gem -= playerData.ReviveCost;
        playerData.isGameOver = false;
        GameManagerBob.instance.RestorePlayerHealth();
        playerMovement.Revive();

        EnemyManagerBob enemyManager = FindFirstObjectByType<EnemyManagerBob>();
        if (enemyManager != null)
        {
            enemyManager.ResetEnemies();
        }

        UIManagerBob.Instance.SetHealth(GameManagerBob.instance.playerHealth);
        UIManagerBob.Instance.ShowDialog("", Color.white);
    }

    private void ReturnToMap()
    {
        playerData.coin = Mathf.FloorToInt(playerData.coin * (1f - gameOverMoneyLossRate));
        playerData.isGameOver = false;
        sceneChanger.ChangeScene("SelectScene", 0);
    }

    async void GetDamage(Collider damageCollider)
    {
        GameManagerBob.instance.playerHealth -= 10;
        Vector3 knockbackDirection = damageCollider.transform.forward;
        if (GameManagerBob.instance.playerHealth <= 0)
        {
            playerData.isGameOver = true;
            playerMovement.ApplyDeathKnockback(knockbackDirection, deathKnockbackForce);
            UIManagerBob.Instance.ShowDialog("ゲームオーバー\nRキーを押して復活(" + playerData.ReviveCost + "ジェム使用)\nQキーを押してマップに戻る", Color.red);
        }
        else
        {
            playerMovement.ApplyKnockback(knockbackDirection, hitKnockbackForce);
        }
        UIManagerBob.Instance.SetHealth(GameManagerBob.instance.playerHealth);
        isInvincible = true;
        UIManagerBob.Instance.SetHealth(GameManagerBob.instance.playerHealth);
        await Task.Delay(1000);
        isInvincible = false;
    }
}
