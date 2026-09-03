using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    [SerializeField] private GameObject attackBox;
    [SerializeField] private int attackCooldownTime = 500; // 攻撃後1秒待機
    [SerializeField] private int attackTime = 200; // 攻撃モーションに入ってから実際に攻撃が入るまで
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject meleeWeapon;
    [SerializeField] private bool isShootable = true;
    [SerializeField] private int shotDelay = 500;
    [SerializeField] private int shotCount = 1;
    [SerializeField] private GameObject bulletPrefab;

    private float attackCooldownRemaining;
    private float shotDelayRemaining;
    private bool isAiming;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        SetWeaponVisibility(false);

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
        if (camera != null)
        {
            camera.position = transform.position + cameraDir;
        }

        bool canAct = attackCooldownRemaining <= 0f;
        bool leftClickPressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool rightClickHeld = Mouse.current != null && Mouse.current.rightButton.isPressed;
        bool startedPunch = false;

        if (canAct && leftClickPressed)
        {
            StartCoroutine(PunchRoutine());
            startedPunch = true;
        }

        bool shouldAim = canAct && !startedPunch && isShootable && rightClickHeld;
        if (shouldAim != isAiming)
        {
            isAiming = shouldAim;
            shotDelayRemaining = isAiming ? shotDelay / 1000f : 0f;
            SetWeaponVisibility(isAiming);
            if (animator != null)
            {
                animator.SetBool("aim", isAiming);
            }

            if (!isAiming)
            {
                shotDelayRemaining = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (attackCooldownRemaining > 0f)
        {
            attackCooldownRemaining = Mathf.Max(0f, attackCooldownRemaining - Time.fixedDeltaTime);
            StopMovement();

            if (attackCooldownRemaining <= 0f && animator != null)
            {
                animator.SetBool("cooltime", false);
            }

            return;
        }

        if (isAiming)
        {
            RotateTowardsInput();
            StopMovement();
            UpdateShooting();
            return;
        }

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

    private IEnumerator PunchRoutine()
    {
        attackCooldownRemaining = attackCooldownTime / 1000f;
        isAiming = false;
        SetWeaponVisibility(false);
        StopMovement();

        if (animator != null)
        {
            animator.SetBool("aim", false);
            animator.SetTrigger("panch");
            animator.SetBool("cooltime", true);
        }

        yield return new WaitForSeconds(attackTime / 1000f);

        if (attackBox != null)
        {
            attackBox.tag = "playerattack";
            yield return new WaitForSeconds(0.1f);
            attackBox.tag = "Untagged";
        }
    }

    private void UpdateShooting()
    {
        shotDelayRemaining -= Time.fixedDeltaTime;
        if (shotDelayRemaining > 0f)
        {
            return;
        }

        Shoot();
        shotDelayRemaining = shotDelay / 1000f;
    }

    private void Shoot()
    {
        if (bulletPrefab == null || shotCount <= 0)
        {
            return;
        }

        float spreadStep = shotCount > 1 ? 30f / (shotCount - 1) : 0f;
        float spreadStart = -15f;
        for (int i = 0; i < shotCount; i++)
        {
            float angle = shotCount > 1 ? spreadStart + spreadStep * i : 0f;
            Quaternion rotation = transform.rotation * Quaternion.Euler(0f, angle, 0f);
            GameObject bullet = Instantiate(bulletPrefab, transform.position + rotation * Vector3.forward, rotation);
            bullet.transform.tag = "playerattack";
        }
    }

    private void RotateTowardsInput()
    {
        Vector3 rotationDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        if (rotationDirection.sqrMagnitude > 0f)
        {
            rb.MoveRotation(Quaternion.LookRotation(rotationDirection));
        }
    }

    private void StopMovement()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0f;
        velocity.z = 0f;
        rb.linearVelocity = velocity;
        rb.angularVelocity = Vector3.zero;
    }

    private void SetWeaponVisibility(bool aiming)
    {
        if (meleeWeapon != null)
        {
            meleeWeapon.SetActive(!aiming);
        }

        if (gun != null)
        {
            gun.SetActive(aiming);
        }
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