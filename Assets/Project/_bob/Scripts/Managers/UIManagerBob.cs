using UnityEngine;
using TMPro;

public class UIManagerBob : MonoBehaviour
{
    public static UIManagerBob Instance;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text goalScore;
    [SerializeField] private TMP_Text bag;
    [SerializeField] private TMP_Text message;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScore(int n, Color c)
    {
        score.text = "Score: " + n.ToString();
        score.color = c;
    }
    public void SetGoalScore(int n)
    {
        goalScore.text = "Goal: " + n.ToString();
    }
    public void SetBag(int n, Color c)
    {
        bag.text = "Bag: " + n.ToString();
        bag.color = c;
    }
    public void ShowDialog(string s, Color c)
    {
        // ダイアログ表示処理を実装する
        message.text = s;
        message.color = c;
    }
}
