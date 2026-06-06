using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [Header("Current Game UI")]
    public TextMeshProUGUI r1TimeText;     //1라운드
    public TextMeshProUGUI r2TimeText;     // 2라운드 
    public TextMeshProUGUI r3TimeText;     // 3라운드 
    public TextMeshProUGUI totalTimeText;  // 최종 시간

    [Header("Top 3 Board UI")]
    public TextMeshProUGUI rank1Text;      // 1등
    public TextMeshProUGUI rank2Text;      // 2등 
    public TextMeshProUGUI rank3Text;      // 3등 

    private const float ROUND_LIMIT = 20f; // 제한 시간 

    void Start()
    {
        // 하이어라키 창의 오브젝트 이름으로 찾아 연결
        GameObject r1Obj = GameObject.Find("1ROUND");
        if (r1Obj != null) r1TimeText = r1Obj.GetComponent<TextMeshProUGUI>();

        GameObject r2Obj = GameObject.Find("2ROUND");
        if (r2Obj != null) r2TimeText = r2Obj.GetComponent<TextMeshProUGUI>();

        GameObject r3Obj = GameObject.Find("3ROUND");
        if (r3Obj != null) r3TimeText = r3Obj.GetComponent<TextMeshProUGUI>();

        GameObject totalObj = GameObject.Find("Total_Score");
        if (totalObj != null) totalTimeText = totalObj.GetComponent<TextMeshProUGUI>();

        GameObject rk1Obj = GameObject.Find("1ST");
        if (rk1Obj != null) rank1Text = rk1Obj.GetComponent<TextMeshProUGUI>();

        GameObject rk2Obj = GameObject.Find("2ND");
        if (rk2Obj != null) rank2Text = rk2Obj.GetComponent<TextMeshProUGUI>();

        GameObject rk3Obj = GameObject.Find("3RD");
        if (rk3Obj != null) rank3Text = rk3Obj.GetComponent<TextMeshProUGUI>();

        //  저장된 라운드별 남은 시간 데이터 로드
        float r1Left = PlayerPrefs.GetFloat("Temp_Round1_LeftTime", -1f);
        float r2Left = PlayerPrefs.GetFloat("Temp_Round2_LeftTime", -1f);
        float r3Left = PlayerPrefs.GetFloat("Temp_Round3_LeftTime", -1f);

        // 소모 시간 계산 알고리즘
        // 플레이어가 해당 라운드를 깨는 데 걸린 시간 계산
        // 탈락했거나 데이터가 없다면 최고 페널티 시간인 20초를 부여하고, 성공했다면 제한시간 - 남은시간
        float r1Used = (r1Left <= -1f || r1Left == 0f) ? ROUND_LIMIT : ROUND_LIMIT - r1Left;
        float r2Used = (r2Left <= -1f || r2Left == 0f) ? ROUND_LIMIT : ROUND_LIMIT - r2Left;
        float r3Used = (r3Left <= -1f || r3Left == 0f) ? ROUND_LIMIT : ROUND_LIMIT - r3Left;

        // 현재 판 점수 UI 업데이트
        if (r1TimeText != null) r1TimeText.text = "1ROUND: " + r1Used.ToString("F1") + "s";
        if (r2TimeText != null) r2TimeText.text = "2ROUND: " + r2Used.ToString("F1") + "s";
        if (r3TimeText != null) r3TimeText.text = "3ROUND: " + r3Used.ToString("F1") + "s";

        // 3개 라운드 소모 시간을 다 더해 총합 점수 계산
        float totalUsedTime = r1Used + r2Used + r3Used;
        if (totalTimeText != null) totalTimeText.text = "TOTAL SCORE: " + totalUsedTime.ToString("F1") + "s";

        // 타임어택 리더보드 정렬 알고리즘
        // 과거에 저장되어 있던 상위 3등 기록 불러오기 (없으면 0으로 초기화)
        float rank1 = PlayerPrefs.GetFloat("SessionRank1", 0f);
        float rank2 = PlayerPrefs.GetFloat("SessionRank2", 0f);
        float rank3 = PlayerPrefs.GetFloat("SessionRank3", 0f);

        List<float> scores = new List<float>();

        // 유효한 기존 기록들만 리스트에 임시로 수집
        if (rank1 > 0f) scores.Add(rank1);
        if (rank2 > 0f) scores.Add(rank2);
        if (rank3 > 0f) scores.Add(rank3);

        // 방금 플레이한 최종 점수를 리스트에 추가
        scores.Add(totalUsedTime);

        // 오름차순 정렬: 타임어택 게임이므로 시간이 적게 걸릴수록 최고 점수이므로 맨 앞으로 정렬
        scores.Sort();

        // 정렬된 리스트에서 가장 점수가 낮은(빠른) 상위 3개를 뽑아 순위 데이터 갱신
        rank1 = scores.Count > 0 ? scores[0] : 0f;
        rank2 = scores.Count > 1 ? scores[1] : 0f;
        rank3 = scores.Count > 2 ? scores[2] : 0f;

        // 새로운 랭킹을 하드디스크에 영구 저장
        PlayerPrefs.SetFloat("SessionRank1", rank1);
        PlayerPrefs.SetFloat("SessionRank2", rank2);
        PlayerPrefs.SetFloat("SessionRank3", rank3);
        PlayerPrefs.Save();

        // Top 3 UI 최종 업데이트
        if (rank1Text != null) rank1Text.text = "1ST: " + (rank1 > 0f ? rank1.ToString("F1") + "s" : "-");
        if (rank2Text != null) rank2Text.text = "2ND: " + (rank2 > 0f ? rank2.ToString("F1") + "s" : "-");
        if (rank3Text != null) rank3Text.text = "3RD: " + (rank3 > 0f ? rank3.ToString("F1") + "s" : "-");
    }

    // 리셋 첫 번째 씬으로 이동
    public void RestartGame()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButton(); // 버튼 소리 재생
        }

        SceneManager.LoadScene(0);
    }

    // 게임 종료 버튼 플레이 버전을 종료하거나 에디터 테스트를 중지
    public void QuitGame()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButton();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터 재생 중지
#else
        Application.Quit(); // 실제 게임 창 완전 종료
#endif
    }
}