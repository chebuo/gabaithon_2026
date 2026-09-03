using UnityEngine;
using TMPro;

public class SelectUIManager : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI gemText;
    [SerializeField]private TextMeshProUGUI coinText;
    [SerializeField]private PlayerData playerData;

    void Update()
    {
        gemText.text=$"{playerData.gem}";
        coinText.text=$"{playerData.coin}";
    }
}