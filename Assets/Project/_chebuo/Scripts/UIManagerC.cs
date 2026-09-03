using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManagerC : MonoBehaviour
{
    public GameObject pausePanel;
    [SerializeField]private Text timeText;

    public void Init()
    {
        pausePanel.SetActive(false);
    }

    public void ShowTimeText(int gameTime,int currentTime)
    {
        timeText.text=$"TIME:{gameTime-currentTime}";
    }
}