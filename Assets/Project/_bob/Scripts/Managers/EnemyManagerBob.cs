using UnityEngine;
using System.Collections;

public class EnemyManagerBob : MonoBehaviour
{
    [SerializeField] private Transform enemyContainer;
    [SerializeField] private GameObject policePrefab;
    public int maxPoliceCount = 5;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnPolice());
    }

    // Update is called once per frame
    async void Update()
    {
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
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];
            GameObject police = Instantiate(policePrefab, spawnPoint.position, spawnPoint.rotation);
            police.transform.SetParent(enemyContainer);
            PoliceMovementBob policeMovement = police.GetComponent<PoliceMovementBob>();
            if (policeMovement != null)
            {
                policeMovement.target = playerTransform;
            }
            yield return new WaitForSeconds(3f); // 3秒待機
        }
    }
}
