using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_pinkman : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // 현재 움직이는 속도

    [Header("Weather System Tuning")]
    public float originalJumpPower = 10f;
    public float originalMaxSpeed = 5f; // 원래 기본 속도 (기본값: 5)

    private float currentJumpPower;
    private float currentMaxSpeed;

    private bool isSlippery = false;
    private bool isFrozen = false;

    private float horizontalInput = 0f;
    Rigidbody2D rigid;
    Animator animator;

    private float groundCheckThreshold = 0.1f;
    private bool facingRight = true;

    [Header("Physics Materials")]
    public PhysicsMaterial2D normalMaterial;
    public PhysicsMaterial2D slipperyMaterial;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentJumpPower = originalJumpPower;
        currentMaxSpeed = originalMaxSpeed;
        moveSpeed = originalMaxSpeed; // 처음에 원래 속도로 지정

        if (rigid != null && normalMaterial != null)
            rigid.sharedMaterial = normalMaterial;
    }

    void Update()
    {
        // 이제 한파에 걸려도(isFrozen) 입력을 막지 않고 계속 부드럽게 움직일 수 있습니다!
        horizontalInput = 0f;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            horizontalInput = -1f;
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            horizontalInput = 1f;

        if (horizontalInput > 0 && facingRight) Flip();
        else if (horizontalInput < 0 && !facingRight) Flip();

        if (animator != null)
        {
            animator.SetBool("isRun", horizontalInput != 0 && IsGrounded());
            animator.SetBool("isJump", !IsGrounded());
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, currentJumpPower);
        }
    }

    void FixedUpdate()
    {
        float targetVelocityX = horizontalInput * moveSpeed;

        if (isSlippery)
        {
            if (horizontalInput != 0)
                rigid.linearVelocity = new Vector2(targetVelocityX, rigid.linearVelocity.y);
        }
        else
        {
            rigid.linearVelocity = new Vector2(targetVelocityX, rigid.linearVelocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Mathf.Abs(rigid.linearVelocity.y) < groundCheckThreshold;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void ApplySlipperyPhysics(bool active)
    {
        isSlippery = active;
        if (rigid != null)
        {
            if (isSlippery && slipperyMaterial != null)
                rigid.sharedMaterial = slipperyMaterial;
            else if (normalMaterial != null)
                rigid.sharedMaterial = normalMaterial;
        }
    }

    public void ReduceJumpPower() { currentJumpPower = originalJumpPower * 0.66f; }
    public void ResetJumpPower() { currentJumpPower = originalJumpPower; }

    // ★ 한파 발동: 속도를 느리게 만듭니다 (예: 1f로 고정)
    public void FreezePlayer()
    {
        isFrozen = true;
        moveSpeed = 1f; // 1초 동안 기어가는 속도
        Debug.Log("핑크맨이 한파로 인해 느려졌습니다!");
    }

    // ★ 한파 해제 / 핫팩 사용: 원래 속도로 복구 (증가)
    public void UnfreezePlayer()
    {
        isFrozen = false;
        moveSpeed = originalMaxSpeed; // 원래 속도(5f)로 복구!
        Debug.Log("핑크맨의 속도가 정상으로 복구되었습니다!");
    }

    // 아이템 충돌 처리 통합
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.instance == null) return;

        if (collision.CompareTag("Item"))
        {
            string round = GameManager.instance.currentRoundName;
            Debug.Log($"{round} 아이템 획득!");
            GameManager.instance.EarnWeatherItem(round);
            Destroy(collision.gameObject);
        }
    }
}