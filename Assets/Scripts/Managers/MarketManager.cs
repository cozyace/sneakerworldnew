using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public struct MarketListing
{
        public string description;
        public string key;
        public int listingPriceCash;
        public int listingPriceGems;
        public int quantity;
        public string sellerId;
        public int sneakerId;
        public DateTime timeStamp;

        public MarketListing(string d, string k, int lc, int lg, int q, string s, int sid, DateTime timestamp)
        {
                description = d;
                key = k;
                listingPriceCash = lc;
                listingPriceGems = lg;
                quantity = q;
                sellerId = s;
                sneakerId = sid;
                timeStamp = timestamp;
        }
}


[Serializable]
public struct PhysicalMarketListing
{
        public GameObject UI;
        public MarketListing Data;

        public PhysicalMarketListing(GameObject ui, MarketListing data)
        {
                UI = ui;
                Data = data;
        }
}

public class MarketManager :MonoBehaviour
{
        private GameManager _GameManager;
        
        [Header("References/Assets")]
        public GameObject MarketListingPrefab;
        public GameObject InventoryItemPrefab;
        [SerializeField] private Transform MarketContentRoot;
        [SerializeField] private GameObject InventoryPanel;
        [SerializeField] private GameObject MyListingsPanel;
        [SerializeField] private GameObject CreateListingPanel;
        [SerializeField] private GameObject DeleteListingConfirmationMenu;
        
        //Smaller-Importance UI References.
        [SerializeField] private Button CreateListingButton;
        [SerializeField] private Image SelectedShoeIcon;
        [SerializeField] private TMP_Text ListingCountLabel;
        [SerializeField] private GameObject InsufficientQuantityText;
        [SerializeField] private GameObject CashIcon;
        [SerializeField] private GameObject GemIcon;
        [SerializeField] private TMP_InputField PriceField;
        [SerializeField] private TMP_InputField QuantityField;
        [SerializeField] private TMP_InputField SearchMarketField;
        
        //Slot References
        [SerializeField] private Button[] InventoryItemSlots;
        [SerializeField] private GameObject[] YourListingSlots;
        
        //Asset Reference.
        public Sprite[] RarityPanels;
        
        //All listings on the market.
        public List<PhysicalMarketListing> _Listings;
        //The listings loaded that were made by the logged-in user.
        public List<PhysicalMarketListing> _MyListings;
        //The sneaker that you have selected in the 'choose item' box.
        private SneakersOwned _SelectedSneaker; 
        
        //The base amount of time before the first refresh of the market.
        private float _RefreshTimer = 0.5f;
        private bool _IsMarketInitiallyRefreshed = false;

        
        
        
        
        private void Awake()
        {
                _GameManager = GetComponent<GameManager>();
                SearchMarketField.onValueChanged.AddListener(delegate { SetMarketListingActiveByName(SearchMarketField.text); });
        }

        private void Update()
        {
                _RefreshTimer -= Time.deltaTime;
                if (_RefreshTimer <= 0)
                {
                        GetListings(_IsMarketInitiallyRefreshed);
                        RefreshInventory();
                    
                    if(!_IsMarketInitiallyRefreshed)
                        _IsMarketInitiallyRefreshed = true;
                    
                        _RefreshTimer = 2.5f;
                }

                if (_SelectedSneaker.name != "" && QuantityField.text != "")
                {
                        InsufficientQuantityText.SetActive(int.Parse(QuantityField.text) > _SelectedSneaker.quantity);
                }
                else
                        InsufficientQuantityText.SetActive(false);

                CreateListingButton.interactable = _MyListings.Count < 8;
        }


        public void RefreshButton()
        {
                GetListings(false);
                RefreshInventory();
        }
        
        
        //Populates either the global & player listings, or only player listings.
        private async void GetListings(bool reloadPlayerListingsOnly)
        {
                //If you're not only reloading the player's
                if (!reloadPlayerListingsOnly)
                {
                        //Delete all existing global market items.
                        foreach (Transform t in MarketContentRoot) 
                                Destroy(t.gameObject);
                }
                
                _MyListings.Clear();
                
                if(!reloadPlayerListingsOnly)
                        _Listings.Clear();
                
                //Store every listing in a temporary list.
                List<MarketListing> allMarketListings = await _GameManager.firebase.GetMarketplaceListingsAsync();

                //Insert the listing data into each slot of the dictionary
                foreach (MarketListing listing in allMarketListings)
                {
                        //If it's another player's listing.
                        if (listing.sellerId != _GameManager.firebase.userId)
                        {
                                if (!reloadPlayerListingsOnly)
                                 _Listings.Add(new PhysicalMarketListing(null, listing));
                        }
                        //If this is a listing made by the player.
                        else if (listing.sellerId == _GameManager.firebase.userId)
                        {
                                _MyListings.Add(new PhysicalMarketListing(null, listing));
                        }
                }

                //Go through all of the global listing (not the player's) and instantiate them individually.
                for (int listingIndex = 0; listingIndex < _Listings.Count; listingIndex++)
                {
                        //Create the physical representation of another user's market listing.
                        if(!reloadPlayerListingsOnly)
                                InstantiateMarketListing(_Listings[listingIndex].Data);
                }
                
                //Refreshes the player's listings.
                RefreshPlayerListings();
        }

        //Creates a single UI object representing a listing.
        private async void InstantiateMarketListing(MarketListing listing)
        {
                MarketListingItem listingInstance = Instantiate(MarketListingPrefab, MarketContentRoot).GetComponent<MarketListingItem>();

                //Find & store the data for the specific sneaker here, grab the data from the database.
                Sneaker sneaker = new Sneaker
                {
                        name = _GameManager.SneakerDatabase.Database[listing.sneakerId].Name,
                        imagePath = _GameManager.SneakerDatabase.Database[listing.sneakerId].Icon.name,
                        rarity = _GameManager.SneakerDatabase.Database[listing.sneakerId].Rarity,
                };

                listingInstance.UpdateUIComponents(
                        this, 
                        sneaker.name,
                        listing.listingPriceCash,
                        listing.listingPriceGems,
                        listing.quantity,
                        sneaker.rarity,
                        Resources.Load<Sprite>($"Sneakers/{sneaker.imagePath}"),
                        await _GameManager.firebase.GetUsernameFromUserIdAsync(listing.sellerId),
                        RarityPanels[(int)sneaker.rarity-1],
                        listing
                        );
                
                
                //Stores where the entry is on the list.
                int physicalMarketListingEntryIndex = _Listings.FindIndex(x=> x.Data.Equals(listing));
                //Update the data with the newly created gameobject.
                _Listings[physicalMarketListingEntryIndex] = new PhysicalMarketListing(listingInstance.gameObject, _Listings[physicalMarketListingEntryIndex].Data);
        }
        
        //Purchases the listing.
        public async void PurchaseListing(MarketListing listing)
        {
                int purchasePrice = 0;
                string notificationString = "";
                
                print($"Your Cash - {_GameManager.GetCash()}");
                print($"Listing Cost - {listing.listingPriceCash}");
                
                if (_GameManager.GetGems() < listing.listingPriceGems || _GameManager.GetCash() < listing.listingPriceCash)
                {
                        print("Insufficient Currency to Purchase!");
                        return;
                }
                
                //Check if cost requirements are met.
                if (listing.listingPriceCash > 0)
                {
                        purchasePrice = listing.listingPriceCash;
                        notificationString = purchasePrice + " Cash";
                }
                else if (listing.listingPriceGems > 0)
                {
                        purchasePrice = listing.listingPriceGems;
                        notificationString = purchasePrice + " Gems";
                }
                        
                
                //Store the shoe as a Sneaker type.
                Sneaker sneaker = new Sneaker
                {
                        name = _GameManager.SneakerDatabase.Database[listing.sneakerId].Name,
                        imagePath =  _GameManager.SneakerDatabase.Database[listing.sneakerId].Icon.name,
                        rarity = _GameManager.SneakerDatabase.Database[listing.sneakerId].Rarity
                };
                
                //Remove the shoe from the listing.
                await _GameManager.firebase.RemoveMarketListing(listing.key);
                
                _GameManager.inventoryManager.AddShoesToCollection(new SneakersOwned(sneaker.name, listing.quantity, _GameManager.SneakerDatabase.Database.Find(x => x.Name == sneaker.name).Value, sneaker.rarity));
                        
                _GameManager.DeductCash(listing.listingPriceCash);
                _GameManager.DeductGems(listing.listingPriceGems);
                
                //Destroy & Refresh the listings.
                GetListings(false);
                
                //Update the seller's gold in the database.
               await _GameManager.firebase.UpdateGoldAsync(listing.sellerId, listing.listingPriceCash);
               await _GameManager.firebase.UpdateGemsAsync(listing.sellerId, listing.listingPriceGems);
               
               //Inform the seller that their listing has been sold.
               await _GameManager.firebase.AddNotificationToUser(listing.sellerId, $"Your listing of {listing.quantity}x {_GameManager.SneakerDatabase.Database[listing.sneakerId].Name} has sold for {notificationString}");
               
        }
        
        //All this does is remove the market listing data from the Firebase database.
        private async void RemoveListing(string key)
        {
                await _GameManager.firebase.RemoveMarketListing(key);
        }

        //Creates a new listing with the chosen item. (Called via UI button in game)
        public async void CreateNewListing()
        {
                if (!CanListSelectedItem())
                        return;

                int cashValue = 0;
                int gemValue = 0;

                if (CashIcon.activeSelf)
                        cashValue = int.Parse(PriceField.text);
                else if (GemIcon.activeSelf) 
                        gemValue = int.Parse(PriceField.text);

                //Generate a new GUID to act as the key for the listing. (Very very very very low probability that two of the same will exist at the same time ever)
                Guid key = Guid.NewGuid();
                
                //Add the listing to the database.
                await _GameManager.firebase.AddMarketListing(new MarketListing("Item Description...", key.ToString(), cashValue, gemValue, int.Parse(QuantityField.text), _GameManager.firebase.userId, _GameManager.SneakerDatabase.Database.FindIndex(x => x.Name == _SelectedSneaker.name), DateTime.Now));
                
                
                SneakersOwned newData = _GameManager.inventoryManager.SneakersOwned[_GameManager.inventoryManager.SneakersOwned.FindIndex(x => x.name == _SelectedSneaker.name)];
                newData.quantity = int.Parse(QuantityField.text);
                
                //Remove the quantity from the player's possession.
                _GameManager.inventoryManager.RemoveShoeFromCollection(newData);
                
                GetListings(false);
                RefreshInventory();
                ResetListingElements();
                
                CreateListingPanel.SetActive(false);
                MyListingsPanel.SetActive(true);
        }
        
        
        
        //Deletes all existing inventory UI items, and then respawns them.
        private void RefreshInventory()
        {
                for (int i = 0; i < InventoryItemSlots.Length; i++)
                {
                        if (InventoryItemSlots[i].transform.childCount == 0)
                                continue;

                        for (int x = 0; x < InventoryItemSlots[i].transform.childCount; x++)
                        {
                                Destroy(InventoryItemSlots[i].transform.GetChild(0).gameObject);
                        }
                }
                
                PopulateMarketInventory();
        }

        //Destroys all existing 'My Listings', enables/disables trash button, and spawns new 'My Listings'.
        private void RefreshPlayerListings()
        {
                foreach (GameObject t in YourListingSlots)
                {
                        bool isListingPresent = t.transform.childCount > 1;
                        
                        Button deleteButton = t.transform.GetChild(0).GetComponent<Button>();
                        Image buttonImage = t.transform.GetChild(0).GetComponent<Image>();
                        
                        deleteButton.enabled = isListingPresent;
                        buttonImage.enabled = isListingPresent;
                        
                        //If the only child is the trash can, skip this.
                        if (!isListingPresent)
                                continue;
                        
                        Destroy(t.transform.GetChild(1).gameObject);
                }
                
                print("Refreshing Listings");
                PopulateClientListings();
        }
        
        //Populates the inventory within the marketplace.
        private void PopulateMarketInventory()
        {
                for (int i = 0; i < InventoryItemSlots.Length; i++)
                {
                        if (i == _GameManager.inventoryManager.SneakersOwned.Count)
                                return;
                        
                        //Grab the UI Components.
                        Image inventoryShoeInstance = Instantiate(InventoryItemPrefab, InventoryItemSlots[i].transform).GetComponent<Image>();
                        TMP_Text shoeName = inventoryShoeInstance.transform.GetChild(0).GetComponent<TMP_Text>();
                        TMP_Text shoeQuantity = inventoryShoeInstance.transform.GetChild(1).GetComponent<TMP_Text>();
                        
                        //Assign the data to the UI Components.
                        shoeName.text = _GameManager.inventoryManager.SneakersOwned[i].name;
                        shoeQuantity.text = _GameManager.inventoryManager.SneakersOwned[i].quantity.ToString();
                        string imagePath = _GameManager.SneakerDatabase.Database.Find(x => x.Name == _GameManager.inventoryManager.SneakersOwned[i].name).Icon.name;
                        inventoryShoeInstance.sprite = Resources.Load<Sprite>($"Sneakers/{imagePath}");
                        
                        //Store index because this is in a loop, so it will not work otherwise.
                        int i1 = i; 
                        InventoryItemSlots[i].onClick.AddListener(()=> SelectItemToList(_GameManager.inventoryManager.SneakersOwned[i1]));
                }
        }

        //Populates the 'My Listings' section.
        private void PopulateClientListings()
        {
                //Set the count label of the player's listing to the correct amount.
               // ListingCountLabel.text = _MyListings.Count.ToString();
                
                for (int i = 0; i < YourListingSlots.Length; i++)
                {
                        if (i == _MyListings.Count)
                                return;
                        
                        //Grab the UI Components.
                        Image inventoryShoeInstance = Instantiate(InventoryItemPrefab, YourListingSlots[i].transform).GetComponent<Image>();
                        TMP_Text shoeName = inventoryShoeInstance.transform.GetChild(0).GetComponent<TMP_Text>();
                        TMP_Text shoeQuantity = inventoryShoeInstance.transform.GetChild(1).GetComponent<TMP_Text>();
                        Button deleteButton = YourListingSlots[i].transform.GetChild(0).GetComponent<Button>();

                        TMP_Text shoeCost = inventoryShoeInstance.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
                        Image shoeCurrencyIcon = inventoryShoeInstance.transform.GetChild(2).GetChild(1).GetComponent<Image>();

                        if (_MyListings[i].Data.listingPriceCash > 0)
                        {
                                shoeCost.text = _MyListings[i].Data.listingPriceCash.ToString();
                                shoeCurrencyIcon.sprite = Resources.Load<Sprite>("Currencies/Cash");
                        }
                        else if (_MyListings[i].Data.listingPriceCash > 0)
                        {
                                shoeCost.text = _MyListings[i].Data.listingPriceGems.ToString();
                                shoeCurrencyIcon.sprite = Resources.Load<Sprite>("Currencies/Gem");  
                        }
                        
                        //Assign the data to the UI components.
                        shoeName.text = _GameManager.SneakerDatabase.Database[_MyListings[i].Data.sneakerId].Name;
                        shoeQuantity.text = _MyListings[i].Data.quantity.ToString();
                 
                        string imagePath = _GameManager.SneakerDatabase.Database[_MyListings[i].Data.sneakerId].Icon.name;
                        inventoryShoeInstance.sprite = Resources.Load<Sprite>($"Sneakers/{imagePath}");
                        
                        //Store the index at which the data exists.
                        int myListingPhysicalDataIndex = _MyListings.FindIndex(x=> x.Data.Equals(_MyListings[i].Data));
                        //Reassign the data with the newly created UI object.
                        _MyListings[myListingPhysicalDataIndex] = new PhysicalMarketListing(inventoryShoeInstance.gameObject, _MyListings[myListingPhysicalDataIndex].Data);
                        
                        //Store index because this is in a loop, so it will not work otherwise.
                        int storedIndex = i;
                        deleteButton.onClick.AddListener(()=> EnableDeleteConfirmationMenu(_MyListings[storedIndex].Data));
                }
        }
        


        private void SetMarketListingActiveByName(string query)
        {
                foreach (PhysicalMarketListing listing in _Listings)
                {
                        if (query == "") //If the query is empty, then turn them all on by setting the query to each name.
                                query = _GameManager.SneakerDatabase.Database[listing.Data.sneakerId].Name;
                        
                        listing.UI.SetActive(_GameManager.SneakerDatabase.Database[listing.Data.sneakerId].Name.ToUpper().Contains(query.ToUpper()));
                }
        }
        

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public void FilterByRarity(TMP_Dropdown dropdown)
        {
                foreach (Transform marketItem in MarketContentRoot.transform)
                {
                        MarketListingItem sneaker = marketItem.GetComponent<MarketListingItem>();
                        marketItem.gameObject.SetActive(dropdown.value == 0 || dropdown.value == (int)sneaker.Rarity);
                }
        }
    
        public void FilterByBrand(TMP_Dropdown dropdown)
        {
                foreach (Transform marketItem in MarketContentRoot.transform)
                {
                        MarketListingItem sneaker = marketItem.GetComponent<MarketListingItem>();
                        marketItem.gameObject.SetActive(dropdown.value == 0 ||
                                sneaker.Name.ToLower().Contains(dropdown.options[dropdown.value].text.ToLower()));
                }
        }
        
        public void SortByOption(TMP_Dropdown dropdown)
        {
                List<Transform> children = new List<Transform>();
                for (int i = MarketContentRoot.transform.childCount - 1; i >= 0; i--)
                {
                        Transform child = MarketContentRoot.transform.GetChild(i);
                        children.Add(child);
                        child.parent = null;
                }

                List<Transform> sortedList = new List<Transform>();
                switch (dropdown.value)
                {
                        case 0:
                                sortedList = SortByTime(children, true);
                                break;
                        case 1:
                                sortedList = SortByTime(children, false);
                                break;
                        case 2:
                                sortedList = SortByName(children, true);
                                break;
                        case 3:
                                sortedList = SortByName(children, false);
                                break;
                        case 4:
                                sortedList = SortByRarity(children, true);
                                break;
                        case 5:
                                sortedList = SortByRarity(children, false);
                                break;
                        case 6:
                                sortedList = SortByPrice(children, true);
                                break;
                        case 7:
                                sortedList = SortByPrice(children, false);
                                break;
                }
        
                foreach (var sneaker in sortedList)
                {
                        sneaker.parent = MarketContentRoot.transform;
                }
        }
        
        
    private List<Transform> SortByTime(List<Transform> sneakers,bool newestFirst)
    {
        if (newestFirst)
        {
            sneakers.Sort((Transform t1, Transform t2) =>
                DateTime.Compare(t1.GetComponent<MarketListingItem>().ListingData.timeStamp,
                    t2.GetComponent<MarketListingItem>().ListingData.timeStamp));
        }
        else
        {
            sneakers.Sort((Transform t1, Transform t2) =>
                DateTime.Compare(t2.GetComponent<MarketListingItem>().ListingData.timeStamp,
                    t1.GetComponent<MarketListingItem>().ListingData.timeStamp));
        }

        return sneakers;
    }
    
    private List<Transform> SortByName(List<Transform> sneakers,bool ascending)
    {
        if (ascending)
        {
            sneakers.Sort((Transform t1, Transform t2) => string.Compare(t1.GetComponent<MarketListingItem>().Name, t2.GetComponent<MarketListingItem>().Name, StringComparison.Ordinal));
        }
        else
        {
            sneakers.Sort((Transform t1, Transform t2) => string.Compare(t2.GetComponent<MarketListingItem>().Name, t1.GetComponent<MarketListingItem>().Name, StringComparison.Ordinal));
        }

        return sneakers;
    }
    
    private List<Transform> SortByRarity(List<Transform> sneakers,bool ascending)
    {
        if (ascending)
        {
            sneakers.Sort((Transform t1, Transform t2) => t1.GetComponent<MarketListingItem>().Rarity
                .CompareTo(t2.GetComponent<MarketListingItem>().Rarity));
        }
        else
        {
            sneakers.Sort((Transform t1, Transform t2) => t2.GetComponent<MarketListingItem>().Rarity
                .CompareTo(t1.GetComponent<MarketListingItem>().Rarity));
        }

        return sneakers;
    }
    
    private List<Transform> SortByPrice(List<Transform> sneakers,bool ascending)
    {
        if (ascending)
        {
            sneakers.Sort((Transform t1, Transform t2) => t1.GetComponent<MarketListingItem>().CashPrice
                .CompareTo(t2.GetComponent<MarketListingItem>().CashPrice));
        }
        else
        {
            sneakers.Sort((Transform t1, Transform t2) => t2.GetComponent<MarketListingItem>().CashPrice
                .CompareTo(t1.GetComponent<MarketListingItem>().CashPrice));
        }

        return sneakers;
    }
        
    
    
    
    
    
    
    
    
    
    
    
    
        
    

        private void EnableDeleteConfirmationMenu(MarketListing listing)
        {
                //Enable the confirmation UI element.
                DeleteListingConfirmationMenu.SetActive(true);
                
                //Get the confirmation button.
                Button confirmButton = DeleteListingConfirmationMenu.transform.GetChild(0).GetChild(1).GetComponent<Button>();
                
                //Attach the hook to remove the specific listing.
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(()=> StartCoroutine(ConfirmRemoveListing(listing.key, listing)));
        }

       
        //Removes listing, and returns the shoe to the seller.
        private IEnumerator ConfirmRemoveListing(string key, MarketListing listing)
        {
                //Give the shoe back to the lister.
                _GameManager.inventoryManager.AddShoesToCollection(new SneakersOwned(_GameManager.SneakerDatabase.Database[listing.sneakerId].Name, listing.quantity, _GameManager.SneakerDatabase.Database.Find(x => x.Name == _GameManager.SneakerDatabase.Database[listing.sneakerId].Name).Value, _GameManager.SneakerDatabase.Database[listing.sneakerId].Rarity));
                
                //Disable the confirmation menu.
                DeleteListingConfirmationMenu.SetActive(false);
                
                //Remove the listing from the Firebase Database.
                RemoveListing(key);

                //Wait a second.
                yield return new WaitForSeconds(0.35f);
                
                //Refresh everything.
                RefreshPlayerListings();
                GetListings(true);
                RefreshInventory();
        }
        

    

        //This method is attached to each of the UI elements instantiated in the inventory, and make you select the item when you click.
        private void SelectItemToList(SneakersOwned sneaker)
        {
                InventoryPanel.SetActive(false);
                SelectedShoeIcon.gameObject.SetActive(true);
                
                string imagePath = _GameManager.SneakerDatabase.Database.Find(x => x.Name == sneaker.name).Icon.name;
                
                SelectedShoeIcon.sprite = Resources.Load<Sprite>($"Sneakers/{imagePath}");
                _SelectedSneaker = sneaker;
                //Put sprite in the main box.
        }

        

        
        
    
    
    
        //Finds if the listing already exists, and returns the index. (If it doesn't exist, returns -1)
        private int GetMarketListingIndexByData(MarketListing listing, IReadOnlyList<PhysicalMarketListing> list)
        {
                for (int i = 0; i < list.Count; i++) 
                {
                        if (list[i].Data.Equals(listing))
                        { 
                                return i;
                        }
                }
                //Do a check here, go through each of the existing listings in the list, and check if it still exists.
                return -1;
        }
    
        //Does the current entered listing meet all the requirements for posting.
        private bool CanListSelectedItem()
        {
                if (_SelectedSneaker.name == "")
                {
                        print("Must select a shoe!");
                        return false;
                }
                
                if (QuantityField.text == "")
                {
                        print("Must enter a quantity!");
                        return false;
                }

                if (PriceField.text == "")
                {
                        print("Must enter a price to create listing!");
                        return false;
                }

                if (_SelectedSneaker.quantity < int.Parse(QuantityField.text)) 
                {
                        print("You don't have enough of this item!");
                        return false;
                }
                
                return true;
        }

        
        //Resets the UI data for the current listing.
        public void ResetListingElements()
        {
                _SelectedSneaker = new SneakersOwned();
                SelectedShoeIcon.gameObject.SetActive(false);
                SelectedShoeIcon.sprite = null;
                QuantityField.text = "";
                PriceField.text = "";
        }
        
        public void SwapCurrencyType()
        {
                CashIcon.SetActive(!CashIcon.activeSelf);
                GemIcon.SetActive(!GemIcon.activeSelf);
        }
        
        
}

