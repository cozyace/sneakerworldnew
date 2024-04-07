using UnityEngine;
using TMPro;

public class NumberButton : MonoBehaviour
{
    public TMP_Text displayText;
    private int currentIndex = 0;
    private int[] values = { 1, 10, 100, 1000, int.MaxValue };

    private void Start()
    {
        // Initialize the display text with the initial value.
        UpdateDisplay();
    }

    public void OnButtonClick()
    {
        // Increment the index and update the display text.
        currentIndex = (currentIndex + 1) % values.Length;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (currentIndex == values.Length - 1)
        {
            displayText.text = "MAX";
        }
        else
        {
            displayText.text = values[currentIndex].ToString();
        }
    }
}
