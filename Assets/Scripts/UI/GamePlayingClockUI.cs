using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour {


    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI bonusTimeText;

    private void Awake() {
        bonusTimeText.gameObject.SetActive(false);
    }

    private void Update() {
        timerImage.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
    }

    public void ShowBonusTime(float bonusAmount) {
        if (bonusAmount < 0) {
            bonusTimeText.text = $"-{bonusAmount}s!";
            bonusTimeText.gameObject.SetActive(true);
            bonusTimeText.color = Color.red;
        } else {
            bonusTimeText.text = $"+{bonusAmount}s!"; 
            bonusTimeText.gameObject.SetActive(true);
            bonusTimeText.color = Color.green; 
        }

        StartCoroutine(HideBonusTimeText());
    }

    private IEnumerator HideBonusTimeText() {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        bonusTimeText.gameObject.SetActive(false);
    }
}