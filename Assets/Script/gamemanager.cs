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
    public string currentRoundName = "1ROUND"; // 현재 플레이 중인 라운드 이름
    public float limitTime = 25f;              // 라운드 제한 시간

    [Header("UI Settings")]
    public TextMeshProUGUI timerText;          // TMP UI

    [Header("Player Script Link")]
    public Player_pinkman player;              // 기믹을 적용할 플레이어

    private bool isGameOver = false;

    // 날씨 시스템 제어용 타이머 변수들
    private float weatherTimer = 0f;           // 주기적으로 재해를 일으킬 누적 타이머
    private float penaltyDurationTimer = 0f;   // 페널티(폭염/한파) 지속시간 타이머
    private float itemDurationTimer = 0f;      // 아이템(우산/얼음/핫팩) 지속시간 타이머

    public bool isItemActive = false;          // 아이템 버프 발동 여부
    public bool isPenaltyActive = false;       // 날씨 페널티가 발동 여부

    void Awake()
    {
        // 게임 전체에서 매니저 인스턴스를 단 하나만 유지하도록 보장
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 활성화된 유니티 씬의 이름을 자동으로 가져와 라운드로 지정
        currentRoundName = SceneManager.GetActiveScene().name;
        isGameOver = false;
        limitTime = 25f;

        // 1라운드 데이터 초기화
        if (currentRoundName == "1ROUND")
        {
            PlayerPrefs.SetFloat("Temp_Round1_LeftTime", -1f); // -1, 노클리어
            PlayerPrefs.SetFloat("Temp_Round2_LeftTime", -1f);
            PlayerPrefs.SetFloat("Temp_Round3_LeftTime", -1f);
            PlayerPrefs.SetString("GameResult", "GAMEOVER");   
            PlayerPrefs.Save();                               
        }
    }

    void Update()
    {
        if (isGameOver) return;

        // 제한 시간을 실시간으로 감소
        limitTime -= Time.deltaTime;

        // UI에 남은 시간 소수점 첫째 자리까지
        if (timerText != null)
        {
            timerText.text = "TIME: " + limitTime.ToString("F1") + "s";
        }

        // 제한 시간 오버 시, 게임오버 처리
        if (limitTime <= 0)
        {
            limitTime = 0;
            GameOver();
        }

        // 기후 변화 감지
        HandleWeatherSystem();
    }

    void HandleWeatherSystem()
    {
        if (player == null) return;

        // 아이템 타이머 관리
        if (isItemActive)
        {
            itemDurationTimer -= Time.deltaTime;
            if (itemDurationTimer <= 0) // 아이템 시간이 종료되면
            {
                isItemActive = false;
                Debug.Log("아이템 버프 효과가 종료되었습니다");

                // 아이템이 사라졌을 때, 현재 날씨 상태를 파악하여 플레이어 능력치를 즉시 재조정
                if (currentRoundName == "1ROUND")
                {
                    player.ApplySlipperyPhysics(true); // 우산 끊기면 다시 미끄러짐
                }
                else if (currentRoundName == "2ROUND" && isPenaltyActive)
                {
                    player.ReduceJumpPower();          // 얼음 깨졌는데 아직 폭염 중이면 점프력 저하
                }
                else if (currentRoundName == "3ROUND" && isPenaltyActive)
                {
                    player.FreezePlayer();             // 핫팩 꺼졌는데 아직 한파 중이면 속도 저하
                }
            }
        }


        // 페널티 타이머 관리 알고리즘
        if (isPenaltyActive)
        {
            penaltyDurationTimer -= Time.deltaTime;
            if (penaltyDurationTimer <= 0) // 자연재해 지속시간이 끝나면
            {
                isPenaltyActive = false;

                // 디버프 상태를 해제하고 플레이어의 스펙을 원래 정상 수치로 복구
                if (currentRoundName == "2ROUND") player.ResetJumpPower();
                if (currentRoundName == "3ROUND") player.UnfreezePlayer();
            }
        }

    
        // 라운드별 상시 기후 기믹 제어 알고리즘
        if (currentRoundName == "1ROUND")
        {
            // 1라운드: 비, 아이템 버프가 없을 때는 무조건 상시로 미끄러운 물리 재질 적용
            if (!isItemActive) player.ApplySlipperyPhysics(true);
        }
        else if (currentRoundName == "2ROUND")
        {
            // 2라운드: 폭염, 5초 주기로 2초 동안 폭염 주의보 발동 (점프력 감소)
            weatherTimer += Time.deltaTime;
            if (weatherTimer >= 5f)
            {
                weatherTimer = 0f;
                isPenaltyActive = true;
                penaltyDurationTimer = 2f;

                Debug.Log("기상 악화, 2초 동안 폭염 주의보 발동합니다.");

                // 얼음 아이템으로 방어 중이 아닐 때만 플레이어에게 하향 버프 부여
                if (!isItemActive) player.ReduceJumpPower();
                else Debug.Log("플레이어가 얼음 아이템 버프 상태이므로 폭염 페널티를 무시합니다.");
            }
        }
        else if (currentRoundName == "3ROUND")
        {
            // 3라운드: 한파, 5초 주기로 2초 동안 한파 몰아침 (이동 속도 저하)
            weatherTimer += Time.deltaTime;
            if (weatherTimer >= 5f)
            {
                weatherTimer = 0f;
                isPenaltyActive = true;
                penaltyDurationTimer = 2f;

                Debug.Log("기상 악화, 2초 동안 한파 경보 발동합니다.");

                // 핫팩 아이템으로 방어 중이 아닐 때만 속도 저하 적용
                if (!isItemActive) player.FreezePlayer();
                else Debug.Log("플레이어가 핫팩 아이템 버프 상태이므로 한파 페널티를 무시합니다.");
            }
        }
    }

    // 아이템 획득 시, 해당 아이템의 버프를 즉시 적용하는 함수
    public void EarnWeatherItem(string round)
    {
        isItemActive = true;

        if (round == "1ROUND")
        {
            itemDurationTimer = 3f; // 우산 방어 3초
            Debug.Log($"1ROUND 우산 획득, 3초 동안 빗길 미끄러짐 방지 버프가 발동됩니다."); 
            if (player != null) player.ApplySlipperyPhysics(false); // 미끄러짐 즉시 면제
        }
        else if (round == "2ROUND")
        {
            itemDurationTimer = 4f; // 얼음 방어 4초
            Debug.Log($"2ROUND 얼음 획득, 4초 동안 폭염 면역 및 점프력 저하 방지 버프가 발동됩니다.");
            if (player != null) player.ResetJumpPower();            // 깎였던 점프력 즉시 복구
        }
        else if (round == "3ROUND")
        {
            itemDurationTimer = 4f; // 핫팩 방어 4초
            Debug.Log($"3ROUND 핫팩 획득, 4초 동안 한파 면역 및 이동 속도 저하 방지 버프가 발동됩니다.");
            if (player != null) player.UnfreezePlayer();            // 얼었던 몸 즉시 녹임 (속도 복구)
        }
    }


    // NPC 구출 성공 및 스테이지 클리어 시 실행
    public void ClearRound()
    {
        // 기록 랭킹용
        if (currentRoundName == "1ROUND") PlayerPrefs.SetFloat("Temp_Round1_LeftTime", limitTime);
        else if (currentRoundName == "2ROUND") PlayerPrefs.SetFloat("Temp_Round2_LeftTime", limitTime);
        else if (currentRoundName == "3ROUND") PlayerPrefs.SetFloat("Temp_Round3_LeftTime", limitTime);

        PlayerPrefs.Save();

        // 순차적으로 다음 라운드 씬을 로드하고, 최종 라운드면 결과창으로 이동
        if (currentRoundName == "1ROUND") SceneManager.LoadScene("2ROUND");
        else if (currentRoundName == "2ROUND") SceneManager.LoadScene("3ROUND");
        else if (currentRoundName == "3ROUND") SceneManager.LoadScene("ResultScene");
    }


    // 제한 시간이 0이 되었을 때 호출
    public void GameOver()
    {
        isGameOver = true;

        // 패배한 라운드의 남은 시간은 전부 0초 처리를 하여 PlayerPrefs에 저장
        if (currentRoundName == "1ROUND") PlayerPrefs.SetFloat("Temp_Round1_LeftTime", 0f);
        else if (currentRoundName == "2ROUND") PlayerPrefs.SetFloat("Temp_Round2_LeftTime", 0f);
        else if (currentRoundName == "3ROUND") PlayerPrefs.SetFloat("Temp_Round3_LeftTime", 0f);

        PlayerPrefs.SetString("GameResult", "GAMEOVER"); // 결과창에 패배 도장을 찍기 위해 기록
        PlayerPrefs.Save();

        SceneManager.LoadScene("ResultScene"); // 즉시 결과창으로 화면 전환
    }
}