using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [Header("Current Game UI")]
    public TextMeshProUGUI resultTitleText; // GAME OVER 또는 CLEAR (화면 상단용 필요시)
    public TextMeshProUGUI r1TimeText;      // 1ROUND: 옆에 들어갈 글자
    public TextMeshProUGUI r2TimeText;      // 2ROUND: 옆에 들어갈 글자
    public TextMeshProUGUI r3TimeText;      // 3ROUND: 옆에 들어갈 글자
    public TextMeshProUGUI totalTimeText;   // SCORE: 옆에 들어갈 글자

    [Header("Top 3 Board UI")]
    public TextMeshProUGUI rank1Text; // 1ST: 기록용 텍스트
    public TextMeshProUGUI rank2Text; // 2ND: 기록용 텍스트
    public TextMeshProUGUI rank3Text; // 3RD: 기록용 텍스트

    void Start()
    {
        // 1. 이번 판 라운드별 데이터 읽어오기
        float r1Left = PlayerPrefs.GetFloat("Temp_Round1_LeftTime", 0f);
        float r2Left = PlayerPrefs.GetFloat("Temp_Round2_LeftTime", 0f);
        float r3Left = PlayerPrefs.GetFloat("Temp_Round3_LeftTime", 0f);
        string resultState = PlayerPrefs.GetString("GameResult", "GAMEOVER");

        if (resultTitleText != null) resultTitleText.text = resultState;

        // UI 텍스트 업데이트 (스크린샷 모양에 맞춤)
        if (r1TimeText != null) r1TimeText.text = "1ROUND: " + r1Left.ToString("F1") + "s";
        if (r2TimeText != null) r2TimeText.text = "2ROUND: " + r2Left.ToString("F1") + "s";
        if (r3TimeText != null) r3TimeText.text = "3ROUND: " + r3Left.ToString("F1") + "s";

        float totalLeftTime = r1Left + r2Left + r3Left;
        if (totalTimeText != null) totalTimeText.text = "SCORE: " + totalLeftTime.ToString("F1") + "s";

        // 2. 역대 탑 3 세션 데이터 읽어오기
        float rank1 = PlayerPrefs.GetFloat("SessionRank1", 0f);
        float rank2 = PlayerPrefs.GetFloat("SessionRank2", 0f);
        float rank3 = PlayerPrefs.GetFloat("SessionRank3", 0f);

        // 3. 게임을 클리어했을 때만 탑 3 랭킹 갱신 및 정렬
        if (resultState == "CLEAR")
        {
            List<float> scores = new List<float>();
            scores.Add(rank1);
            scores.Add(rank2);
            scores.Add(rank3);
            scores.Add(totalLeftTime);

            // 내림차순 정렬 (큰 숫자가 앞으로)
            scores.Sort((a, b) => b.CompareTo(a));

            rank1 = scores[0];
            rank2 = scores[1];
            rank3 = scores[2];

            PlayerPrefs.SetFloat("SessionRank1", rank1);
            PlayerPrefs.SetFloat("SessionRank2", rank2);
            PlayerPrefs.SetFloat("SessionRank3", rank3);
        }

        // 4. 탑 3 UI 텍스트에 값 뿌려주기
        if (rank1Text != null) rank1Text.text = "1ST: " + rank1.ToString("F1") + "s";
        if (rank2Text != null) rank2Text.text = "2ND: " + rank2.ToString("F1") + "s";
        if (rank3Text != null) rank3Text.text = "3RD: " + rank3.ToString("F1") + "s";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}