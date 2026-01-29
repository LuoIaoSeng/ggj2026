using UnityEngine;

public class Level01 : MonoBehaviour
{
    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private float fadeDuration = 1;
    void Start()
    {
        screenFader.FadeIn(fadeDuration);
    }
}
