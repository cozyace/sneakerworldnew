using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomerQueue : MonoBehaviour
{
    //All of the customers waiting in the line.
    public List<AI> CustomersInQueue = new List<AI>();
    
    //All of the space in your store you have for customers to form a line (based on how large your store is)
    public List<Transform> QueuePositions = new List<Transform>();
    //All of the available counter positions (based on how many employees you have) YOU CAN DISABLE A COUNTER POSITION BY TICKING THE UNLOCKED OFF
    public List<TransactionCounterPosition> CounterPositions = new List<TransactionCounterPosition>();
    public Transform LeaveCounterWaypoint;

    

    private void Update()
    {
        int availableCounterIndex = -1;
        
        for (int i = 0; i < CounterPositions.Count; i++)
        {
            if (CounterPositions[i].ActiveCustomer == null && CounterPositions[i].IsPositionUnlocked)
            {
                availableCounterIndex = i;
            }
        }
        
        for (int x = 0; x < CustomersInQueue.Count; x++)
        {
            CustomersInQueue[x].UpdateDestination(QueuePositions[x]);
        }
        
        //If there's customers in the queue, there's an open counter, and that counter is unlocked.
        if (CustomersInQueue.Count > 0 && availableCounterIndex != -1)
        {
            CounterPositions[availableCounterIndex] = new TransactionCounterPosition(CounterPositions[availableCounterIndex].Position, CustomersInQueue[0], CounterPositions[availableCounterIndex].IsPositionUnlocked);
            CounterPositions[availableCounterIndex].ActiveCustomer.UpdateDestination(CounterPositions[availableCounterIndex].Position);
            CustomersInQueue.RemoveAt(0);
        }
    }

    //Called from the AI script, when their transaction is complete.
    public void CompleteActiveTransaction(AI bot)
    {
        bot.UpdateDestination(LeaveCounterWaypoint);

        //Only using for loop to store index.
        for (int i = 0; i < CounterPositions.Count; i++)
        {
            if (CounterPositions[i].ActiveCustomer == bot)
                CounterPositions[i] = new TransactionCounterPosition(CounterPositions[i].Position, null, CounterPositions[i].ActiveCustomer);
        }

    }


    //Called when a new customer is spawned, and the queue isn't already full.
    public void AddCustomerToQueue(AI customer)
    {
        //If the queue is full, don't add.
        if (CustomersInQueue.Count > 3)
        {
            Debug.LogError("Trying to add new customer to queue when queue is full, deleting customer");
            return;
        }
        
        //Add the new customer to the list.
        CustomersInQueue.Add(customer);

        //Set the next open position to the next empty queue position.
        customer.UpdateDestination(QueuePositions[CustomersInQueue.Count -1]);
    }
    
    //Returns how many people the AIManager should spawn maximum.
    public int GetStoreCapacityLevel()
    {
        int availableCounters = 0;
        
        for (int i = 0; i < CounterPositions.Count; i++)
        {
            if (CounterPositions[i].IsPositionUnlocked)
                availableCounters++;
        }

        return 3 + availableCounters;
    }
    
    
}

[Serializable]
public struct TransactionCounterPosition
{
    public Transform Position;
    public AI ActiveCustomer;
    public bool IsPositionUnlocked;

    public TransactionCounterPosition(Transform pos, AI customer, bool isPosUnlocked)
    {
        Position = pos;
        ActiveCustomer = customer;
        IsPositionUnlocked = isPosUnlocked;
    }
}
