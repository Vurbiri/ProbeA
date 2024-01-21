using UnityEngine;
using UnityEngine.UI;

public class ScreenStick : MonoBehaviour
{
    [SerializeField] private RectTransform _rectParent;
    [SerializeField] private RectTransform _rectDial;
    [SerializeField] private RectTransform _rectArrow;
    [SerializeField] private float _startArrowFill = 0.62f;

    private PlayerController _controller;
    
    private Image _imageArrow;
    private float _lengthArrow;

    private void Start()
    {
        _controller = PlayerController.InstanceF;
        _imageArrow = _rectArrow.gameObject.GetComponent<Image>();
        _lengthArrow = 1f - _startArrowFill;

        _rectDial.gameObject.SetActive(false);

        _controller.EventPressStarted += OnPressStarted;
        _controller.EventPressCanceled += OnPressCanceled;
        _controller.EventRotation += OnRotation;
    }

    private void OnPressStarted(Vector2 position)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectParent, position, Camera.main, out Vector2 localPoint);
        _rectDial.anchoredPosition = localPoint;
        _rectDial.gameObject.SetActive(true);
        _imageArrow.fillAmount = _startArrowFill;
    }

    private void OnPressCanceled() => _rectDial.gameObject.SetActive(false);

    private void OnRotation(Vector2 rot, float magnitude)
    {
        rot = rot.normalized;
        float angle = Vector3.Angle(Vector2.up, rot);
        float sign = Mathf.Sign(Vector3.Dot(Vector2.left, rot));

        _rectArrow.eulerAngles = new(90f, 0f, angle * sign);
        _imageArrow.fillAmount = _startArrowFill + magnitude * _lengthArrow;
    }
    private void OnDestroy()
    {
        if (PlayerController.Instance == null)
            return;
        _controller.EventPressStarted -= OnPressStarted;
        _controller.EventPressCanceled -= OnPressCanceled;
        _controller.EventRotation -= OnRotation;
    }
}
