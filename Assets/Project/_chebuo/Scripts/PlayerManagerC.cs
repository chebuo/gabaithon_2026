using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

public class PlayerManagerC : MonoBehaviour
{
    public float jumpForce;
    public float moveSpeed;

    [SerializeField]private InputActionAsset inputActions;
    InputAction jumpAction;

    public bool isGame=false;
    [SerializeField]private GameObject bottomR;
    [SerializeField]private GameObject bottomL;
    public bool isGround=false;
    private bool isMiss=false;
    public bool isDead=false;
    [SerializeField]private LayerMask groundLayer;
    [SerializeField]private float groundCheckDistance=0.1f;
    [SerializeField]private float missCheckDistance=0.1f;
    public PlayerStateC currentState=PlayerStateC.moving;

    PlayerControllerC playerController;
    Animator animator;
    void Awake()
    {
        playerController=this.GetComponent<PlayerControllerC>();
        animator=this.GetComponent<Animator>();
        jumpAction=inputActions.FindAction("Jump");
        jumpAction.Enable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StateLoop().Forget();
        ActionLoop().Forget();
    }

    private async UniTask StateLoop()
    {
        var token = destroyCancellationToken;
        await UniTask.WaitUntil(()=>isGame,cancellationToken: token);
        while (isGame)
        {
            AllCheck();
            switch (currentState)
            {
                case PlayerStateC.moving:
                    Move();
                    break;
                case PlayerStateC.missing:
                    StopMove();
                    break;
                default:
                    break;
            }
            await UniTask.Yield(cancellationToken: token);
        }
    }

    private async UniTask ActionLoop()
    {
        var token = destroyCancellationToken;
        await UniTask.WaitUntil(()=>isGame,cancellationToken: token);
        while (isGame)
        {
            CheckInput();
            await UniTask.Yield(cancellationToken: token);
        }
    }

    private void CheckInput()
    {
        if (jumpAction.WasPressedThisFrame()&&isGround)
        {
            Jump();
        }
    }

    public void Move()
    {
        playerController.Move(moveSpeed);
    }

    public void StopMove()
    {
        moveSpeed=0;
    }

    public void Jump()
    {
        playerController.Jump(jumpForce);
    }

    public void AllCheck()
    {
        CheckGround();
        CheckMiss();
        CheckDead();
    }

    public void CheckGround()
    {
        bool isHitR=Physics.Raycast(
            bottomR.transform.position,
            Vector3.down,
            out RaycastHit hitR,
            groundCheckDistance,
            groundLayer
        );
        bool isHitL=Physics.Raycast(
            bottomL.transform.position,
            Vector3.down,
            out RaycastHit hitL,
            groundCheckDistance,
            groundLayer
        );
        isGround=isHitR||isHitL;
        animator.SetBool("isGround",isGround);
    }

    public void CheckMiss()
    {
        bool isHitR=Physics.Raycast(
            bottomR.transform.position,
            Vector3.right,
            missCheckDistance,
            groundLayer
        );
        isMiss=isHitR;
        if(isMiss)ChangeState(PlayerStateC.missing);
    }

    public void CheckDead()
    {
        if (this.transform.position.y < 0)
        {
            ChangeState(PlayerStateC.dead);
            Debug.Log(currentState);
        }
    }

    public void ChangeState(PlayerStateC state)
    {
        if(currentState==state)return;
        currentState=state;
    }
}
