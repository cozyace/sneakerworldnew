using TMPro;
using UnityEngine;

public class FriendItemListing : MonoBehaviour
{
    public TMP_Text username;
    public GameObject addFriendButton, cancelButton, friendsButton;
    public PlayFabController playfab;

    public void Friends()
    {
        addFriendButton.SetActive(false);
        cancelButton.SetActive(false);
        friendsButton.SetActive(true);
    }

    public void SubmitFriendRequest()
    {
        playfab.AddFriend(playfab.addFriendSearch);
        addFriendButton.SetActive(false);
        cancelButton.SetActive(true);
    }

    public void CancelButton()
    {
        cancelButton.SetActive(false);
        addFriendButton.SetActive(true);
    }

    public void TradeButton()
    {
        playfab.tradePanel.SetActive(true);
        playfab.Trade(this);
    }
}
