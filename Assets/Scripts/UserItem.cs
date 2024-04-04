using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UserItem : MonoBehaviour
{
    public TMP_Text UsernameText;
    public TMP_Text CashText;
    public TMP_Text LevelText;
    public Image IconImage;
    public Image ActivityCircle;
    
    public GameManager GameManager;
    public FirebaseManager FirebaseManager;
    
    public GameObject addFriendButton, cancelFriendRequestButton, acceptFriendRequestButton, declineFriendRequestButton, tradeButtton;

    public GameObject AcceptRequestButton; //The button to accept an incoming request.
    public GameObject DenyRequestButton; //The button to deny an incoming request.
    public GameObject TradeRequestButton; //The button to request a trade with an existing friend.
    public GameObject RemoveFriendButton; //The button to remove an existing friend from your friends list.
    
    private void Start() 
    {
        GameManager = FindObjectOfType<GameManager>();
        FirebaseManager = GameManager.firebase;
    }

    public void SetData(UserData data)
    {
        UsernameText.text = data.Username;
        CashText.text = data.Cash.ToString();
        LevelText.text = data.Level.ToString();
        IconImage.sprite = data.Icon;
        ActivityCircle.color = data.IsOnline ? Color.green : Color.gray;
    }

    public async void AddFriendButton() 
    {
        GameManager._FriendsUIManager.FriendsUsernameSelected = UsernameText.text;
        await FirebaseManager.AddUserAsync(FirebaseManager.auth.CurrentUser.UserId, UsernameText.text);
        GameManager._FriendsUIManager.UpdateAddFriendUI(this);
    }

    public async void CancelFriendRequestButton()
    {
        GameManager._FriendsUIManager.FriendsUsernameSelected = UsernameText.text;
        await FirebaseManager.CancelRequestAsync(FirebaseManager.auth.CurrentUser.UserId, UsernameText.text);
        GameManager._FriendsUIManager.UpdateCancelRequestUI(this);
    }

    public async void AcceptFriendRequest()
    {
        GameManager._FriendsUIManager.FriendsUsernameSelected = UsernameText.text;
        await FirebaseManager.AcceptRequestAsync(FirebaseManager.auth.CurrentUser.UserId, UsernameText.text);
        GameManager._FriendsUIManager.UpdateFriendsUI(this);
    }

    public async void DeclineFriendRequest()
    {
        GameManager._FriendsUIManager.FriendsUsernameSelected = UsernameText.text;
        await FirebaseManager.DeclineRequestAsync(FirebaseManager.auth.CurrentUser.UserId, UsernameText.text);
    }

    public void TradeButton()
    {
      //  GameManager._FriendsUIManager.tradePanel.SetActive(true);
    }
}
