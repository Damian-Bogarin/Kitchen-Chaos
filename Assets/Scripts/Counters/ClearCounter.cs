using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField]
    private KitchenObjectSO kitchenObjectSO;
  

  

    public override void Interact(Player player)
    {
        if (!HasKitchenObject()) // la mesa esta vacia
        {
            // esta vacio
            if (player.HasKitchenObject())
            {
                // player tiene algo y dejarlo en la mesa
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //player no tiene nada
            }
        }
        else // la mesa tiene un objeto
        {
            // el player tiene algo
            if (player.HasKitchenObject())
            {
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKO) )
                {
                   
                    if (plateKO.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                    GetKitchenObject().DestroySelf();
                    }
                }
                else // player tiene algo que no es un plato
                {
                    if (GetKitchenObject().TryGetPlate(out plateKO))
                    {
                        if (plateKO.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else // player no tiene nada
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }

    }

}
