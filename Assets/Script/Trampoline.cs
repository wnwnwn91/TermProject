using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("팅겨나가는 힘")]
    public float bouncePower = 15f; // 일반 점프보다 조금 더 높게 설정

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    //  Is Trigger가 체크된 센서 콜라이더에 플레이어가 닿았을 때 실행
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRigid = collision.GetComponent<Rigidbody2D>();
            Player_pinkman playerScript = collision.GetComponent<Player_pinkman>();

            if (playerRigid != null)
            {
                // 1. 플레이어의 기존 Y축 속도를 0으로 초기화 (낙하 속도 씹기)
                playerRigid.linearVelocity = new Vector2(playerRigid.linearVelocity.x, 0);

                // 2. 위쪽으로 강한 힘을 주어 튕겨내기
                playerRigid.AddForce(Vector2.up * bouncePower, ForceMode2D.Impulse);

                // 3. (선택사항) 이단점프 아이템을 먹었을 수 있으니 점프 횟수 초기화 해 주기
                // playerScript.jumpCount = 0; 

                // 4. 트램펄린 점프 애니메이션 발동!
                if (anim != null)
                {
                    // 애니메이션 파라미터에 Trigger 타입으로 "isJump"나 "Bounce"를 만들어주세요.
                    anim.SetTrigger("Bounce");
                }

                Debug.Log("트램펄린 점프 발동!");
            }
        }
    }
}