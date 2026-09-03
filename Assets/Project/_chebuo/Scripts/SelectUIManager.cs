using UnityEngine;
using UnityEngine.UI;

public class SelectUIManager : MonoBehaviour
{
    [SerializeField]private Text gemText;
    [SerializeField]private Text coinText;
    [SerializeField]private PlayerData playerData;

    void Update()
    {
        gemText.text=$"GEM: {playerData.gem}";
        coinText.text=$"COIN: {playerData.coin}";
    }
}