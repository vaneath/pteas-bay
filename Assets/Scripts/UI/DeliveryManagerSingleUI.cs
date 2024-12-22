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
    [SerializeField] private Image timerBar;


    private void Awake() {
        iconTemplate.gameObject.SetActive(false);
        customerTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSO(RecipeSO recipeSO) {
        recipeNameText.text = recipeSO.recipeName;
        //timerText.text = $"{recipeSO.timer:0.0}";

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

    public void UpdateTimer(float remainingTime, float totalTime)
    {
        // Calculate fill amount as a value between 0 and 1
        float fillAmount = remainingTime / totalTime;
        timerBar.fillAmount = fillAmount; // Update fill amount of the timer image

        // Change color based on fill amount
        if (fillAmount > 0.5f)
        {
            timerBar.color = Color.Lerp(Color.yellow, Color.green, (fillAmount - 0.5f) * 2); // Green to Yellow
        }
        else
        {
            timerBar.color = Color.Lerp(Color.red, Color.yellow, fillAmount * 2); // Yellow to Red
        }
    }
}