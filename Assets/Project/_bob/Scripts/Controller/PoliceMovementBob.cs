using UnityEngine;
using System.Collections;

public class PoliceMovementBob : MonoBehaviour
{
    [SerializeField] private GameObject baton;
    [SerializeField] private GameObject sks;
    public AttackType attackType;
    public Transform target; // 追跡するターゲット（プレイヤー）
    public bool isInBank = false; // 自分自身が銀行内にいるかどうかのフラグ
    [SerializeField] private int health = 100;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;
    [SerializeField] private int attackTimeBaton = 800; // 攻撃モーションに入ってから実際に攻撃が入るまで
    [SerializeField] private int attackCooldownTimeBaton = 2000; // 攻撃後2秒待機
    [SerializeField] private int attackDamageBaton = 10; // 攻撃力
    [SerializeField] private GameObject hitBox;
    [SerializeField] private int attackTimeSKS = 1000; // 攻撃モーションに入ってから実際に攻撃が入るまで
    [SerializeField] private int attackCooldownTimeSKS = 3000; // 攻撃後3秒待機
    [SerializeField] private float attackRengeSKS = 10f; // 攻撃範囲
    [SerializeField] private float attackAngle = 30f; // プレイヤーを正面に捉えていると判定する角度
    [SerializeField] private GameObject bulletPrefab;
    private float attackDamageSKS = 0;
    [SerializeField] private float hitKnockbackForce = 3f;
    [SerializeField] private float deathKnockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private Rigidbody rb;
    private float attackCooldownRemaining;
    private Animator animator;
    private bool isInvincible;
    private bool isDead;
    private float knockbackRemaining;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        animator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        if (attackType == AttackType.Baton) baton.SetActive(true);
        if (attackType == AttackType.SKS) sks.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        if (knockbackRemaining > 0f)
        {
            knockbackRemaining = Mathf.Max(0f, knockbackRemaining - Time.fixedDeltaTime);
            return;
        }

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

        if (CanAttack())
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

    private bool CanAttack()
    {
        if (GameManagerBob.instance == null ||
            GameManagerBob.instance.isPlayerInBank != isInBank ||
            target == null ||
            !IsFacingTarget())
        {
            return false;
        }

        if (attackType == AttackType.Baton)
        {
            return IsPlayerInHitBox();
        }

        return attackType == AttackType.SKS &&
            Vector3.Distance(transform.position, target.position) <= attackRengeSKS;
    }

    private bool IsFacingTarget()
    {
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0f;

        if (directionToTarget.sqrMagnitude <= 0f)
        {
            return true;
        }

        return Vector3.Angle(transform.forward, directionToTarget) <= attackAngle;
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
        bool isBatonAttack = attackType == AttackType.Baton;
        attackCooldownRemaining = (isBatonAttack ? attackCooldownTimeBaton : attackCooldownTimeSKS) / 1000f;
        StopMovement();

        if (animator != null)
        {
            animator.SetTrigger(isBatonAttack ? "attackbaton" : "attacksks");
            animator.SetBool("iscooltime", true);
        }

        StartCoroutine(isBatonAttack ? AttackHitBoxRoutine() : AttackSKSRoutine());
    }

    private IEnumerator AttackHitBoxRoutine()
    {
        yield return new WaitForSeconds(attackTimeBaton / 1000f);

        if (hitBox != null)
        {
            hitBox.tag = "damageArea";
            yield return new WaitForSeconds(0.1f);
            hitBox.tag = "Untagged";
        }
    }

    private IEnumerator AttackSKSRoutine()
    {
        yield return new WaitForSeconds(attackTimeSKS / 1000f);

        if (bulletPrefab != null)
        {
            Instantiate(bulletPrefab, transform.position + transform.forward + transform.up * 1.4f, transform.rotation);

            if (animator != null)
            {
                animator.SetTrigger("shot");
            }
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

    private void OnTriggerStay(Collider other)
    {
        if (isDead || isInvincible || !other.CompareTag("playerattack"))
        {
            return;
        }

        health -= 40;
        ApplyKnockback(other, hitKnockbackForce);

        if (health <= 0)
        {
            Die(other);
            return;
        }

        StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(0.2f);
        isInvincible = false;
    }

    private void Die(Collider attackCollider)
    {
        animator.SetTrigger("death");
        isDead = true;
        StopAllCoroutines();
        attackCooldownRemaining = 0f;

        if (animator != null)
        {
            animator.SetBool("iscooltime", false);
        }

        StopMovement();
        rb.constraints &= ~(RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ);
        ApplyKnockback(attackCollider, deathKnockbackForce);
        Destroy(gameObject, 2f);
    }

    private void ApplyKnockback(Collider attackCollider, float force)
    {
        Vector3 knockbackDirection = target != null
            ? transform.position - target.position
            : transform.position - attackCollider.transform.position;
        knockbackDirection.y = 0f;

        if (knockbackDirection.sqrMagnitude <= 0f)
        {
            knockbackDirection = -attackCollider.transform.forward;
            knockbackDirection.y = 0f;
        }

        if (knockbackDirection.sqrMagnitude > 0f)
        {
            knockbackRemaining = knockbackDuration;
            rb.AddForce(knockbackDirection.normalized * force, ForceMode.VelocityChange);
        }
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
