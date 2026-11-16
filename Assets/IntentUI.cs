using UnityEngine;
using UnityEngine.UI;

public class IntentUI : MonoBehaviour
{
    private Image icon;

    private void Awake()
    {
        icon = GetComponent<Image>();
    }

    public void SetIcon(Sprite sprite, float alpha)
    {
        icon.sprite = sprite;
        SetAlpha(alpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = icon.color;
        color.a = Mathf.Clamp01(alpha);
        icon.color = color;
    }
}
