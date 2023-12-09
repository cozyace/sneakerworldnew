using UnityEngine;
using UnityEngine.UI;

public class ButtonFader : MonoBehaviour
{
    public Button button;
    public float fadeDuration = 1.0f;

    private Color initialColor;
    private float timer;

    private void Start()
    {
        initialColor = button.image.color;
        initialColor.a = 0.0f; // Set the initial alpha to 0
        button.image.color = initialColor; // Apply the initial color
    }

    private void Update()
    {
        if (button.image.color.a < 1.0f)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);

            Color targetColor = initialColor;
            targetColor.a = 1.0f; // Set the target alpha to 1

            button.image.color = Color.Lerp(initialColor, targetColor, t);

            if (t >= 1.0f)
            {
                initialColor = button.image.color;
                timer = 0.0f;
            }
        }
    }
}