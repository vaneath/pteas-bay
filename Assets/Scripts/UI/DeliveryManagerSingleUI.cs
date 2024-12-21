using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;
    [SerializeField] private Transform customerContainer;
    [SerializeField] private Transform customerTemplate;


    private void Awake() {
        iconTemplate.gameObject.SetActive(false);
        customerTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSO(RecipeSO recipeSO) {
        recipeNameText.text = recipeSO.recipeName;

        foreach (Transform child in iconContainer) {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList) {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }

        if (recipeSO.customer != null)
        {
            Transform customerTransform = Instantiate(customerTemplate, customerContainer);
            customerTransform.gameObject.SetActive(true);
            customerTransform.GetComponent<Image>().sprite = recipeSO.customer.customerSprite;                                                                   
        }
    }
}