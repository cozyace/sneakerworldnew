using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    public TMP_Text UsernameText;
    public TMP_Text CashText;
    public TMP_Text RankingText;
    public TMP_Text GoalText;
    public Image BackgroundPanel;

    public Sprite[] PanelSpriteResources;
    
    public void UpdatePosition(int pos)
    {
        RankingText.text = $"#{pos}";
        
        if (pos <= 3)
            BackgroundPanel.sprite = PanelSpriteResources[pos-1];
    }
    
    
}
