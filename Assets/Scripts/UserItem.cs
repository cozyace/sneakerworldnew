using TMPro;
using UnityEngine;

public class UserItem : MonoBehaviour
{
    public TMP_Text usernameText;
    public GameManager gameManager;
    public FirebaseManager firebase;

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
    }
}
