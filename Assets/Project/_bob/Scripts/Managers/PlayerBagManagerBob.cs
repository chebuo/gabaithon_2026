using UnityEngine;

public class PlayerBagManagerBob : MonoBehaviour
{
    [Header("スコア・バッグ設定")]
    public int value = 0;
    public int bagCapacity = 5;
    public int goalScore = 10;
    private int score = 0;
    private float delTimer = 0f;

    private PlayerMovementBob playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovementBob>();
        UIManagerBob.Instance.SetGoalScore(goalScore);
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
        if (other.gameObject.tag == "track")
        {
            delTimer += Time.deltaTime;
            if (delTimer >= 0.2f && value > 0)
            {
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
}
