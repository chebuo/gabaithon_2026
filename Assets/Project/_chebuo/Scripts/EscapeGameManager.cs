using UnityEngine;
using Cysharp.Threading.Tasks;

public class EscapeGameManager : MonoBehaviour
{
    public float gameTime=180;
    private float currentTime=0;

    public bool isClear=false;

    SceneChanger sceneChanger=new SceneChanger();
    [SerializeField]PlayerManagerC playerManager;
    [SerializeField]UIManagerC uiManager;

    public EscapeGameState currentState=EscapeGameState.idle;

    void Awake()
    {
        uiManager.Init();
    }

    void Start()
    {
        GameLoop().Forget();
    }

    private async UniTask GameLoop()
    {
        await PlayGame();
        await FinishGame();
    }

    public async UniTask PlayGame()
    {
        ChangeState(EscapeGameState.playing);
        _=Timer();
        await UniTask.WaitUntil(()=>playerManager.currentState==PlayerStateC.dead||currentState==EscapeGameState.finish);
    }

    private async UniTask FinishGame()
    {
        Debug.Log(isClear);
        
        sceneChanger.ChangeScene("FailEscape",0);
    }

    public void PauseGame()
    {
        ChangeState(EscapeGameState.pause);
        Time.timeScale=0;
        uiManager.pausePanel.SetActive(true);
    }

    public void ContinueGame()
    {
        ChangeState(EscapeGameState.playing);
        Time.timeScale=1;
        uiManager.pausePanel.SetActive(false);
    }

    public void StopGame()
    {
        ChangeState(EscapeGameState.finish);
        sceneChanger.ChangeScene("SelectScene",0);
    }

    public async UniTask Timer()
    {
        while (currentTime < gameTime)
        {
            if (currentState == EscapeGameState.playing)
            {
                uiManager.ShowTimeText((int)currentTime);
                await UniTask.Delay(1000);
                currentTime++;
            }
            else
            {
                await UniTask.Yield();
            }
        }
        if(!playerManager.isMiss)isClear=true;
        ChangeState(EscapeGameState.finish);
    }


    public void ChangeState(EscapeGameState state)
    {
        if(currentState==state)return;
        currentState=state;
    }
}