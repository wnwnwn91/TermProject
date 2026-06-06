using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Round Settings")]
    public string roundName = "1ROUND"; // 이 아이템이 어떤 라운드용 아이템인지 지정 

    // 한 프레임에 여러 번 부딪혀서 버그가 나는 것을 막아주는 방어막 알고리즘
    private bool isCollected = false;

    // 플레이어와 아이템 박스의 트리거 순간 실행되는 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 이미 획득 처리가 진행 중인 아이템이라면 아래 코드를 실행하지 않고 즉시 차단
        if (isCollected) return;

        // 충돌한 대상이 Player 태그를 가진 진짜 플레이어인지 검사
        if (collision.CompareTag("Player"))
        {
            isCollected = true; // 대상을 확인하자마자 획득 상태로 전환 

            // 라운드별 맞춤 사운드 재생 알고리즘, 효과음
            if (SoundManager.instance != null)
            {
                if (roundName == "1ROUND") SoundManager.instance.PlayUmbrella();  
                else if (roundName == "2ROUND") SoundManager.instance.PlayIce();  
                else if (roundName == "3ROUND") SoundManager.instance.PlayHotPack(); 
            }

            // GameManager에 아이템 버프 신호 전달
            if (GameManager.instance != null)
            {
                GameManager.instance.EarnWeatherItem(roundName); // 게임 매니저에게 기믹 방어막을 켜라고 명령
            }

            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite != null) sprite.enabled = false; // 아이템 이미지 숨기기

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;       // 비활성화하여 추가 충돌 방지

            // 오브젝트 2초뒤 완전 파괴
            Destroy(gameObject, 2.0f);
        }
    }
}