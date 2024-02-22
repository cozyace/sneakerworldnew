using TMPro;
using UnityEngine;

public class UserItem : MonoBehaviour
{
    public TMP_Text usernameText;
    public GameManager gameManager;
    public FirebaseManager firebase;
    public GameObject addFriendButton, cancelFriendRequestButton, acceptFriendRequestButton, declineFriendRequestButton, tradeButtton;

    private void Start() 
    {
        gameManager = FindObjectOfType<GameManager>();
        firebase = gameManager.firebase;
    }

    public void SetData(string username)
    {
        usernameText.text = username;
    }

    public async void AddFriendButton() 
    {
        gameManager._friendUsernameSelected = usernameText.text;
        await firebase.AddUserAsync(firebase.auth.CurrentUser.UserId, usernameText.text);
        gameManager.UpdateAddFriendUI(this);
    }

    public async void CancelFriendRequestButton()
    {
        gameManager._friendUsernameSelected = usernameText.text;
        await firebase.CancelRequestAsync(firebase.auth.CurrentUser.UserId, usernameText.text);
        gameManager.UpdateCancelRequestUI(this);
    }

    public async void AcceptFriendRequest()
    {
        gameManager._friendUsernameSelected = usernameText.text;
        await firebase.AcceptRequestAsync(firebase.auth.CurrentUser.UserId, usernameText.text);
        gameManager.UpdateFriendsUI(this);
    }

    public async void DeclineFriendRequest()
    {
        gameManager._friendUsernameSelected = usernameText.text;
        await firebase.DeclineRequestAsync(firebase.auth.CurrentUser.UserId, usernameText.text);
    }

    public void TradeButton()
    {
        gameManager.tradePanel.SetActive(true);
    }
}
