using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnAmount;
    private int platesSpawnAmountMax = 4;

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if(spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;

            if(KitchenGameManager.Instance.IsGamePlaying() &&  platesSpawnAmount < platesSpawnAmountMax)
            {
                platesSpawnAmount++;
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);  
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if(platesSpawnAmount > 0)
            {
                if(platesSpawnAmount == platesSpawnAmountMax) { spawnPlateTimer = 0f; }
                platesSpawnAmount--;
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
                OnPlateRemoved?.Invoke(this,EventArgs.Empty);
            }
        }
    }

}
