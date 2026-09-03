using UnityEngine;
using TMPro;

public class UIManagerC : MonoBehaviour
{
    public GameObject pausePanel;
    [SerializeField]private TextMeshProUGUI timeText;
    [SerializeField]private TextMeshProUGUI coinText;

    [SerializeField]private PlayerData playerData;

    public void Init()
    {
        pausePanel.SetActive(false);
        ShowCoinText();
    }

    public void ShowTimeText(int currentTime)
    {
        timeText.text=$"TIME:{currentTime}";
    }

    public void ShowCoinText()
    {
        coinText.text=$"{playerData.coin}";
    }




}