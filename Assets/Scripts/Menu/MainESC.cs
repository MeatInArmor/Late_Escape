using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainESC: MonoBehaviour
{
    public static bool InOptionsMenu;

    public GameObject MainMenuUI;

    public GameObject OptionsMenuUI;

    public GameObject OptionsBackground;
    public GameObject MainBackground;

    // Start is called before the first frame update
    void Start()
    {
        InOptionsMenu = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (InOptionsMenu)
            {
                BackToMenu();
            }
        }
    }

    public void BackToMenu()
    {
        OptionsMenuUI.SetActive(false);
        MainMenuUI.SetActive(true);
        OptionsBackground.SetActive(false);
        MainBackground.SetActive(true);
        InOptionsMenu = false;
    }

    public void GoToOptionsMenu()
    {
        InOptionsMenu = true;
    }

}
