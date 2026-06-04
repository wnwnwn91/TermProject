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

    private const float ROUND_LIMIT = 20f; // 각 라운드 제한 시간

    void Start()
    {
        // 1. 이번 판 라운드별 남은 시간 데이터 읽어오기 (기본값은 안 거쳤다는 의미의 -1)
        float r1Left = PlayerPrefs.GetFloat("Temp_Round1_LeftTime", -1f);
        float r2Left = PlayerPrefs.GetFloat("Temp_Round2_LeftTime", -1f);
        float r3Left = PlayerPrefs.GetFloat("Temp_Round3_LeftTime", -1f);
        string resultState = PlayerPrefs.GetString("GameResult", "GAMEOVER");

        if (resultTitleText != null) resultTitleText.text = resultState;

        // 2. 각 라운드별 "걸린 시간" 계산 (만약 플레이 안 했거나 시간 초과면 20초 다 쓴 걸로 처리)
        float r1Used = (r1Left <= -1f || r1Left == 0f) ? ROUND_LIMIT : ROUND_LIMIT - r1Left;
        float r2Used = (r2Left <= -1f || r2Left == 0f) ? ROUND_LIMIT : ROUND_LIMIT - r2Left;
        float r3Used = (r3Left <= -1f || r3Left == 0f) ? ROUND_LIMIT : ROUND_LIMIT - r3Left;

        // 3. UI 텍스트 업데이트
        if (r1TimeText != null) r1TimeText.text = "1ROUND: " + r1Used.ToString("F1") + "s";
        if (r2TimeText != null) r2TimeText.text = "2ROUND: " + r2Used.ToString("F1") + "s";
        if (r3TimeText != null) r3TimeText.text = "3ROUND: " + r3Used.ToString("F1") + "s";

        float totalUsedTime = r1Used + r2Used + r3Used;
        if (totalTimeText != null) totalTimeText.text = "SCORE: " + totalUsedTime.ToString("F1") + "s";

        // 4. 역대 탑 3 세션 데이터 읽어오기
        float rank1 = PlayerPrefs.GetFloat("SessionRank1", 0f);
        float rank2 = PlayerPrefs.GetFloat("SessionRank2", 0f);
        float rank3 = PlayerPrefs.GetFloat("SessionRank3", 0f);

        // 5. 3라운드까지 완벽하게 "CLEAR" 했을 때만 탑 3 랭킹 갱신
        if (resultState == "CLEAR")
        {
            List<float> scores = new List<float>();

            if (rank1 > 0f) scores.Add(rank1);
            if (rank2 > 0f) scores.Add(rank2);
            if (rank3 > 0f) scores.Add(rank3);

            scores.Add(totalUsedTime);
            scores.Sort((a, b) => a.CompareTo(b)); // 오름차순 정렬

            rank1 = scores.Count > 0 ? scores[0] : 0f;
            rank2 = scores.Count > 1 ? scores[1] : 0f;
            rank3 = scores.Count > 2 ? scores[2] : 0f;

            PlayerPrefs.SetFloat("SessionRank1", rank1);
            PlayerPrefs.SetFloat("SessionRank2", rank2);
            PlayerPrefs.SetFloat("SessionRank3", rank3);
            PlayerPrefs.Save();
        }

        // 6. 탑 3 UI 텍스트 출력
        if (rank1Text != null) rank1Text.text = "1ST: " + (rank1 > 0f ? rank1.ToString("F1") + "s" : "-");
        if (rank2Text != null) rank2Text.text = "2ND: " + (rank2 > 0f ? rank2.ToString("F1") + "s" : "-");
        if (rank3Text != null) rank3Text.text = "3RD: " + (rank3 > 0f ? rank3.ToString("F1") + "s" : "-");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    // [추가] 게임 종료를 담당하는 함수
    public void QuitGame()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 테스트 중일 때 켜져있는 재생(Play) 모드를 끕니다.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 게임 빌드 파일(.exe)에서는 게임 프로그램 자체를 완전히 종료합니다.
        Application.Quit();
#endif
    }
}
