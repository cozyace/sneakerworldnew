using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultValues : MonoBehaviour
{
    [Header ("Auth Scene")]
    [Header("Login References")]
    public GameObject loginUI;
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;
    public TMP_Text loginOutputText;
    public Button loginButton;

    [Header("Register References")]
    public GameObject registerUI;
    public TMP_InputField registerUsername;
    public TMP_InputField registerEmail;
    public TMP_InputField registerPassword;
    public TMP_InputField registerConfirmPassword;
    public TMP_Text registerOutputText;
    public Button registerButton;

    [Header("Main Scene")]
    [Header("Leaderboards")]
    public GameObject leaderboardPanel;
    public GameObject leaderboardListing;
    public RectTransform listingTransform;
}
