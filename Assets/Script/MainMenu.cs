using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //  씬 전환을 위해 반드시 필요한 네임스페이스!

public class MainMenu : MonoBehaviour
{
    //  시작 버튼을 눌렀을 때 호출할 함수 (반드시 public이어야 버튼에서 보입니다)
    public void StartGame()
    {
        Debug.Log("START GAME");
        // "GameScene" 자리에 이동하고 싶은 실제 게임 씬의 이름을 정확히 적어주세요!
        SceneManager.LoadScene("1ROUND");
    }

    // [선택 사항] END 버튼을 눌렀을 때 게임을 종료하는 함수
    public void QuitGame()
    {
        Debug.Log("QUIT GAME");

        // 1. 실제 빌드된 게임 프로그램 종료
        Application.Quit();

        // 2.  [추가] 유니티 에디터에서 테스트할 때 재생을 자동으로 꺼주는 코드!
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}