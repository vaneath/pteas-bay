using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour {


    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;


    public static DeliveryManager Instance { get; private set; }


    [SerializeField] private RecipeListSO recipeListSO;
    [SerializeField] private CustomerListSO customerListSO;
    [SerializeField] private DeliveryManagerUI deliveryManagerUI;


    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 3;
    private int successfulRecipesAmount;
    private bool firstRecipeSpawned = false;

    public bool isEnableRecipeTimer = false;

    private void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update() {
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = firstRecipeSpawned ? UnityEngine.Random.Range(10f,15f) : spawnRecipeTimerMax;

            if ((KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax) || (waitingRecipeSOList.Count == 0 && firstRecipeSpawned)) {
                if (!firstRecipeSpawned)
                {
                    firstRecipeSpawned = true;
                    spawnRecipeTimer = UnityEngine.Random.Range(10f, 15f);
                }

                RecipeSO originalRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                RecipeSO newRecipeSO = ScriptableObject.CreateInstance<RecipeSO>(); // Create a new instance

                newRecipeSO.recipeName = originalRecipeSO.recipeName;
                newRecipeSO.kitchenObjectSOList = new List<KitchenObjectSO>(originalRecipeSO.kitchenObjectSOList); // Clone the list
                newRecipeSO.customer = customerListSO.customerSOList[UnityEngine.Random.Range(0, customerListSO.customerSOList.Count)];
                newRecipeSO.timerMax = UnityEngine.Random.Range(30f, 45f);
                newRecipeSO.timer = newRecipeSO.timerMax;

                waitingRecipeSOList.Add(newRecipeSO);
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }

        for (int i = waitingRecipeSOList.Count - 1; i >= 0; i--)
        {
            RecipeSO recipeSO = waitingRecipeSOList[i];
           if (isEnableRecipeTimer) {
                recipeSO.timer -= Time.deltaTime;
            }

            // Check if the recipe has expired
            if (recipeSO.timer <= 0f)
            {
                waitingRecipeSOList.RemoveAt(i); // Remove expired recipe
                KitchenGameManager.Instance.AddBonusTimeToTimer(-5f);
                OnRecipeFailed?.Invoke(this, EventArgs.Empty); // Notify that recipe has failed
            }
        }

        // Update the UI every frame to reflect the timers
        deliveryManagerUI.UpdateVisual(); // Call this every frame
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
                // Has the same number of ingredients
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    // Cycling through all ingredients in the Recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        // Cycling through all ingredients in the Plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
                            // Ingredient matches!
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound) {
                        // This Recipe ingredient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe) {
                    // Player delivered the correct recipe!

                    successfulRecipesAmount++;

                    waitingRecipeSOList.RemoveAt(i);

                    KitchenGameManager.Instance.AddBonusTimeToTimer(5f);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        // No matches found!
        // Player did not deliver a correct recipe
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount() {
        return successfulRecipesAmount;
    }

    public void enableRecipeTimer () {
        isEnableRecipeTimer = true;
    }

    public void disableRecipeTimer () {
        isEnableRecipeTimer = false;
    }

    public void setMaxWaitingRecipe (int amount) {
        waitingRecipesMax = amount;
    }
}
