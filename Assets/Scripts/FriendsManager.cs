using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Friends;
using Unity.Services.Friends.Models;
using UnityEngine;

public class FriendsManager : MonoBehaviour
{
    public bool UseFriends = true;
    private static FriendsManager internalActive;
    public string username = "maverick";
    public Action onUserSignIn;
    private IReadOnlyList<Relationship> friends;

    public static FriendsManager Active
    {
        get
        {
            if (internalActive == null)
                internalActive = FindObjectOfType<FriendsManager>();

            return internalActive;
        }
    }

    private void Start()
    {
        if (UseFriends)
        {
            InitializeFriends();
        }
    }

    private async void InitializeFriends()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        if (AuthenticationService.Instance.IsSignedIn == true)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        await FriendsService.Instance.InitializeAsync();

        friends = FriendsService.Instance.Friends;
    }

    public async void PlayerSignIn()
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(username);

        Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId} : " +
            $"{AuthenticationService.Instance.Profile}" +
            $" {AuthenticationService.Instance.PlayerName}");

        onUserSignIn?.Invoke();
    }
}