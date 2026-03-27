using UnityEngine;

public class CursorUI : MonoBehaviour
{
    private RectTransform _cursorTransform;
    private Canvas _parentCanvas;
    private RectTransform _canvasRectTransform;
    private Camera _canvasCamera;

    private void Awake()
    {
        _cursorTransform = GetComponent<RectTransform>();
        _parentCanvas = GetComponentInParent<Canvas>();

        if (_parentCanvas != null)
        {
            _canvasRectTransform = _parentCanvas.GetComponent<RectTransform>();
            _canvasCamera = _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : _parentCanvas.worldCamera;
        }
    }

    private void Update()
    {
        if (_cursorTransform == null || _canvasRectTransform == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform,
            CustomInput.Position,
            _canvasCamera,
            out var localPoint))
        {
            _cursorTransform.anchoredPosition = localPoint;
        }
    }
}