using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Serialization;


public struct UserData
{
    public string Username;
    public int Cash;
    public int Level;
    public Sprite Icon;
    public bool IsOnline;

    public UserData(string username, int cash, int level, Sprite icon, bool isOnline)
    {
        Username = username;
        Cash = cash;
        Level = level;
        Icon = icon;
        IsOnline = isOnline;
    }
}

public class FriendsUIManager : MonoBehaviour
{
    [SerializeField] private UserItem FriendListingPrefab;
    [SerializeField] private UserItem FriendRequestPrefab;

    [SerializeField] private Transform FriendListingParent;
    [SerializeField] private Transform FriendRequestListingParent;

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
    
    private List<UserItem> _InstantiatedFriendListings = new List<UserItem>();
    private List<UserItem> _InstantiatedFriendRequestListings = new List<UserItem>();
    
    public string FriendsUsernameSelected;

    
    private async void Start()
    {
        _GameManager = FindObjectOfType<GameManager>();
        _Firebase = _GameManager.firebase;
        
       
        Friends = await _Firebase.UpdateFriendsAsync();
        FriendRequests = await _Firebase.UpdateFriendRequestsAsync();
        FriendRequestsSent = await _Firebase.UpdateFriendRequestsSentAsync();
        SetupDatabaseListeners();
    }
    
    
    private void SetupDatabaseListeners()
    {
        var snapshot = _Firebase.dbReference.Child($"users/{_Firebase.userId}/friends");
        snapshot.ChildAdded += ListenForFriendRequests;
        snapshot.ChildRemoved += ListenForFriendRequests;
        snapshot.ChildChanged += ListenForFriends;
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

    public async void OnSearchUsers(string _username)
    {
        try
        {
            SearchFriendsErrorText.gameObject.SetActive(false);
            foreach (UserItem child in _InstantiatedFriendRequestListings)
                DestroyImmediate(child.gameObject);

            _InstantiatedFriendRequestsNames.Clear();
            SearchedUsers.Clear();
            SearchedUsers = await _Firebase.SearchUsersAsync(_username);

            foreach (string friend in Friends)
            {
                string username = await _Firebase.GetUsernameFromUserIdAsync(friend);

                if (SearchedUsers.Contains(username))
                    SearchedUsers.Remove(username);
            }

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
                if (!_InstantiatedFriendRequestsNames.Contains(user))
                {
                    _InstantiatedFriendRequestsNames.Add(user);
                    UserItem userItem = Instantiate(FriendRequestPrefab, FriendRequestListingParent);
                    userItem.UsernameText.text = user;

                    if (FriendRequestsSent.Contains(await _Firebase.GetUserIdFromUsernameAsync(user)))
                        UpdateAddFriendUI(userItem);

                    _InstantiatedFriendRequestListings.Add(userItem);
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

        _InstantiatedFriendsNames.Clear();

        foreach (var friend in Friends)
        {
            string username = await _Firebase.GetUsernameFromUserIdAsync(friend);

            if (!_InstantiatedFriendsNames.Contains(username))
            {
                _InstantiatedFriendsNames.Add(username);
                UserItem userItem = Instantiate(FriendListingPrefab, FriendListingParent);
                userItem.UsernameText.text = username;
                UpdateFriendsUI(userItem);
                _InstantiatedFriendListings.Add(userItem);
            }
        }

        foreach (var friend in FriendRequests)
        {
            string username = await _Firebase.GetUsernameFromUserIdAsync(friend);

            if (!_InstantiatedFriendsNames.Contains(username))
            {
                _InstantiatedFriendsNames.Add(username);
                UserItem userItem = Instantiate(FriendListingPrefab, FriendListingParent);
                userItem.UsernameText.text = username;
                _InstantiatedFriendListings.Add(userItem);
            }
        }
    }

    public void UpdateAddFriendUI(UserItem item)
    {
        if (item.addFriendButton != null) item.addFriendButton.SetActive(false);
        if (item.cancelFriendRequestButton != null) item.cancelFriendRequestButton.SetActive(true);
    }

    public void UpdateFriendsUI(UserItem item)
    {
        if (item.acceptFriendRequestButton != null) item.acceptFriendRequestButton.SetActive(false);
        if (item.declineFriendRequestButton != null) item.declineFriendRequestButton.SetActive(false);
        if (item.tradeButtton != null) item.tradeButtton.SetActive(true);
    }

    public void UpdateCancelRequestUI(UserItem item)
    {
        if (item.tradeButtton != null) item.tradeButtton.SetActive(false);
        if (item.acceptFriendRequestButton != null) item.acceptFriendRequestButton.SetActive(false);
        if (item.cancelFriendRequestButton != null) item.cancelFriendRequestButton.SetActive(false);
        if (item.declineFriendRequestButton != null) item.declineFriendRequestButton.SetActive(false);
        if (item.addFriendButton != null) item.addFriendButton.SetActive(true);
    }

  

    private void ClearUI()
    {
        foreach (UserItem child in _InstantiatedFriendListings)
            Destroy(child);

        foreach (UserItem child in _InstantiatedFriendRequestListings)
            Destroy(child.gameObject);
    }
    
    //This will instantiate a single friend request listing prefab.
    public void InstantiateFriendRequestPrefab(UserData userData)
    {
        UserItem requestListing = Instantiate(FriendRequestPrefab, FriendRequestListingParent);
        requestListing.SetData(userData);
    }

    //This will instantiate a single friend listing prefab.
    public void InstantiateFriendPrefab(UserData userData)
    {
        UserItem friendListing = Instantiate(FriendListingPrefab, FriendListingParent);
        friendListing.SetData(userData);
    }

    //This will accept an incoming friend request
    public void AcceptFriendRequest()
    {
        
    }

    //This will decline an incoming friend request.
    public void DeclineFriendRequest()
    {
        
    }

    //This will initiate a trade request to the selected friend.
    public void InitiateTradeRequest()
    {
        
    }

    //This method will remove a friend that's currently on your friends list.
    public void RemoveExistingFriend()
    {
        
    }
    
    
}
