using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool InOptionsMenu;
    public static bool GameIsPaused;

    public GameObject PauseMenuUI;
    public GameObject OptionsMenuUI;

    public GameObject PauseButtonUI;
    //public GameObject PlayerButtonsUI;

    public GameObject OptionsBackground;

    public bool cursorInputForLook;

    //public Camera Cam;

    void Start()
    {
        GameIsPaused = false;
        InOptionsMenu = false;
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.BackQuote) && InOptionsMenu == false)
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
        else if (Input.GetKeyDown(KeyCode.BackQuote)) BackToMenu();
    }


    public void Resume()
    {
        Time.timeScale = 1f;
        PauseMenuUI.SetActive(false);
        PauseButtonUI.SetActive(true);
        OptionsBackground.SetActive(false);
        GameIsPaused = false;
        Cursor.visible = false;

    }

    public void Pause()
    {
        Time.timeScale = 0f;
        PauseMenuUI.SetActive(true);
        PauseButtonUI.SetActive(false);
        GameIsPaused = true;
        Cursor.visible = true;

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

    public void BackToMenu()
    {
        OptionsMenuUI.SetActive(false);
        PauseMenuUI.SetActive(true);
        OptionsBackground.SetActive(false);
        Time.timeScale = 0f;
        InOptionsMenu = false;
    }

    public void GoToOptionsMenu()
    {
        InOptionsMenu = true;
        OptionsBackground.SetActive(true);
    }

}