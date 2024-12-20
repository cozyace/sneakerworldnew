using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


public struct UserData
{
    public string Username;
    public int Cash;
    public int Level;
    public Sprite Icon;

    public UserData(string username, int cash, int level, Sprite icon)
    {
        Username = username;
        Cash = cash;
        Level = level;
        Icon = icon;
    }
}

public class FriendsUIManager : MonoBehaviour
{
    [SerializeField] private UserItem FriendListingPrefab;

    [SerializeField] private Transform FriendListingParent; //The parent of the player's current friends.
    [SerializeField] private Transform FriendRequestListingParent; //The parent of the incoming friend requests section of the 'Friends' menu.
    [SerializeField] private Transform AddFriendListingParent; //The parent of the 'Add Friends' menu children.

    private FirebaseManager _Firebase;
    private GameManager _GameManager;

    
    [Header("Friends")]
    public List<string> SearchedUsers;
    public List<string> Friends;
    public List<string> FriendRequests;
    public List<string> FriendRequestsSent;
    
    
    public TMPro.TextMeshProUGUI SearchFriendsErrorText;
    
    private List<string> _InstantiatedFriendsNames = new List<string>();
    private List<string> _InstantiatedFriendRequestsNames = new List<string>();
    private List<string> _InstantiatedFriendSearchNames = new List<string>();
    
    private List<UserItem> _InstantiatedFriendListings = new List<UserItem>();
    private List<UserItem> _InstantiatedFriendRequestListings = new List<UserItem>();
    private List<UserItem> _InstantiatedFriendSearchListings = new List<UserItem>();
    
    public string FriendsUsernameSelected;

    
    private async void Start()
    {
        _GameManager = FindFirstObjectByType<GameManager>();
        _Firebase = _GameManager.firebase;
        
       
        Friends = await _Firebase.UpdateFriendsAsync();
        FriendRequests = await _Firebase.UpdateFriendRequestsAsync();
        FriendRequestsSent = await _Firebase.UpdateFriendRequestsSentAsync();
        SetupDatabaseListeners();
    }
    
    // !!!!!!!! USE THIS FOR THE PLAYER MARKET INVENTORY UPDATING, SO IT DOESN'T REFRESH EVERY X AMOUNT OF SECONDS, AND INSTEAD WILL ONLY UPDATE WHEN NEEDED.
    private void SetupDatabaseListeners()
    {
        var snapshot = _Firebase.dbReference.Child($"users/{_Firebase.userId}/friends");
        snapshot.ChildAdded += ListenForFriendRequests;
        snapshot.ChildRemoved += ListenForFriendRequests;
        snapshot.ChildChanged += ListenForFriends;
    }



    public async Task RefreshFriends()
    {
        ClearUI();
        Friends.Clear();
        FriendRequests.Clear();
        FriendRequestsSent.Clear();
        Friends = await _Firebase.UpdateFriendsAsync();
        FriendRequests = await _Firebase.UpdateFriendRequestsAsync();
        FriendRequestsSent = await _Firebase.UpdateFriendRequestsSentAsync();
        await ListFriends();
    }

    public async Task UpdateFriendRequests()
    {
        FriendRequests.Clear();
        FriendRequestsSent.Clear();
        FriendRequests = await _Firebase.UpdateFriendRequestsAsync();
        FriendRequestsSent = await _Firebase.UpdateFriendRequestsSentAsync();
    }

    public async Task UpdateFriends()
    {
        Friends.Clear();
        Friends = await _Firebase.UpdateFriendsAsync();
    }

    public async void ListenForFriendRequests(object sender, ChildChangedEventArgs args)
    {
        ClearUI();
        await UpdateFriendRequests();
        await ListFriends();
    }

    public async void ListenForFriends(object sender, ChildChangedEventArgs args)
    {
        ClearUI();
        await UpdateFriends();
        await ListFriends();
    }

    public async void OnSearchUsers(string searchedUsername)
    {
        ClearUI();
        
        try
        {
            SearchFriendsErrorText.gameObject.SetActive(false);
            
            foreach (UserItem child in _InstantiatedFriendSearchListings)
                DestroyImmediate(child);

            _InstantiatedFriendSearchNames.Clear();
            SearchedUsers.Clear();
            SearchedUsers = await _Firebase.SearchUsersAsync(searchedUsername);

            //Removes people already on your friends list
            foreach (string friend in Friends)
            {
                string username = await _Firebase.GetUsernameFromUserIdAsync(friend);

                if (SearchedUsers.Contains(username))
                    SearchedUsers.Remove(username);
            }

            //Removes people with incoming friend requests towards you.
            foreach (string friendRequest in FriendRequests)
            {
                string username = await _Firebase.GetUsernameFromUserIdAsync(friendRequest);

                if (SearchedUsers.Contains(username))
                    SearchedUsers.Remove(username);
            }

            if (SearchedUsers.Count == 0)
            {
                SearchFriendsErrorText.gameObject.SetActive(true);
                SearchFriendsErrorText.text = "No matching users found!";
                return;
            }

            foreach (string user in SearchedUsers)
            {
                if (!_InstantiatedFriendSearchNames.Contains(user))
                {
                    _InstantiatedFriendSearchNames.Add(user);
                    UserItem userItem = Instantiate(FriendListingPrefab, AddFriendListingParent);
                    
                    PlayerStats loadedPlayerData = await _Firebase.LoadDataAsync(await _Firebase.GetUserIdFromUsernameAsync(user));
                    userItem.SetData(new UserData(user, loadedPlayerData.cash, loadedPlayerData.level, Resources.Load<Sprite>("DefaultAvatar")));

                    SetInitialFindFriendUI(userItem);
                    
                    //If you've already sent this person a request, update the button to show the proper UI.
                    if (FriendRequestsSent.Contains(await _Firebase.GetUserIdFromUsernameAsync(user)))
                        UpdateAddFriendUI(userItem);

                    _InstantiatedFriendSearchListings.Add(userItem);
                }
            }
        }
        catch (FirebaseException e)
        {
            SearchFriendsErrorText.gameObject.SetActive(true);
            SearchFriendsErrorText.text = e.Message;
        }
    }

    public async Task ListFriends()
    {
        foreach (UserItem child in _InstantiatedFriendListings)
            Destroy(child);
        
        foreach(UserItem child in _InstantiatedFriendRequestListings)
            Destroy(child);

        _InstantiatedFriendsNames.Clear();

        foreach (string friend in Friends)
        {
            string username = await _Firebase.GetUsernameFromUserIdAsync(friend);
            

            if (_InstantiatedFriendsNames.Contains(username))
                continue;
            
            _InstantiatedFriendsNames.Add(username);
            UserItem userItem = Instantiate(FriendListingPrefab, FriendListingParent);
            print("Looped friend - " + username);    
            
            PlayerStats loadedPlayerData = await _Firebase.LoadDataAsync(friend);
            userItem.SetData(new UserData(username, loadedPlayerData.cash, loadedPlayerData.level, Resources.Load<Sprite>("DefaultAvatar")));
            SetInitialActiveFriendUI(userItem);
            _InstantiatedFriendListings.Add(userItem);
        }

        foreach (string friend in FriendRequests)
        {
            string username = await _Firebase.GetUsernameFromUserIdAsync(friend);

            if (_InstantiatedFriendsNames.Contains(username))
                continue;
            
            _InstantiatedFriendsNames.Add(username);
            UserItem userItem = Instantiate(FriendListingPrefab, FriendRequestListingParent);
            PlayerStats loadedPlayerData = await _Firebase.LoadDataAsync(friend);
            userItem.SetData(new UserData(username, loadedPlayerData.cash, loadedPlayerData.level, Resources.Load<Sprite>("DefaultAvatar")));
            SetInitialIncomingRequestFriendUI(userItem);
            _InstantiatedFriendListings.Add(userItem);
        }
    }
 

    
    //When a active friend listing is created.
    public void SetInitialActiveFriendUI(UserItem item)
    {
        item.TradeRequestButton.SetActive(true);
        item.RemoveFriendButton.SetActive(true);
    }
    //When an incoming request is created.
    public void SetInitialIncomingRequestFriendUI(UserItem item)
    {
        item.AcceptRequestButton.SetActive(true);
        item.DenyRequestButton.SetActive(true);
    }
    
    //When you click on the 'Add' button. (In a 'Add Friends' search)
    public void UpdateAddFriendUI(UserItem item)
    {
        item.CancelButton.SetActive(true);
        item.AddButton.SetActive(false);
    }
    
    //When a friend search query is displayed
    public void SetInitialFindFriendUI(UserItem item)
    {
        item.AddButton.SetActive(true);
    }

    //When you press the 'Cancel' friend request button.
    public void UpdateCancelRequestUI(UserItem item)
    {
        item.AddButton.SetActive(true);
        item.CancelButton.SetActive(true);
    }
    
    
    

    private void ClearUI()
    {
        foreach (UserItem child in _InstantiatedFriendListings)
            if(child != null)
                Destroy(child.gameObject);

        foreach (UserItem child in _InstantiatedFriendRequestListings)
            if(child != null)
                Destroy(child.gameObject);
        
        foreach(UserItem child in _InstantiatedFriendSearchListings)
            if(child != null)
                Destroy(child.gameObject);
    }
    

    
    
}