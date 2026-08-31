using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger
{
    float currentTime=0;
    float waitTime=0.8f;
    public void ChangeScene(string sceneName)
    {
        currentTime += Time.deltaTime;
        if(currentTime >= waitTime)LoadScene();
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
