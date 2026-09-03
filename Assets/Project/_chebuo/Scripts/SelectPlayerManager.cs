using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

public class SelectPlayerManager : MonoBehaviour
{
    [SerializeField]float moveSpeed;

    [SerializeField]private InputActionAsset inputActions;

    [SerializeField]GameObject[] bikkuris;

    InputAction moveAction;
    InputAction selectAction;

    public bool selectBank=false;
    public bool selectCasino=false;
    public bool selectEscape=false;

    Animator animator;
    SelectPlayerController selectPlayerController;
    SceneChanger sceneChanger=new SceneChanger();
    [SerializeField]private PlayerData playerData;
    void Awake()
    {
        animator=this.GetComponent<Animator>();
        selectPlayerController=this.GetComponent<SelectPlayerController>();
        moveAction=inputActions.FindAction("Move");
        selectAction=inputActions.FindAction("Select");
        moveAction.Enable();
        selectAction.Enable();
    }

    void Start()
    {
        SelectLoop().Forget();
    }

    private async UniTask SelectLoop()
    {
        var token=destroyCancellationToken;
        while (true)
        {
            Move();
            ShowBikkuri();
            if(selectAction.triggered)
            {
                Debug.Log("Select");
            }
            await UniTask.Yield(cancellationToken: token);
        }
    }

    private void Move()
    {
        var moveInput=moveAction.ReadValue<Vector2>();
        if(moveInput==Vector2.zero)animator.SetBool("isMove",false);
        else animator.SetBool("isMove",true);
        selectPlayerController.ChangeDir(moveInput);
        selectPlayerController.Move(moveInput*moveSpeed);
    }

    public void ShowBikkuri()
    {
        if(playerData.isClearBank==false&&playerData.isClearCasino==false&&playerData.isClearEscape==false)
        {
            bikkuris[0].SetActive(true);
            bikkuris[1].SetActive(false);
            bikkuris[2].SetActive(false);
        }
        else if(playerData.isClearBank==true&&playerData.isClearCasino==false&&playerData.isClearEscape==false)
        {
            bikkuris[0].SetActive(false);
            bikkuris[1].SetActive(true);
            bikkuris[2].SetActive(false);
        }
        else if(playerData.isClearBank==true&&playerData.isClearCasino==true&&playerData.isClearEscape==false)
        {
            bikkuris[0].SetActive(false);
            bikkuris[1].SetActive(false);
            bikkuris[2].SetActive(true);
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if(col.gameObject.CompareTag("Bank")&&playerData.isClearBank==false&&playerData.isClearCasino==false&&playerData.isClearEscape==false)
        {
            if(selectAction.triggered)
            {
                Debug.Log("Bank");
                selectBank=true;
                sceneChanger.ChangeScene("bob_main2",0);
            }
        }
        if(col.gameObject.CompareTag("Casino")&&playerData.isClearBank==true&&playerData.isClearCasino==false&&playerData.isClearEscape==false)
        {
            if(selectAction.triggered)
            {
                Debug.Log("casino");
                selectCasino=true;
                sceneChanger.ChangeScene("casino",0);
            }
        }
        if(col.gameObject.CompareTag("Escape")&&playerData.isClearBank==true&&playerData.isClearCasino==true&&playerData.isClearEscape==false){
            Debug.Log("escape!!");
            if(selectAction.triggered){
                Debug.Log("Escape");
                selectEscape=true;
                sceneChanger.ChangeScene("chebuo",0);
            }
        }
    }

}