using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_pinkman : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;          // 플레이어의 실시간 현재 이동 속도

    [Header("Weather System Tuning")]
    public float originalJumpPower = 10f; // 원래 기본 점프력
    public float originalMaxSpeed = 5f;   // 원래 기본 이동 속도

    private float currentJumpPower;       // 날씨에 따라 실시간 변화하는 현재 점프력
    private float currentMaxSpeed;        // 실시간 최대 속도 제한용 변수

    private bool isSlippery = false;      // 1라운드: 미끄러움 상태 여부
    private bool isFrozen = false;        // 3라운드: 속도 상태 여부

    private float horizontalInput = 0f;   // 좌우 방향 입력값 (-1, 0, 1)
    Rigidbody2D rigid;
    Animator animator;

    private float groundCheckThreshold = 0.1f; // 바닥 착지 여부를 판정할 Y축 속도 임계값
    private bool facingRight = true;           // 플레이어가 현재 오른쪽을 보고 있는지

    [Header("Physics Materials")]
    public PhysicsMaterial2D normalMaterial;   // 평소에 쓰는 일반 마찰력 물리 재질
    public PhysicsMaterial2D slipperyMaterial; // 빗길에 미끄러지는 얼음 마찰력 물리 재질

    void Awake()
    {
        // 컴포넌트 참조 및 기본 스펙 수치 초기화
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentJumpPower = originalJumpPower;
        currentMaxSpeed = originalMaxSpeed;
        moveSpeed = originalMaxSpeed;

        // 시작할 때는 일반 물리 재질을 기본으로 장착
        if (rigid != null && normalMaterial != null)
            rigid.sharedMaterial = normalMaterial;
    }

    void Update()
    {
        // New Input System 기반 키보드 입력 알고리즘
        horizontalInput = 0f;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            horizontalInput = -1f; // 왼쪽 이동
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            horizontalInput = 1f;  // 오른쪽 이동

        // Flip 조건문 알고리즘
        if (horizontalInput > 0 && facingRight) Flip();
        else if (horizontalInput < 0 && !facingRight) Flip();

        // 애니메이션 파라미터 제어
        if (animator != null)
        {
            // 움직이고 있으면서 동시에 바닥에 붙어있을 때만 달리기 애니메이션 재생
            animator.SetBool("isRun", horizontalInput != 0 && IsGrounded());
            // 공중에 떠 있는 상태라면 무조건 점프 애니메이션 재생
            animator.SetBool("isJump", !IsGrounded());
        }

        // 점프 입력 알고리즘
        // 스페이스바를 누른 순간과 착지 상태를 모두 만족할 때 점프
        if (Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, currentJumpPower);
        }
    }

    void FixedUpdate()
    {
        // 입력 방향에 현재 속도를 곱해 목표 물리 속도를 계산한 뒤, 리지드바디의 X축 속도에 즉시 대입
        float targetVelocityX = horizontalInput * moveSpeed;

        if (isSlippery)
        {
            // 미끄러운 상태일 때는 입력이 있을 때만 속도를 직접 제어하여 관성에 의해 미끄러지도록 유도
            if (horizontalInput != 0)
                rigid.linearVelocity = new Vector2(targetVelocityX, rigid.linearVelocity.y);
        }
        else
        {
            rigid.linearVelocity = new Vector2(targetVelocityX, rigid.linearVelocity.y);
        }
    }

    // 플레이어의 Y축 물리 속도가 0에 가까운지 체크하여 바닥 착지 여부를 반환하는 알고리즘
    private bool IsGrounded()
    {
        return Mathf.Abs(rigid.linearVelocity.y) < groundCheckThreshold;
    }

    // 플레이어의 좌우 스케일 X값을 반전시켜 이미지를 좌우로 돌려주는 알고리즘
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // 1라운드 비 기믹: GameManager에서 미끄러운 물리 재질을 꼈다 뺐다 제어하는 함수
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

    // 2라운드 폭염 기믹: 외부에서 점프력을 약화하거나 원래대로 원상복구 시키는 함수
    public void ReduceJumpPower() { currentJumpPower = originalJumpPower * 0.66f; } // 점프력 감소
    public void ResetJumpPower() { currentJumpPower = originalJumpPower; }

    // 3라운드 한파 기믹: 외부에서 추위 상태를 발동시켜 이동 속도를 느리게 만드는 함수
    public void FreezePlayer()
    {
        isFrozen = true;
        moveSpeed = 1f; //1f, 속도 저하
        Debug.Log("3ROUND, 한파로 인해 속도 저하");
    }

    // 3라운드 한파 해제 / 핫팩 아이템 버프: 추위 상태를 풀고 속도를 정상으로 복구하는 함수
    public void UnfreezePlayer()
    {
        isFrozen = false;
        moveSpeed = originalMaxSpeed; // 원래 기본 속도(5f)로 원상복구
        Debug.Log("3ROUND, 속도가 정상으로 복구");
    }

    // 플레이어가 맵에 배치된 특정 트리거 영역에 들어왔을 때 실행되는 통합 충돌 알고리즘
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.instance == null) return;

        // 날씨 대응 아이템 박스와 충돌했을 때
        if (collision.CompareTag("Item"))
        {
            string round = GameManager.instance.currentRoundName;
            Debug.Log($"{round} 아이템 획득");

            // SoundManager와 연동하여 현재 라운드 이름에 알맞은 획득 효과음 재생
            if (SoundManager.instance != null)
            {
                if (round == "1ROUND") SoundManager.instance.PlayUmbrella();
                else if (round == "2ROUND") SoundManager.instance.PlayIce();
                else if (round == "3ROUND") SoundManager.instance.PlayHotPack();
            }

            // GameManager에게 아이템을 획득했다는 신호를 보내 버프를 켜고, 맵에 있던 아이템 상자 오브젝트는 제거
            GameManager.instance.EarnWeatherItem(round);
            Destroy(collision.gameObject);
        }

        // 구출 대상 NPC와 충돌했을 때
        string objName = collision.gameObject.name;
        if (objName.Contains("Ninja") || objName.Contains("Mask") || objName.Contains("Virtual") || collision.CompareTag("NPC"))
        {
            Debug.Log($"NPC 구출 성공: {objName}");

            // SoundManager와 연동하여 구출 성공 사운드 팡 터트리기
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayRescue();
            }
        }
    }
}