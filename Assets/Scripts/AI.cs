using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("Movement")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int currentWaypointIndex = 0;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Purchase")]
    [SerializeField] private Transform popupPosition;
    [SerializeField] private GameObject itemPopupPrefab;
    [SerializeField] private GameObject cashPopupPrefab;
    [SerializeField] private GameObject waitingBarPrefab;
    [SerializeField] private int xpPerPurchase;
    [SerializeField] private int lastKnownTransactionAmount;
    [SerializeField] private RectTransform popupSpawn;

    [Header("Game Mananager")]
    public GameManager gameManager;

    private float waitingTime;
    private float currentWaitingTime;
    private bool waitingBarInstantiated = false;
    private GameObject waitingBar;

    private static readonly int Buy = Animator.StringToHash("Buy");

    private void Start()
    {
        if (gameManager == null) 
            gameManager = FindAnyObjectByType<GameManager>();

        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        waypoints = new[]
        {
            GameObject.FindGameObjectWithTag("mainDesk").transform,
            GameObject.FindGameObjectWithTag("finishedPurchase").transform,
            GameObject.FindGameObjectWithTag("exit").transform,
        };
        waitingTime = gameManager.upgradesManager.waitTime;
        currentWaitingTime = waitingTime;

        popupSpawn = GameObject.FindGameObjectWithTag("popupTransform").GetComponent<RectTransform>();
    }

    private void Update()
    {
        Transform waypoint = waypoints[currentWaypointIndex];
        if (Vector2.Distance(transform.position, waypoint.position) > moveSpeed * Time.deltaTime)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoint.position, moveSpeed * Time.deltaTime);
            animator.SetBool(Buy, false);
            if (transform.position.x < waypoint.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            transform.position = waypoint.position;
            if (waypoint.tag.Equals("exit")) Destroy(gameObject);
            if (waypoint.tag.Equals("mainDesk"))
            {
                if (!waitingBarInstantiated)
                {
                    waitingBar = Instantiate(waitingBarPrefab, popupPosition.position, Quaternion.identity);
                    waitingBar.GetComponent<RadialProgressBar>().Setup(popupPosition);
                    waitingBarInstantiated = true;
                }
                currentWaitingTime -= Time.deltaTime;
                waitingBar.GetComponent<RadialProgressBar>().SetFillAmount(1 - currentWaitingTime/waitingTime);
                if (currentWaitingTime > 0) return;
                waitingBar.SetActive(false);
                animator.SetBool(Buy, true);
                BuySneaker();
            }
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
    
    private void BuySneaker()
    { 
        var chosenSneaker = ChooseSneaker();
        if (chosenSneaker == null)
        {
            return;
        }
        CreateItemPopup(chosenSneaker);
        gameManager.inventoryManager.BuySneaker(chosenSneaker);
        gameManager.AddExperience(xpPerPurchase);
        lastKnownTransactionAmount = chosenSneaker.purchasedPrice;
        CreateCashPopup();

        if (gameManager.playerStats.level == 10 && gameManager.inventoryStats.numSneakers < 11)
            gameManager.inventoryManager.AddSneakerSlot();

        if (gameManager.playerStats.level == 25 && gameManager.inventoryStats.numSneakers < 26)
            gameManager.inventoryManager.AddSneakerSlot();

        if (gameManager.playerStats.level == 40 && gameManager.inventoryStats.numSneakers < 41)
            gameManager.inventoryManager.AddSneakerSlot();

        if (gameManager.playerStats.level == 75 && gameManager.inventoryStats.numSneakers < 76)
            gameManager.inventoryManager.AddSneakerSlot();

        if (gameManager.playerStats.level == 100 && gameManager.inventoryStats.numSneakers < 101)
            gameManager.inventoryManager.AddSneakerSlot();
    }

    private SneakerInventoryItem ChooseSneaker()
    {
        List<SneakerInventoryItem> sneakersAvailable = gameManager.inventoryManager.sneakers.Where(sneaker => sneaker.aiCanBuy).ToList();

        if (sneakersAvailable.Count == 0)
        {
            Debug.Log("No sneakers available to buy!");
            return null;
        }
        return sneakersAvailable[
            Random.Range(0, sneakersAvailable.Count)];
    }

    private void CreateCashPopup()
    {
        GameObject popup = Instantiate(cashPopupPrefab, popupSpawn);
        popup.GetComponent<CashPopup>().SetPopup(lastKnownTransactionAmount);
    }

    private void CreateItemPopup(SneakerInventoryItem sneakerInventoryItem)
    {
        GameObject popup = Instantiate(itemPopupPrefab, popupPosition.position, Quaternion.identity);
        popup.GetComponent<ItemPopup>().SetPopup(popupPosition, sneakerInventoryItem.sneakerImage.sprite);
    }

    public void IncreaseDrawOrderInLayer()
    {
        if (spriteRenderer != null) spriteRenderer.sortingOrder++;
    }
}