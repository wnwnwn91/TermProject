using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("START GAME");
        SceneManager.LoadScene("1ROUND");
    }

    public void ShowResult()
    {
        Debug.Log("SHOW RESULT SCENE");
        SceneManager.LoadScene("ResultScene");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}