using UnityEngine;

public class BulletMovementBob : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("DestroyBullet", 5f); // 5秒後に弾を破壊
        if (gameObject.tag == "playerattack")
        {
            moveSpeed = 20f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
