using UnityEngine;
using System.Collections;

public class PoliceMovementBob : MonoBehaviour
{
    public Transform target; // 追跡するターゲット（プレイヤー）
    public bool isInBank = false; // 自分自身が銀行内にいるかどうかのフラグ
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;
    [SerializeField] private int attackTime = 800; // 攻撃モーションに入ってから実際に攻撃が入るまで
    [SerializeField] private int attackCooldownTime = 2000; // 攻撃後2秒待機
    [SerializeField] private int attackDamage = 10; // 攻撃力
    [SerializeField] private GameObject hitBox;

    private Rigidbody rb;
    private float attackCooldownRemaining;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (attackCooldownRemaining > 0f)
        {
            attackCooldownRemaining = Mathf.Max(0f, attackCooldownRemaining - Time.fixedDeltaTime);
            StopMovement();

            if (attackCooldownRemaining <= 0f && animator != null)
            {
                animator.SetBool("iscooltime", false);
            }

            return;
        }

        if (IsPlayerInHitBox())
        {
            StartAttack();
            return;
        }

        Vector3 destination = targetPosition;
        if (GameManagerBob.instance != null &&
            GameManagerBob.instance.isPlayerInBank == isInBank &&
            target != null)
        {
            destination = target.position;
        }

        Vector3 moveDirection = destination - rb.position;
        moveDirection.y = 0f;

        if (moveDirection.sqrMagnitude <= stoppingDistance * stoppingDistance)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.x = 0f;
            velocity.z = 0f;
            rb.linearVelocity = velocity;
            return;
        }

        moveDirection.Normalize();
        rb.MoveRotation(Quaternion.LookRotation(moveDirection));

        Vector3 velocityWhileMoving = rb.linearVelocity;
        velocityWhileMoving.x = moveDirection.x * moveSpeed;
        velocityWhileMoving.z = moveDirection.z * moveSpeed;
        rb.linearVelocity = velocityWhileMoving;
    }

    private bool IsPlayerInHitBox()
    {
        if (hitBox == null)
        {
            return false;
        }

        Collider hitBoxCollider = hitBox.GetComponent<Collider>();
        if (hitBoxCollider == null || !hitBoxCollider.enabled)
        {
            return false;
        }

        Collider[] colliders = Physics.OverlapBox(
            hitBoxCollider.bounds.center,
            hitBoxCollider.bounds.extents,
            hitBox.transform.rotation);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    private void StartAttack()
    {
        attackCooldownRemaining = attackCooldownTime / 1000f;
        StopMovement();

        if (animator != null)
        {
            animator.SetTrigger("attackbaton");
            animator.SetBool("iscooltime", true);
        }

        StartCoroutine(AttackHitBoxRoutine());
    }

    private IEnumerator AttackHitBoxRoutine()
    {
        yield return new WaitForSeconds(attackTime / 1000f);

        if (hitBox != null)
        {
            hitBox.tag = "damageArea";
            yield return new WaitForSeconds(0.1f);
            hitBox.tag = "Untagged";
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bank"))
        {
            isInBank = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bank"))
        {
            isInBank = false;
        }
    }
}
