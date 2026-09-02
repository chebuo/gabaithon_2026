using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class SceneChanger
{
    public void ChangeScene(string sceneName,float delayTime)
    {
        UniTask.Delay((int)(1000*delayTime));
        LoadScene();
        void LoadScene()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
