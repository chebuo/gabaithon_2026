using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBagManagerBob : MonoBehaviour
{
    [Header("スコア・バッグ設定")]
    public int value = 0;
    public int bagCapacity = 5;
    public int goalScore = 10;
    private int score = 0;
    private float delTimer = 0f;
    private bool isCleared = false;

    private PlayerMovementBob playerMovement;
    [SerializeField] private GameObject goldBar;
    [SerializeField] private Vector3 throwpower;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovementBob>();
    }
    private void Start()
    {
        UIManagerBob.Instance.SetGoalScore(goalScore);
    }
    private void Update()
    {
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
                score += 1;
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
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "gopoint")
        {
            if (score >= goalScore)
            {
                UIManagerBob.Instance.ShowDialog("press space to go next stage", Color.green);
                isCleared = true;
            } else
            {
                UIManagerBob.Instance.ShowDialog("you need more score", Color.red);
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "gopoint")
        {
            UIManagerBob.Instance.ShowDialog("", Color.white);
            isCleared = false;
        }
    }
    public void GameClear()
    {
        // ゲームクリア処理を実装する
        Debug.Log("Game Clear!");
    }
}
