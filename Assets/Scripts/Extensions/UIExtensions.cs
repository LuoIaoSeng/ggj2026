using UnityEngine.UIElements;

public static class UIExtensions
{
    public static void SetDisplay(this VisualElement visualElement, bool display)
    {
        visualElement.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
    }
}