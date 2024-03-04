﻿using System;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite[] AvailableSprites;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private SkeletonAnimation Skeleton;

    [Header("Movement")]
    [SerializeField] private float MoveSpeed;
    private Transform _ExitStoreWaypoint;
    private Transform _CurrentWaypoint;
    
    [Header("Animation")]
    [SerializeField] private Animator Animator;
    
    [Header("Purchase")]
    [SerializeField] private Transform PopupPosition;
    [SerializeField] private GameObject ItemPopupPrefab;
    [SerializeField] private CashPopup CashPopupPrefab;
    [SerializeField] private GameObject WaitingBarPrefab;
    [SerializeField] private int XpGainedPerPurchase;
    [SerializeField] private int LastKnownTransactionAmount;
    
    private RectTransform _PopupParentObject;
    private GameManager _GameManager;
    
    private float _BaseWaitingTime; //The base amount of time required for them to complete the transaction.
    private float _CurrentWaitingTime; //Their current counter for the amount of time needed left in the transaction.
    private GameObject _WaitingBar; //The reference to the transaction circle UI.

    private static readonly int Buy = Animator.StringToHash("Buy");

    
    
    
    
    
    private void Start()
    {
        if (_GameManager == null) 
            _GameManager = FindAnyObjectByType<GameManager>();

        //Assign this AI's sprite to a randomized sprite within a list.
        SpriteRenderer.sprite = AvailableSprites[Random.Range(0, AvailableSprites.Length)];
        
        //Get the store exit waypoint.
        _ExitStoreWaypoint = GameObject.FindGameObjectWithTag("exit").transform;
        
        //Grab the base waiting time from the GameManager.
        _BaseWaitingTime = _GameManager.upgradesManager.waitTime;
        
        //Set the current time to the base.
        _CurrentWaitingTime = _BaseWaitingTime;

        
        _PopupParentObject = GameObject.FindGameObjectWithTag("popupTransform").GetComponent<RectTransform>();
    }

    public void UpdateDestination(Transform waypoint) => _CurrentWaypoint = waypoint;
    private void Update()
    {
        bool isAIMoving = Vector2.Distance(transform.position, _CurrentWaypoint.position) > MoveSpeed * Time.deltaTime;
        
        //If the AI is still far from the waypoint.
        if (isAIMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, _CurrentWaypoint.position, MoveSpeed * Time.deltaTime);
            Skeleton.AnimationName = "Walk";
            Animator.SetBool(Buy, false);
            
            if (transform.position.x < _CurrentWaypoint.position.x)
            {
                SpriteRenderer.flipX = true;
            }
            else
            {
                SpriteRenderer.flipX = false;
            }
        }
        else //If the AI has reached the desired waypoint.
        {
            transform.position = _CurrentWaypoint.position;
            Skeleton.AnimationName = "Idle";
            
            //If the AI has arrived at the exit location.
            if (_CurrentWaypoint.CompareTag("exit") && Vector2.Distance(transform.position, _CurrentWaypoint.position) < MoveSpeed * Time.deltaTime)
            {
                _GameManager.aiManager.DeleteAI(this);
            }
            
            //Once the AI is done their purchase, they're sent to this waypoint, then when they reach it, they'll be sent to the exit.
            if (_CurrentWaypoint.CompareTag("finishedPurchase"))
            {
                UpdateDestination(_ExitStoreWaypoint);
            }
            
            //If the AI has arrived at the counter to purchase a shoe.
            if (_CurrentWaypoint.CompareTag("mainDesk"))
            {
                
                if (!_WaitingBar)
                {
                    _WaitingBar = Instantiate(WaitingBarPrefab, PopupPosition.position, Quaternion.identity);
                    _WaitingBar.GetComponent<RadialProgressBar>().Setup(PopupPosition);
                }
                
                //Begin the countdown for them to continue on their path.
                _CurrentWaitingTime -= Time.deltaTime;
                //Tick the radial progress bar up.
                _WaitingBar.GetComponent<RadialProgressBar>().SetFillAmount(1 - _CurrentWaitingTime/_BaseWaitingTime);
                
                //If the timer isn't complete, return early.
                if (_CurrentWaitingTime > 0) return;
                
                //Once the transaction is complete
                _GameManager._CustomerQueue.CompleteActiveTransaction(this);
                _GameManager.aiManager.SatisfyAI(this);
                _WaitingBar.SetActive(false);
                Animator.SetBool(Buy, true);
                BuySneaker();
               
            }
        }
    }
    
    private void BuySneaker()
    { 
        //Decide which shoe the customer would like.
        SneakerInventoryItem chosenSneaker = ChooseSneaker();
        
        //If there's no shoes available, return early.
        if (chosenSneaker == null)
            return;
        
        //Create the Item & Cash UI popups.
        CreateItemPopup(chosenSneaker);
        CreateCashPopup();
        
        //Remove the X amount of quantity from the sneaker the AI desired to purchase.
        _GameManager.inventoryManager.BuySneaker(chosenSneaker);
        
        //Give the player the experience given per purchase.
        _GameManager.AddExperience(XpGainedPerPurchase);
        
        //Store the last known transaction.
        LastKnownTransactionAmount = chosenSneaker.purchasePrice;
        

        //Move the below code to the GameManager
        if (_GameManager.playerStats.level == 10)
            _GameManager.inventoryManager.AddSneakerSlot();

        if (_GameManager.playerStats.level == 25)
            _GameManager.inventoryManager.AddSneakerSlot();

        if (_GameManager.playerStats.level == 40)
            _GameManager.inventoryManager.AddSneakerSlot();

        if (_GameManager.playerStats.level == 75)
            _GameManager.inventoryManager.AddSneakerSlot();

        if (_GameManager.playerStats.level == 100)
            _GameManager.inventoryManager.AddSneakerSlot();
    }

    //Returns the shoe the customer would like to buy. (Randomized)
    private SneakerInventoryItem ChooseSneaker()
    {
        List<SneakerInventoryItem> sneakersAvailable = _GameManager.inventoryManager.sneakers.Where(sneaker => sneaker.aiCanBuy).ToList();

        if (sneakersAvailable.Count == 0)
        {
            Debug.Log("No sneakers available to buy!");
            return null;
        }
        return sneakersAvailable[Random.Range(0, sneakersAvailable.Count)];
    }
    
    //Instantiates the UI popup for the cash gained from the transaction.
    private void CreateCashPopup()
    {
        CashPopup popup = Instantiate(CashPopupPrefab, _PopupParentObject);
        popup.SetPopup(LastKnownTransactionAmount);
    }

    
    //Instantiates the UI popup for the item lost in the transaction.
    private void CreateItemPopup(SneakerInventoryItem sneakerInventoryItem)
    {
        GameObject popup = Instantiate(ItemPopupPrefab, PopupPosition.position, Quaternion.identity);
        popup.GetComponent<ItemPopup>().SetPopup(PopupPosition, sneakerInventoryItem.sneakerImage.sprite);
    }
    
    //Reorders the sprite of the AI.
    public void IncreaseDrawOrderInLayer()
    {
        if (SpriteRenderer != null) SpriteRenderer.sortingOrder++;
    }
    
}