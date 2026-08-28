using UnityEngine;

public class BuildGenerator : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject buildingPrefab;

    [Header("First Building")]
    [SerializeField] private Vector3 firstBuildingPosition = Vector3.zero;
    [SerializeField] private Vector3 firstBuildingScale = new Vector3(5f, 5f, 5f);

    [Header("Building")]
    [SerializeField] private float minBuildingWidth = 3f;
    [SerializeField] private float maxBuildingWidth = 8f;
    [SerializeField] private float minHeight = -3f;
    [SerializeField] private float maxHeight = 5f;

    [Header("Gap")]
    [SerializeField] private float minGap = 1f;
    [SerializeField] private float maxGap = 3f;

    [Header("Noise")]
    [SerializeField] private float noiseScale = 0.2f;
    [SerializeField] private float maxHeightDifference=2;


    [Header("Generate")]
    [SerializeField] private float generateDistance = 30f;

    private float nextBuildX;
    private float noiseOffset;
    private float currentTopY;

    private void Start()
    {
        // 最初の建物だけ固定
        GenerateFirstBuilding();

        // その後の建物を生成
        for (int i = 0; i < 9; i++)
        {
            GenerateBuilding();
        }
    }

    private void Update()
    {
        // プレイヤーの前方に十分な建物がなければ生成
        if (player.position.x + generateDistance > nextBuildX)
        {
            GenerateBuilding();
        }
    }

    private void GenerateFirstBuilding()
    {
        nextBuildX=firstBuildingPosition.x;
        float standardY=firstBuildingPosition.y;
        Vector3 position =new Vector3(
            nextBuildX+firstBuildingScale.x/2,
            standardY+firstBuildingScale.y/2,
            firstBuildingPosition.z
        );

        GameObject firstBuilding = Instantiate(
            buildingPrefab,
            position,
            Quaternion.identity,
            transform
        );

        firstBuilding.transform.localScale = firstBuildingScale;

        // 最初の建物の右端を計算
        float rightEdge =
            firstBuildingPosition.x + firstBuildingScale.x;

        // 次の建物は最初の建物の右側から生成
        nextBuildX = rightEdge;

        // 最初の建物も1つの地形として扱うので、
        // Noiseの位置も進めておく
        noiseOffset += noiseScale;

        currentTopY=firstBuildingPosition.y+firstBuildingScale.y;
    }

    private void GenerateBuilding()
    {
        // 建物の幅をランダムに決定
        float width = Random.Range(
            minBuildingWidth,
            maxBuildingWidth
        );

        // 建物同士の隙間をランダムに決定
        float gap = Random.Range(
            minGap,
            maxGap
        );

        // Perlin Noiseから高さを取得
        float noise = Mathf.PerlinNoise(
            noiseOffset,
            0f
        );

        float heightDifference=Mathf.Lerp(
            -maxHeightDifference,
            maxHeightDifference,
            noise
        );

        float topY=currentTopY+heightDifference;

        topY=Mathf.Clamp(
            topY,
            minHeight,
            maxHeight
        );

        // nextBuildXは「次の建物の左端」
        // 先に隙間を空ける
        nextBuildX += gap;

        float buildingHeight=topY;

        Vector3 position = new Vector3(
            nextBuildX + width / 2f,
            buildingHeight / 2f,
            firstBuildingPosition.z
        );

        GameObject building = Instantiate(
            buildingPrefab,
            position,
            Quaternion.identity,
            transform
        );

        building.transform.localScale = new Vector3(
            width,
            buildingHeight,
            firstBuildingScale.z
        );

        // 次の建物の左端を更新
        nextBuildX += width;

        // Perlin Noiseを進める
        noiseOffset += noiseScale;

        currentTopY=topY;
    }

}