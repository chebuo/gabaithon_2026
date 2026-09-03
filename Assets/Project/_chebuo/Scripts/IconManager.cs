using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class IconManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gemText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private PlayerData playerData;

    private int lastGem;
    private int lastCoin;

    private void Awake()
    {
        lastGem = playerData.gem;
        lastCoin = playerData.coin;

        ShowUpdate();

        ShowUpdateLoop().Forget();
    }

    private async UniTaskVoid ShowUpdateLoop()
    {
        while (this != null)
        {
            if (lastGem != playerData.gem ||
                lastCoin != playerData.coin)
            {
                lastGem = playerData.gem;
                lastCoin = playerData.coin;

                ShowUpdate();
            }

            await UniTask.Delay(100);
        }
    }

    private void ShowUpdate()
    {
        gemText.text = $"{playerData.gem}";
        coinText.text = $"{playerData.coin}";
    }
}