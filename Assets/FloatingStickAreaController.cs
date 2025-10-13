using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

// Handle floating joystick area interactions and pass events to the joystick
namespace Assets.Scripts.Controller.Ui
{
    public class FloatingStickAreaController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        GameObject _joystick;

        OnScreenStick _screenStick;
        RectTransform _mainRect;
        RectTransform _joystickRect;

        Vector2 touchPosition;

        private void Awake()
        {
            _screenStick = _joystick.GetComponentInChildren<OnScreenStick>();
            _mainRect = GetComponent<RectTransform>();
            _joystickRect = _joystick.GetComponent<RectTransform>();

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (GameManager.instance == null || GameManager.instance.GetControlMode() != ControlMode.JOYSTICK_DYNAMIC || _joystick == null)
            {
                return;
            }
            // Pass the event to the joystick
            ExecuteEvents.dragHandler(_screenStick, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (GameManager.instance == null || GameManager.instance.GetControlMode() != ControlMode.JOYSTICK_DYNAMIC || _joystick == null)
            {
                return;
            }

            _joystick.SetActive(true);

            // Move joystick to touch position
            Vector2 localPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_mainRect, eventData.pressPosition, Camera.main, out localPosition);

            touchPosition = localPosition;

            _joystickRect.localPosition = touchPosition;

            ExecuteEvents.pointerDownHandler(_joystick.GetComponentInChildren<OnScreenStick>(), eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (GameManager.instance == null || GameManager.instance.GetControlMode() != ControlMode.JOYSTICK_DYNAMIC || _joystick == null)
            {
                return;
            }
            _joystick.SetActive(false);
            ExecuteEvents.pointerUpHandler(_screenStick, eventData);
        }
    }
}