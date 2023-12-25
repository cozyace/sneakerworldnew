using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UsernameObjectsUI : MonoBehaviour
{
    public GameObject userItemGO;
    public GameObject addFriendButton, cancelButton, friendsButton;
    public GameObject errorTextGO;
    public TMP_InputField searchInputField;

    public void UpdateUserListUI()
    {
        userItemGO.SetActive(false);
        errorTextGO.SetActive(false);
        string username = string.Empty;

        if (FirebaseManager.instance.usernames.Count < 1)
        {
            errorTextGO.SetActive(true);
        }
        else
        {
            username = FirebaseManager.instance.usernames[0];
            userItemGO.SetActive(true);
            userItemGO.GetComponent<UserItem>().SetData(username);
        }
    }

    public void SearchButton()
    {
        StartCoroutine(SearchLogic());
    }

    public void AddOrRemoveFriendButton()
    {

    }

    private IEnumerator SearchLogic()
    {
        FirebaseManager.instance.SearchUsers(searchInputField.text);
        yield return new WaitForSeconds(0.5f);
        UpdateUserListUI();
    }
}
