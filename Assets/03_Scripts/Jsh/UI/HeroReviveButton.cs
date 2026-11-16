using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroReviveButton : MonoBehaviour
{
    [SerializeField] private Image heroImage;
    [SerializeField] private TMP_Text heroNameText;

    private Button reviveButton;

    private void Awake()
    {
        reviveButton = GetComponent<Button>();
    }

    public void Init(string heroName, Sprite sprite, Action onClicked)
    {
        if (reviveButton == null)
            reviveButton = GetComponent<Button>();
        if (reviveButton == null)
            reviveButton = GetComponentInChildren<Button>();

        if (heroNameText == null)
            heroNameText = GetComponentInChildren<TMP_Text>();
        if (heroImage == null)
        {
            var images = GetComponentsInChildren<Image>();
            // Button 배경 Image 제외하고 찾기
            foreach (var img in images)
            {
                if (img.gameObject != gameObject)
                {
                    heroImage = img;
                    break;
                }
            }
        }

        if (heroNameText != null) heroNameText.text = heroName;

        if (heroImage != null)
        {
            if (sprite != null)
            {
                heroImage.sprite = sprite;
                heroImage.enabled = true;
            }
            else
            {
                heroImage.enabled = false;
            }
        }

        if (reviveButton != null)
        {
            reviveButton.onClick.RemoveAllListeners();
            reviveButton.onClick.AddListener(() => onClicked?.Invoke());
        }
        else
        {
            Debug.LogError("[HeroReviveButton] Button 컴포넌트 없음. 프리팹 구조 확인 필요.");
        }
    }
}

