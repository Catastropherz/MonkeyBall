using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class MainMenu : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    //// Panel references
    //private GameObject joystick;

    //OnScreenStick _screenStick;
    //RectTransform _mainRect;
    //RectTransform _joystickRect;

    //private void Start()
    //{
    //    // references to UI objects in the newly loaded scene
    //    joystick = GameObject.FindWithTag("JoyStick");

    //    if (joystick != null)
    //    {
    //        _screenStick = joystick.GetComponentInChildren<OnScreenStick>();
    //        _mainRect = GetComponent<RectTransform>();
    //        _joystickRect = joystick.GetComponent<RectTransform>();
    //        // Hide joystick for dynamic mode
    //        if (GameManager.instance != null && GameManager.instance.GetControlMode() == ControlMode.JOYSTICK_DYNAMIC)
    //        {
    //            joystick.SetActive(false);
    //        }
    //    }
    //}

    //// Dynamic Joystick
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (GameManager.instance == null || GameManager.instance.GetControlMode() != ControlMode.JOYSTICK_DYNAMIC || joystick == null)
    //    {
    //        return;
    //    }
    //    // Move joystick to touch position
    //    Vector2 localPosition;

    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(_mainRect, eventData.pressPosition, Camera.main, out localPosition);

    //    _joystickRect.localPosition = localPosition;

    //    ExecuteEvents.pointerDownHandler(joystick.GetComponentInChildren<OnScreenStick>(), eventData);
    //}
    //public void OnDrag(PointerEventData eventData)
    //{
    //    if (GameManager.instance == null || GameManager.instance.GetControlMode() != ControlMode.JOYSTICK_DYNAMIC || joystick == null)
    //    {
    //        return;
    //    }
    //    ExecuteEvents.dragHandler(_screenStick, eventData);
    //}
    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    if (GameManager.instance == null || GameManager.instance.GetControlMode() != ControlMode.JOYSTICK_DYNAMIC || joystick == null)
    //    {
    //        return;
    //    }
    //    ExecuteEvents.pointerUpHandler(_screenStick, eventData);
    //}

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

    public void ResetGyroscope()
    {
        GameManager.instance.ResetGyroscope();
    }
}
