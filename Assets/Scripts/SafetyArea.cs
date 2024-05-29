using UnityEngine;

public class SafetyArea : MonoBehaviour
{
    public Canvas canvas;
    RectTransform panelRectTransform;
    Vector2 canvasSize;
    private void Start()
    {
        ApplySafeArea();
    }
    private void ApplySafeArea()
    {
        panelRectTransform = GetComponent<RectTransform>();
        float screenHeight = Screen.height;
        canvasSize = canvas.GetComponent<RectTransform>().rect.size;
        Rect safeArea = Screen.safeArea;
        Vector2 objectSize = panelRectTransform.rect.size;
        float topMargin = screenHeight - safeArea.yMax;
        float bottomMargin = safeArea.yMin;
        float margin = topMargin + objectSize.y / 2;
        panelRectTransform.anchoredPosition = new Vector2(safeArea.x, - margin);

    }
}
