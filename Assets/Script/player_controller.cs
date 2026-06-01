using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_pinkman : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    public LayerMask groundLayer;

    int jumpCount = 0;
    public int maxJumpCount = 1;

    Rigidbody2D rigid;
    SpriteRenderer sprend;
    Animator anim;
    bool isGrounded;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float h = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h = 1f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h = -1f;
        }

        if (h > 0) sprend.flipX = false;
        else if (h < 0) sprend.flipX = true;

        if (h == 0) anim.SetBool("isRun", false);
        else anim.SetBool("isRun", true);

        // 💡 자기 자신을 찌르지 않도록 시작 위치를 발밑으로 더 내렸습니다.
        Vector2 rayStart = (Vector2)rigid.position + Vector2.down * 0.7f;
        Debug.DrawRay(rayStart, Vector2.down * 0.2f, Color.red);

        RaycastHit2D rayHit = Physics2D.Raycast(rayStart, Vector2.down, 0.2f, groundLayer);

        // 💡 레이캐스트가 닿았고, 그 대상이 플레이어 본인이 아닐 때만 바닥으로 인정합니다.
        if (rayHit.collider != null && rayHit.collider.gameObject != gameObject)
        {
            isGrounded = true;
            jumpCount = 0;
        }
        else
        {
            isGrounded = false;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (jumpCount < maxJumpCount)
            {
                rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0);
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

                jumpCount++;
            }
        }

        anim.SetBool("isJump", !isGrounded);
    }

    void FixedUpdate()
    {
        float h = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h = 1f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h = -1f;
        }

        rigid.linearVelocity = new Vector2(h * maxSpeed, rigid.linearVelocity.y);

        if (h == 0)
        {
            rigid.linearVelocity = new Vector2(0, rigid.linearVelocity.y);
        }
    }
}