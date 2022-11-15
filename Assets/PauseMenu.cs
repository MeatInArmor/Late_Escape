using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused;

    public GameObject PauseMenuUI;

    public GameObject PauseButtonUI;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    //public Camera Cam;

    void Start()
    {
        GameIsPaused = false;
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        cursorInputForLook = true;
    }


    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        PauseButtonUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
        cursorInputForLook = true;
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        PauseButtonUI.SetActive(false);

        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Игра закрылась");
        Application.Quit();
    }


}