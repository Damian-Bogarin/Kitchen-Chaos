using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipesSO;
    [SerializeField] private BurningRecipeSO[] burningRecipesSO;

    private State state;
    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    private void Start()
    {
        state = State.Idle;
    }


    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new() { progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax });

                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {
                        fryingTimer = 0f;
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new() { progressNormalized = burningTimer / burningRecipeSO.burningTimerMax });

                    if (burningTimer > burningRecipeSO.burningTimerMax)
                    {
                        burningTimer = 0f;
                        Debug.Log("Fried!");
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                        OnProgressChanged?.Invoke(this, new() { progressNormalized = 0f });

                    }
                    break;
                case State.Burned:
                    break;
                default:
                    break;
            }

        }
      
    }



    public override void Interact(Player player)
    {
        if (!HasKitchenObject()) // la mesa esta vacia
        {
            // esta vacio
            if (player.HasKitchenObject())
            {
                // player tiene algo y dejarlo en la mesa
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Frying;
                    fryingTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });

                    OnProgressChanged?.Invoke(this, new() { progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax });
                }
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {

                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        state = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                        OnProgressChanged?.Invoke(this, new() { progressNormalized = 0f });
                    }
                }
            }
            else // player no tiene nada
            {
                GetKitchenObject().SetKitchenObjectParent(player);

                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                OnProgressChanged?.Invoke(this, new() { progressNormalized = 0f });
            }
        }
    }


    private bool HasRecipeWithInput(KitchenObjectSO inputKOSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKOSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutPutForInput(KitchenObjectSO inputKOSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKOSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else { return null; }
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKOSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipesSO)
        {
            if (fryingRecipeSO.input == inputKOSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKOSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipesSO)
        {
            if (burningRecipeSO.input == inputKOSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }


    public bool IsFried()
    {
        return state == State.Fried;
    }
}
