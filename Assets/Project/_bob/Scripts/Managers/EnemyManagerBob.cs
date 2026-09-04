using UnityEngine;
using System.Collections;

public class EnemyManagerBob : MonoBehaviour
{
    [SerializeField] private Transform enemyContainer;
    [SerializeField] private GameObject policePrefab;
    public int maxPoliceCount = 5;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnPolice());
        time = 0f;
    }

    // Update is called once per frame
    async void Update()
    {
        time += Time.deltaTime;
    }

    private IEnumerator SpawnPolice()
    {
        while (true)
        {
            if (enemyContainer.childCount >= maxPoliceCount)
            {
                yield return null; // 最大数に達している場合は待機
                continue;
            }
            if (time < 15)  // ゲーム開始後15秒間は敵が湧かない
            {
                yield return null;
                continue;
            }
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];
            GameObject police = Instantiate(policePrefab, spawnPoint.position, spawnPoint.rotation);
            police.transform.SetParent(enemyContainer);
            PoliceMovementBob policeMovement = police.GetComponent<PoliceMovementBob>();
            if (time > 30)
            {
                if (UnityEngine.Random.value < 0.3f) // 30%の確率で敵の武器が銃になる
                {
                    policeMovement.attackType = AttackType.SKS;
                }
            }
            if (policeMovement != null)
            {
                policeMovement.target = playerTransform;
            }
            if (time < 30)  // ゲーム開始後30秒間は敵が湧かない
            {
                yield return new WaitForSeconds(10f); // 5秒待機
            }
            else if (time < 60)
            {
                yield return new WaitForSeconds(8f); // 3秒待機
            }
            else if (time < 90)
            {
                yield return new WaitForSeconds(6f); // 2秒待機
            }
            else
            {
                yield return new WaitForSeconds(2f); // 1秒待機
            }
        }
    }

    public void ResetEnemies()
    {
        time = 0f;

        for (int i = enemyContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(enemyContainer.GetChild(i).gameObject);
        }
    }
}
