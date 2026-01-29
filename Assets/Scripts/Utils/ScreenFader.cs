using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private UIDocument fadeScreen;
    private VisualElement canvas;
    void Start()
    {
        VisualElement root = fadeScreen.rootVisualElement;
        canvas = root.Q("canvas");
    }
    public void FadeIn(float duration)
    {
        float t = 1;
        DOTween.To(() => t, (x) =>
        {
            canvas.style.backgroundColor = new Color(0, 0, 0, x);
        }, 0, duration);
    }
    public void FadeOut(float duration)
    {
        float t = 0;
        DOTween.To(() => t, (x) =>
        {
            canvas.style.backgroundColor = new Color(0, 0, 0, x);
        }, 1, duration);
    }
}