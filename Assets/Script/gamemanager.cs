using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Progress Settings")]
    public string currentRoundName = "1ROUND";
    public float limitTime = 20f;

    [Header("UI Settings")]
    public TextMeshProUGUI timerText; // TMP 전용 컴포넌트

    [Header("Player Script Link")]
    public Player_pinkman player;

    private bool isGameOver = false;

    private float weatherTimer = 0f;
    private float penaltyDurationTimer = 0f;
    private float itemDurationTimer = 0f;

    public bool isItemActive = false;
    public bool isPenaltyActive = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentRoundName = SceneManager.GetActiveScene().name;
        isGameOver = false;
        limitTime = 20f;

        // 1라운드가 새로 시작될 때 이전 판의 잔상 데이터를 깨끗하게 청소
        if (currentRoundName == "1ROUND")
        {
            PlayerPrefs.SetFloat("Temp_Round1_LeftTime", -1f); // -1은 아직 도달 안 했다는 뜻
            PlayerPrefs.SetFloat("Temp_Round2_LeftTime", -1f);
            PlayerPrefs.SetFloat("Temp_Round3_LeftTime", -1f);
            PlayerPrefs.SetString("GameResult", "GAMEOVER"); // 기본값은 게임오버
            PlayerPrefs.Save();
        }
    }

    void Update()
    {
        if (isGameOver) return;

        limitTime -= Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = "TIME: " + limitTime.ToString("F1") + "s";
        }

        if (limitTime <= 0)
        {
            limitTime = 0;
            GameOver();
        }

        HandleWeatherSystem();
    }

    void HandleWeatherSystem()
    {
        if (player == null) return;

        // ==========================================
        // 1. 아이템 타이머 관리
        // ==========================================
        if (isItemActive)
        {
            itemDurationTimer -= Time.deltaTime;
            if (itemDurationTimer <= 0)
            {
                isItemActive = false;
                Debug.Log("Item effect expired");

                // 1라운드: 우산이 끝났으니 다시 미끄러지게 설정
                if (currentRoundName == "1ROUND")
                {
                    player.ApplySlipperyPhysics(true);
                }
                // 2라운드: 얼음 효과가 끝났을 때, 아직 폭염 페널티 기간 중이었다면 점프력을 깎음
                else if (currentRoundName == "2ROUND" && isPenaltyActive)
                {
                    player.ReduceJumpPower();
                }
                // 3라운드: 핫팩 효과가 끝났을 때, 하필 한파 페널티 기간(1초) 중이었다면 다시 속도를 저하시킴
                else if (currentRoundName == "3ROUND" && isPenaltyActive)
                {
                    player.FreezePlayer();
                    Debug.Log("Hot Pack expired during Cold Wave! Speed Reduced.");
                }
            }
        }

        // ==========================================
        // 2. 페널티 타이머 관리
        // ==========================================
        if (isPenaltyActive)
        {
            penaltyDurationTimer -= Time.deltaTime;
            if (penaltyDurationTimer <= 0)
            {
                isPenaltyActive = false;

                // 페널티 시간이 끝나면 각 라운드별 능력치 원상복구
                if (currentRoundName == "2ROUND") player.ResetJumpPower();
                if (currentRoundName == "3ROUND") player.UnfreezePlayer(); // ★ 원래 상태로 속도 증가(복구)
            }
        }

        // ==========================================
        // 3. 라운드별 상시 기후 기믹 제어
        // ==========================================
        if (currentRoundName == "1ROUND")
        {
            // 아이템이 없을 때만 미끄러운 물리 적용
            if (!isItemActive)
            {
                player.ApplySlipperyPhysics(true);
            }
        }
        else if (currentRoundName == "2ROUND")
        {
            weatherTimer += Time.deltaTime;
            if (weatherTimer >= 7f) // 7초마다 폭염 주의보 발동
            {
                weatherTimer = 0f;
                isPenaltyActive = true;
                penaltyDurationTimer = 2f; // 2초 동안 페널티 지속

                // 얼음 아이템이 없을 때만 실제로 점프력을 감소시킴
                if (!isItemActive)
                {
                    player.ReduceJumpPower();
                    Debug.Log("Warning: Heat Wave! Jump Power Reduced.");
                }
                else
                {
                    Debug.Log("Heat Wave Blocked by Ice Item!");
                }
            }
        }
        else if (currentRoundName == "3ROUND")
        {
            weatherTimer += Time.deltaTime;
            if (weatherTimer >= 6f) // 6초 주기로 한파 몰아침
            {
                weatherTimer = 0f;
                isPenaltyActive = true;
                penaltyDurationTimer = 1f; // ★ 변경: 1초 동안 페널티 지속

                // 핫팩 아이템이 없을 때만 실제로 속도를 저하시킴
                if (!isItemActive)
                {
                    player.FreezePlayer(); // 몸이 얼어붙어 속도 저하 발생
                    Debug.Log("Warning: Cold Wave! Speed Reduced.");
                }
                else
                {
                    Debug.Log("Cold Wave Blocked by Hot Pack!");
                }
            }
        }
    }

    public void EarnWeatherItem(string round)
    {
        isItemActive = true;

        if (round == "1ROUND")
        {
            itemDurationTimer = 3f; // 우산 지속시간 3초
            Debug.Log("Umbrella Item Active");

            if (player != null)
                player.ApplySlipperyPhysics(false); // 미끄러짐 해제
        }
        else if (round == "2ROUND")
        {
            itemDurationTimer = 6f; // 얼음 지속시간 6초
            Debug.Log("Ice Item Active - 6s Heat Protection");

            if (player != null)
            {
                player.ResetJumpPower();
            }
        }
        else if (round == "3ROUND")
        {
            itemDurationTimer = 5f; // 핫팩 지속시간 5초로 고정 및 초기화
            Debug.Log("Hot Pack Item Active - 5s Cold Protection");

            // 한파 페널티를 받아 이미 몸이 얼어 속도가 저하된 상태에서 먹더라도 즉시 원상태로 속도 증가(복구)
            if (player != null)
            {
                player.UnfreezePlayer();
            }
        }
    }

    public void ClearRound()
    {
        // 현재 라운드 이름에 맞춰 남은 시간을 정확하게 저장
        if (currentRoundName == "1ROUND") PlayerPrefs.SetFloat("Temp_Round1_LeftTime", limitTime);
        else if (currentRoundName == "2ROUND") PlayerPrefs.SetFloat("Temp_Round2_LeftTime", limitTime);
        else if (currentRoundName == "3ROUND") PlayerPrefs.SetFloat("Temp_Round3_LeftTime", limitTime);

        PlayerPrefs.Save(); // 데이터 저장

        if (currentRoundName == "1ROUND") SceneManager.LoadScene("2ROUND");
        else if (currentRoundName == "2ROUND") SceneManager.LoadScene("3ROUND");
        else if (currentRoundName == "3ROUND") SceneManager.LoadScene("ResultScene");
    }

    public void GameOver()
    {
        isGameOver = true;

        // 게임오버 된 현재 라운드는 남은 시간을 0으로 만듦
        if (currentRoundName == "1ROUND") PlayerPrefs.SetFloat("Temp_Round1_LeftTime", 0f);
        else if (currentRoundName == "2ROUND") PlayerPrefs.SetFloat("Temp_Round2_LeftTime", 0f);
        else if (currentRoundName == "3ROUND") PlayerPrefs.SetFloat("Temp_Round3_LeftTime", 0f);

        PlayerPrefs.SetString("GameResult", "GAMEOVER");
        PlayerPrefs.Save();

        SceneManager.LoadScene("ResultScene");
    }
}