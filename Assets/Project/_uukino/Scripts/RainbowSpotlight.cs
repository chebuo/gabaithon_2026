using UnityEngine;

// スポットライト(Light)を虹色に周期的に変化させる演出用コンポーネント
[RequireComponent(typeof(Light))]
public class RainbowSpotlight : MonoBehaviour
{
    [Header("虹色設定")]
    public float cycleSpeed = 0.5f; // 色相が1周する速さ（1秒あたりの割合）
    [Range(0f, 1f)] public float saturation = 1f;
    [Range(0f, 1f)] public float brightness = 1f;
    public float intensity = 3f;

    private Light spotLight;
    private float hue = 0f;

    void Awake()
    {
        spotLight = GetComponent<Light>();
    }

    void OnEnable()
    {
        hue = 0f;
    }

    void Update()
    {
        hue += cycleSpeed * Time.deltaTime;
        if (hue > 1f) hue -= 1f;

        spotLight.color = Color.HSVToRGB(hue, saturation, brightness);
        spotLight.intensity = intensity;
    }
}
