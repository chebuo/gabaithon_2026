using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

public class SelectPlayerManager : MonoBehaviour
{
    [SerializeField]float moveSpeed;

    [SerializeField]private InputActionAsset inputActions;

    InputAction moveAction;
    InputAction selectAction;

    public bool selectBank=false;
    public bool selectCasino=false;

    Animator animator;
    SelectPlayerController selectPlayerController;
    SceneChanger sceneChanger=new SceneChanger();
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
        Debug.Log(moveInput);
        if(moveInput==Vector2.zero)animator.SetBool("isMove",false);
        else animator.SetBool("isMove",true);
        selectPlayerController.Move(moveInput*moveSpeed);
    }

    private void OnTriggerStay(Collider col)
    {
        if(col.gameObject.CompareTag("Bank"))
        {
            if(selectAction.triggered)
            {
                Debug.Log("Bank");
                selectBank=true;
                sceneChanger.ChangeScene("bob_main2",0);
            }
        }
        // if(col.gameObject.CompareTag("casino"))
        // {
        //     if(selectAction.triggered)
        //     {
        //         Debug.Log("casino");
        //         selectCasino=true;
        //         //sceneChanger.ChangeScene("casino",0);
        //     }
        // }
    }
}