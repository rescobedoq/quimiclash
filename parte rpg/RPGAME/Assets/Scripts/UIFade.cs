using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public static UIFade instance;
    public float fadeSpeed = 1f;
    public Image fadeScreen;
    public bool shouldFadeToBlack;
    public bool shouldFadeFromBlack;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Reiniciar alpha a 0
        if (fadeScreen != null)
        {
            Color c = fadeScreen.color;
            c.a = 0f;
            fadeScreen.color = c;
        }
    }


    void Update()
    {
        if (shouldFadeToBlack)
        {
            fadeScreen.color = new Color(
                fadeScreen.color.r,
                fadeScreen.color.g,
                fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime)
            );

            if (fadeScreen.color.a >= 1f)
                shouldFadeToBlack = false;
        }

        if (shouldFadeFromBlack)
        {
            fadeScreen.color = new Color(
                fadeScreen.color.r,
                fadeScreen.color.g,
                fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime)
            );

            if (fadeScreen.color.a <= 0f)
                shouldFadeFromBlack = false;
        }
    }

    public void FadeToBlack()
    {
        Debug.Log("🕶️ Fading to black");
        shouldFadeToBlack = true;
        shouldFadeFromBlack = false;
    }

    public void FadeFromBlack()
    {
        Debug.Log("🌅 Fading from black");
        shouldFadeFromBlack = true;
        shouldFadeToBlack = false;

        // Por seguridad, si el alpha estaba alto lo bajamos manualmente
        if (fadeScreen.color.a >= 1f)
        {
            fadeScreen.color = new Color(
                fadeScreen.color.r,
                fadeScreen.color.g,
                fadeScreen.color.b,
                fadeScreen.color.a
            );
        }
    }
}
