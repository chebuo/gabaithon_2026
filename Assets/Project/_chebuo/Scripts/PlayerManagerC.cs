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
    private bool isGround=false;
    [SerializeField]private LayerMask groundLayer;
    [SerializeField]private float groundCheckDistance=0.1f;
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
        _=StateLoop();
        _=ActionLoop();
    }

    private async UniTask StateLoop()
    {
        await UniTask.WaitUntil(()=>isGame);
        while (isGame)
        {
            CheckGround();
            switch (currentState)
            {
                case PlayerStateC.moving:
                    Move();
                    break;
                case PlayerStateC.dead:
                    break;
            }
            await UniTask.Yield();
        }
    }

    private async UniTask ActionLoop()
    {
        await UniTask.WaitUntil(()=>isGame);
        while (isGame)
        {
            CheckInput();
            await UniTask.Yield();
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
        bool isHit=Physics.Raycast(
            transform.position,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer
        );
        isGround=isHit;
    }
}
