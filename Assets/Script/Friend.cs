using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friend : MonoBehaviour
{

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