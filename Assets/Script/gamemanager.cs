using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Setting")]
    public TextMeshProUGUI timeText;

    [Header("Time Setting")]
    public float surviveTime = 0f;
    public float limitTime = 20f;  // 현재 라운드의 남은 시간

    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        // 1라운드(Index 0) 시작 시 이번 판의 임시 기록방 초기화
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0)
        {
            PlayerPrefs.SetFloat("Temp_Round1_LeftTime", 0f);
            PlayerPrefs.SetFloat("Temp_Round2_LeftTime", 0f);
            PlayerPrefs.SetFloat("Temp_Round3_LeftTime", 0f);
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            surviveTime += Time.deltaTime;
            limitTime -= Time.deltaTime;

            if (limitTime <= 0f)
            {
                limitTime = 0f;
                GameOver();
            }

            UpdateTimeUI();
        }
    }

    void UpdateTimeUI()
    {
        if (timeText != null)
        {
            timeText.text = "Time: " + limitTime.ToString("F1") + "s";
        }
    }

    public void ClearRound()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 깨고 남은 시간을 임시 저장방에 기록합니다.
        if (currentSceneIndex == 0) PlayerPrefs.SetFloat("Temp_Round1_LeftTime", limitTime);
        else if (currentSceneIndex == 1) PlayerPrefs.SetFloat("Temp_Round2_LeftTime", limitTime);
        else if (currentSceneIndex == 2) PlayerPrefs.SetFloat("Temp_Round3_LeftTime", limitTime);

        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < 3)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // 3라운드까지 성공적으로 다 깨면 CLEAR 상태로 이동
            PlayerPrefs.SetString("GameResult", "CLEAR");
            SceneManager.LoadScene("ResultScene");
        }
    }

    void GameOver()
    {
        isGameOver = true;

        // 도중에 죽으면 남은 시간이 0초이므로 0으로 저장
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0) PlayerPrefs.SetFloat("Temp_Round1_LeftTime", 0f);
        else if (currentSceneIndex == 1) PlayerPrefs.SetFloat("Temp_Round2_LeftTime", 0f);
        else if (currentSceneIndex == 2) PlayerPrefs.SetFloat("Temp_Round3_LeftTime", 0f);

        PlayerPrefs.SetString("GameResult", "GAMEOVER");
        SceneManager.LoadScene("ResultScene");
    }
}