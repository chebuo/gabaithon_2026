using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementBob : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField, Tooltip("水平方向の移動速度 (m/s)")]
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // 回転で転がってしまうのを防ぎたい場合はInspectorのConstraintsで
        // Freeze Rotation X/Zを設定するか、以下を有効にしてください。
        // rb.freezeRotation = true;
    }

    private void Update()
    {
        moveInput = ReadMoveInput();
    }

    private void FixedUpdate()
    {
        // カメラ・ワールド基準ではなく、自身の向き基準でXZ平面移動ベクトルを作成
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        move = transform.TransformDirection(move) * moveSpeed;

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