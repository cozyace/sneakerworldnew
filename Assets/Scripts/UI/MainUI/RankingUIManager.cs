
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RankingUIManager : MonoBehaviour
{
    public Leaderboard LeaderboardListingPrefab;
    public Transform ListingParent;

    private GameManager _GameManager;
    private FirebaseManager _Firebase;
    private MainNavigationSelection _Navigation;

    private void Awake()
    {
        _Navigation = FindObjectOfType<MainNavigationSelection>();
        _GameManager = FindObjectOfType<GameManager>();
        _Firebase = _GameManager.firebase;
    }

    private async void Start()
    {
        await CreateLeaderboardListings();
        //Opens the Ranking Window.
        _Navigation.SelectButton(1);
    }
    
    public void ClearListings()
    {
        foreach (Transform child in ListingParent)
        {
            Destroy(child.gameObject);
        }
    }
    
    public async Task CreateLeaderboardListings()
    {
        Dictionary<string, int> rankings = await _Firebase.CalculateLeaderboardRankingsAsync();

        int position = 0;
        
        foreach (KeyValuePair<string, int> rank in rankings)
        {
            position++;
            Leaderboard leaderboardListing = Instantiate(LeaderboardListingPrefab, ListingParent);
            leaderboardListing.UsernameText.text = rank.Key;
            leaderboardListing.CashText.text = rank.Value.ToString();
            leaderboardListing.UpdatePosition(position); 
        } 
    }
}
