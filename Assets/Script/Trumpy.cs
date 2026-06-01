using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trumpy : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 태그 검사 조건을 아예 지워버렸습니다! 
        // 이제 핑크맨이든 장애물이든 뭐든 닿기만 하면 발동합니다.
        if (GameManager.instance != null)
        {
            GameManager.instance.ClearRound();
        }

        Destroy(gameObject);
    }
}