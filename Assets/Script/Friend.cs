using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friend : MonoBehaviour
{
    public Sprite ninjaFrogSprite;  // 1라운드용 개구리 이미지
    public Sprite maskDudeSprite;   // 2라운드용 가면 가이 이미지
    public Sprite virtualGuySprite; // 3라운드용 가상 가이 이미지

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // 내 오브젝트에 붙어있는 SpriteRenderer 컴포넌트를 가져옵니다.
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // GameManager에 등록된 현재 라운드 이름을 확인하고 이미지를 바꿉니다.
        if (GameManager.instance != null && spriteRenderer != null)
        {
            string currentRound = GameManager.instance.currentRoundName;

            if (currentRound == "1ROUND")
            {
                if (ninjaFrogSprite != null) spriteRenderer.sprite = ninjaFrogSprite;
            }
            else if (currentRound == "2ROUND")
            {
                if (maskDudeSprite != null) spriteRenderer.sprite = maskDudeSprite;
            }
            else if (currentRound == "3ROUND")
            {
                if (virtualGuySprite != null) spriteRenderer.sprite = virtualGuySprite;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 부딪히면 다음 라운드로 전환
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.ClearRound();
            }

            Destroy(gameObject);
        }
    }
}