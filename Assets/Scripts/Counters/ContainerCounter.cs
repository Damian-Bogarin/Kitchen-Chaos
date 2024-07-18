using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField]
    private KitchenObjectSO kitchenObjectSO;

    public event EventHandler OnPlayerGrabbedObject;


    public override void Interact(Player player)
    {


        if (!player.HasKitchenObject())
        {
            // player no tiene nada
            KitchenObject.SpawnKitchenObject(kitchenObjectSO,player);

            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
       


    }


    
}
