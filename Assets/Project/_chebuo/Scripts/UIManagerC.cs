using UnityEngine;
using UnityEngine.UI;

public class UIManagerC : MonoBehaviour
{
    public GameObject pausePanel;
    [SerializeField]private Text timeText;
    [SerializeField]private Text coinText;

    [SerializeField]private PlayerData playerData;

    public void Init()
    {
        pausePanel.SetActive(false);
        ShowCoinText();
    }

    public void ShowTimeText(int currentTime)
    {
        timeText.text=$"TIME:{currentTime.ToString()}";
    }

    public void ShowCoinText()
    {
        coinText.text=$"COIN:{playerData.coin.ToString()}";
    }




}