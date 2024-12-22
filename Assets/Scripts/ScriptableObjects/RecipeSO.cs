using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject {


    public List<KitchenObjectSO> kitchenObjectSOList;
    public CustomerSO customer;
    public string recipeName;

    public float timerMax = 30f; // Total time for the recipe in seconds
    [HideInInspector] public float timer; // Remaining time for the recipe

}