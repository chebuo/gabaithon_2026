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
    private bool isGround=false;
    private bool isMiss=false;
    [SerializeField]private LayerMask groundLayer;
    [SerializeField]private float groundCheckDistance=0.1f;
    [SerializeField]private float missCheckDistance=0.1f;
    public PlayerStateC currentState=PlayerStateC.moving;

    PlayerControllerC playerController;
    void Awake()
    {
        playerController=this.GetComponent<PlayerControllerC>();
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
            CheckGround();
            CheckMiss();
            switch (currentState)
            {
                case PlayerStateC.moving:
                    Move();
                    break;
                case PlayerStateC.dead:
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

    public void Jump()
    {
        playerController.Jump(jumpForce);
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
        Debug.Log($"{isHitL},{isHitR}");
        isGround=isHitR||isHitL;
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
        if(isMiss)ChangeState(PlayerStateC.dead);
    }

    public void ChangeState(PlayerStateC state)
    {
        currentState=state;
    }
}
