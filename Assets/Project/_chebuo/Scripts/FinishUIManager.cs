using UnityEngine;
using TMPro;

public class FinishUIManager : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI gemText;
    [SerializeField]TextMeshProUGUI coinText;

    [SerializeField]private GameObject clearPanel;
    [SerializeField]private GameObject failPanel;
    [SerializeField]private GameObject warningPanel;

    SceneChanger sceneChanger=new SceneChanger();
    [SerializeField]private PlayerData playerData;

    void Awake()
    {
        if(EscapeGameManager.isClear)
        {
            clearPanel.SetActive(true);
            failPanel.SetActive(false);
        }
        else
        {
            clearPanel.SetActive(false);
            failPanel.SetActive(true);
        }
        ShowGemText();
        ShowCoinText();
    }

    public void ShowGemText()
    {
        gemText.text=$"{playerData.gem}";
    }

    public void ShowCoinText()
    {
        coinText.text=$"{playerData.coin}";
    }

    public void OnClickRetryButton()
    {
        if (playerData.coin == 0)
        {
            playerData.isGameOver=true;
            sceneChanger.ChangeScene("SelectScene",0);
            return;
        }
        playerData.coin/=2;
        sceneChanger.ChangeScene("SelectScene",0);
    }

    public void OnClickSelectButton()
    {
        sceneChanger.ChangeScene("SelectScene",0);
    }

    public void OnClickRiviveButton()
    {
        if(playerData.gem>=playerData.ReviveCost)
        {
            playerData.isRevive=true;
            playerData.gem-=playerData.ReviveCost;
            sceneChanger.ChangeScene("chebuo",0);
        }
        else
        {
            playerData.isRevive=false;
            warningPanel.SetActive(true);
        }
    }

    public void OnClickCloseButton()
    {
        warningPanel.SetActive(false);
    }
}