using TMPro;
using UnityEngine;

public class UserItem : MonoBehaviour
{
    public TMP_Text usernameText;

    public void SetData(string username)
    {
        usernameText.text = username;
    }
}
