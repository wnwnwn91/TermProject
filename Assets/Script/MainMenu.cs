using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //버튼 소리를 재생하고 1라운드 게임 화면으로 이동
    public void StartGame()
    {
        Debug.Log("게임 시작");

        // 씬이 완전히 넘어가기 전에 사운드 매니저에게 버튼 클릭 소리 재생 명령 전달
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButton();
        }

        // 1ROUND라는 이름을 가진 유니티 씬을 화면에 로드
        SceneManager.LoadScene("1ROUND");
    }

    // 누르면 버튼 소리를 재생하고 결과창 화면으로 이동
    public void ShowResult()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButton();
        }

        // ResultScene이라는 이름을 가진 유니티 씬을 화면에 로드
        SceneManager.LoadScene("ResultScene");
    }

    // 버튼 소리를 재생하고 게임을 완전히 종료
    public void QuitGame()
    {
        Debug.Log("QUIT GAME");

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButton();
        }

        // 종료 명령어
        Application.Quit();

        // 전처리 알고리즘, 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}