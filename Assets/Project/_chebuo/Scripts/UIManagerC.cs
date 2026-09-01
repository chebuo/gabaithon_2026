using UnityEngine;
using UnityEngine.UI;

public class UIManagerC : MonoBehaviour
{
    public GameObject pausePanel;
    [SerializeField]private Text timeText;

    public void Init()
    {
        pausePanel.SetActive(false);
    }

    public void ShowTimeText(int currentTime)
    {
        timeText.text=$"TIME:{currentTime.ToString()}";
    }



}