using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementBob : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField, Tooltip("水平方向の移動速度 (m/s)")]
    public float moveSpeed = 5f;

    [Header("回転設定")]
    [SerializeField, Tooltip("移動キーを押していない間に回転を止める強さ")]
    private float rotationStopStrength = 10f;

    [SerializeField]
    private Animator animator;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool wasMoving;
    [SerializeField] private Transform camera;
    [SerializeField] private Vector3 cameraDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // 回転で転がってしまうのを防ぎたい場合はInspectorのConstraintsで
        // Freeze Rotation X/Zを設定するか、以下を有効にしてください。
        // rb.freezeRotation = true;
    }

    private void Update()
    {
        moveInput = ReadMoveInput();

        bool isMoving = moveInput.sqrMagnitude > 0f;
        if (isMoving != wasMoving && animator != null)
        {
            animator.SetTrigger(isMoving ? "startrun" : "stop");
        }

        wasMoving = isMoving;
        camera.position = transform.position + cameraDir;
    }

    private void FixedUpdate()
    {
        // 入力をワールド基準のXZ平面移動ベクトルに変換
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        move *= moveSpeed;

        if (move.sqrMagnitude > 0f)
        {
            rb.MoveRotation(Quaternion.LookRotation(move));
        }
        else
        {
            // 停止中は現在の回転速度と逆向きのトルクを加えて徐々に回転を止める
            rb.AddTorque(-rb.angularVelocity * rotationStopStrength, ForceMode.Acceleration);
        }

        // Y方向の速度(重力・ジャンプ等)は維持しつつ、水平方向のみ書き換える
        Vector3 velocity = rb.linearVelocity;
        velocity.x = move.x;
        velocity.z = move.z;
        rb.linearVelocity = velocity;
    }

    /// <summary>
    /// Input Systemの Keyboard.current を使ってWASD入力を取得する
    /// (GetKeyDownなど旧Input系は使用しない)
    /// </summary>
    private Vector2 ReadMoveInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return Vector2.zero;

        Vector2 input = Vector2.zero;

        if (keyboard.wKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.aKey.isPressed) input.x -= 1f;

        return input.normalized;
    }
}