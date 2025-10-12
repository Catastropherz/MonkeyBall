using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        GameManager.instance.LoadLevel(1);
    }

    // Load next level
    public void NextLevel()
    {
        GameManager.instance.NextLevel();
    }

    // Pause game
    public void Pause()
    {
        GameManager.instance.Pause();
    }

    // Resume game
    public void Resume()
    {
        GameManager.instance.Resume();
    }

    // Restart level
    public void RestartLevel()
    {
        GameManager.instance.RestartLevel();
    }

    //Back to main menu
    public void BackToMainMenu()
    {
        GameManager.instance.BackToMainMenu();
    }
}
