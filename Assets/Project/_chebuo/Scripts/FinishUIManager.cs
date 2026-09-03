using UnityEngine;
using TMPro;

public class FinishUIManager : MonoBehaviour
{
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
    }

    public void OnClickRetryButton()//GameOver時に押すボタン
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

    public void OnClickSelectButton()//GameClear時に押すボタン
    {
        playerData.isClearBank=false;
        playerData.isClearCasino=false;
        playerData.isClearEscape=false;
        sceneChanger.ChangeScene("SelectScene",0);
    }

    public void OnClickRiviveButton()//GameOver時に押す復活ボタン
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

    public void OnClickCloseButton()//GameOver時に押す復活警告パネルの閉じるボタン
    {
        warningPanel.SetActive(false);
    }
}