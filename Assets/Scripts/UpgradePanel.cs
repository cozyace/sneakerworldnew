using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text description;
    [SerializeField] public Button buyButton;

    [Header("Values")]
    public int level;
    public int price;

    public void SetInitialValues(string startingDescription)
    {
        SetPrice(100);
        SetCurrentLevel(0);
        SetDescription(startingDescription);
    }

    public void BuyUpgrade()
    {
        level += 1;
        levelText.text = $"Level {level}";
        price = level * 200;
        priceText.text = $"{price}";
    }

    private void SetPrice(int value)
    {
        priceText.text = $"{value}";
        price = value;
    }

    private void SetCurrentLevel(int value)
    {
        levelText.text = $"Level {value}";
        level = value;
    }

    public void SetDescription(string value)
    {
        description.text = value;
    }

    public void SetMaxedOut()
    {
        priceText.text = "MAXED OUT";
    }
}
