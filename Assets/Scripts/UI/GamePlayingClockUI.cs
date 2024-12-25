using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    public static GamePlayingClockUI Instance { get; private set; }

    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI bonusTimeText;
    private int currentScore;
    private int highScore;
    private const string HIGH_SCORE_KEY = "HighScore";
    private const int POINTS_PER_DELIVERY = 100;

    private void Awake()
    {
        Instance = this;
        bonusTimeText.gameObject.SetActive(false);
        LoadHighScore();
    }

    private void Update()
    {
        timerImage.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
    }

    public void ShowBonusTime(float bonusAmount)
    {
        if (bonusAmount < 0)
        {
            bonusTimeText.text = $"-{bonusAmount}s!";
            bonusTimeText.gameObject.SetActive(true);
            bonusTimeText.color = Color.red;
        }
        else
        {
            bonusTimeText.text = $"+{bonusAmount}s!";
            bonusTimeText.gameObject.SetActive(true);
            bonusTimeText.color = Color.green;
        }

        StartCoroutine(HideBonusTimeText());
    }

    private IEnumerator HideBonusTimeText()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        bonusTimeText.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
    }

    private void OnDestroy()
    {
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.OnRecipeSuccess -= DeliveryManager_OnRecipeSuccess;
        }
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        currentScore += POINTS_PER_DELIVERY;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
        }
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    public int GetCurrentScore() => currentScore;
    public int GetHighScore() => highScore;
}
