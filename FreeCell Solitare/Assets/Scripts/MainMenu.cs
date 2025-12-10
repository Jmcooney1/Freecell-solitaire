
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Background targets (choose one)")]
    public SpriteRenderer backgroundRenderer;
    public Image backgroundImage;

    [Header("Theme sprites")]
    public List<Sprite> themeSprites = new List<Sprite>();

    public void StartGame()
    {
        SceneManager.LoadScene("FreeCell Soltatire", LoadSceneMode.Additive);
    }

    public void SetTheme(int index)
    {
        if (themeSprites == null || themeSprites.Count == 0)
        {
            Debug.LogWarning("SetTheme: no theme sprites assigned in inspector");
            return;
        }

        if (index < 0 || index >= themeSprites.Count)
        {
            Debug.LogWarning($"SetTheme: index {index} out of range (0..{themeSprites.Count - 1})");
            return;
        }

        Sprite s = themeSprites[index];

        if (backgroundImage != null)
        {
            backgroundImage.sprite = s;
            return;
        }

        if (backgroundRenderer != null)
        {
            backgroundRenderer.sprite = s;
            return;
        }

        // Fallback: try to find a GameObject tagged "Background" in any loaded scene
        GameObject bg = GameObject.FindWithTag("Background");
        if (bg != null)
        {
            var sr = bg.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = s;
                return;
            }

            var img = bg.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = s;
                return;
            }

            Debug.LogWarning("SetTheme: 'Background' GameObject found, but it has no Image or SpriteRenderer component.");
            return;
        }

        Debug.LogWarning("SetTheme: no background target assigned and no GameObject with tag 'Background' found.");
    }

    // convenience wrappers for existing UI buttons
    public void ThemeSelectOne() => SetTheme(0);
    public void ThemeSelectTwo() => SetTheme(1);

}
