using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenFilter : MonoBehaviour
{
    [SerializeField] private UIDocument fadeScreen;
    private VisualElement canvas;
    void Start()
    {
        VisualElement root = fadeScreen.rootVisualElement;
        canvas = root.Q("canvas");
    }
    public void TransitionColor(Vector4 targetColor, float duration)
    {
        Vector4 originalColor = new(
            canvas.resolvedStyle.backgroundColor.r,
            canvas.resolvedStyle.backgroundColor.g,
            canvas.resolvedStyle.backgroundColor.b,
            canvas.resolvedStyle.backgroundColor.a
        );
        float t = 0;
        DOTween.To(() => t, (x) =>
        {
            Vector4 color = x.Lerp(0, 1, originalColor, targetColor);
            canvas.style.backgroundColor = new Color(color.x, color.y, color.z, color.w);
        }, 1, duration);
    }
}