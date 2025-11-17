using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

// Handle UI interactions and call GameManager functions
public class MainMenu : MonoBehaviour
{
    //public GameObject GirlText;
    public TextMeshProUGUI GirlTextMesh;

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
        GameManager.instance.ShowRewardedAd(2);
    }

    //Select level 3
    public void SelectLevel3()
    {
        GameManager.instance.ShowRewardedAd(3);
    }

    //Select level 4
    public void SelectLevel4()
    {
        GameManager.instance.ShowRewardedAd(4);
    }

    //Select Level 5
    public void SelectLevel5()
    {
        GameManager.instance.ShowRewardedAd(5);
    }

    //Select Shop
    public void SelectShop()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.EnterShop();
        }

        if (GameManager.instance != null && GameManager.instance.IsGirlUnlocked())
        {
            //GirlText.GetComponent<TextMeshPro>().text = "Girl";
            GirlTextMesh.text = "Girl";
            // print debug message
            Debug.Log("Girl unlocked.");
        }
        else if (GameManager.instance != null && !GameManager.instance.IsGirlUnlocked())
        {
            //GirlText.GetComponent<TextMeshPro>().text = "$0.99";
            GirlTextMesh.text = "$0.99";
            Debug.Log("Girl locked.");
        }
        else
        {
            Debug.LogError("GameManager instance is null.");
        }
    }

    public void SelectAchievementPanel()
    {
        GameManager.instance.EnterAchievementPanel();
    }

    public void SelectGirlSkin()
    {
        GameManager.instance.SelectGirlSkin();
    }

    public void SelectBoySkin()
    {
        GameManager.instance.SelectBoySkin();
    }

    // Change control to joystick fixed
    public void JoystickFixed()
    {
        GameManager.instance.ChangeControlMode(0);
    }

    // Change control to joystick dynamic
    public void JoystickDynamic()
    {
        GameManager.instance.ChangeControlMode(1);
    }

    // Change control to gyroscope
    public void Gyroscope()
    {
        GameManager.instance.ChangeControlMode(2);
    }

    // Reset gyroscope
    public void ResetGyroscope()
    {
        GameManager.instance.ResetGyroscope();
    }
}
