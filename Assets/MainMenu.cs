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

    //Select level 1
    public void SelectLevel1()
    {
        GameManager.instance.LoadLevel(1);
    }

    //Select level 2
    public void SelectLevel2()
    {
        GameManager.instance.LoadLevel(2);
    }

    //Select level 3
    public void SelectLevel3()
    {
        GameManager.instance.LoadLevel(3);
    }

    //Select level 4
    public void SelectLevel4()
    {
        GameManager.instance.LoadLevel(4);
    }

    //Select Level 5
    public void SelectLevel5()
    {
        GameManager.instance.LoadLevel(5);
    }
}
