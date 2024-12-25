using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDifficultyUI : MonoBehaviour
{


    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button startGameButton;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }


    private void Awake()
    {
        startGameButton.interactable = false;
        easyButton.onClick.AddListener(() => OnDifficultyButtonClick(Difficulty.Easy));
        mediumButton.onClick.AddListener(() => OnDifficultyButtonClick(Difficulty.Medium));
        hardButton.onClick.AddListener(() => OnDifficultyButtonClick(Difficulty.Hard));
        startGameButton.onClick.AddListener(() => OnStartButtonClick());
    }

    private void OnDifficultyButtonClick(Difficulty difficulty)
    {
        startGameButton.interactable = true;

        ColorBlock colors = easyButton.colors;
        colors.normalColor = Color.grey;
        easyButton.colors = colors;

        colors = mediumButton.colors;
        colors.normalColor = Color.grey;
        mediumButton.colors = colors;

        colors = hardButton.colors;
        colors.normalColor = Color.grey;
        hardButton.colors = colors;

        switch (difficulty)
        {
            case Difficulty.Easy:
                easyButton.colors = ChangeColorToSelected(easyButton.colors);
                DeliveryManager.Instance.disableRecipeTimer();
                break;
            case Difficulty.Medium:
                mediumButton.colors = ChangeColorToSelected(mediumButton.colors);
                DeliveryManager.Instance.enableRecipeTimer();
                break;
            case Difficulty.Hard:
                hardButton.colors = ChangeColorToSelected(hardButton.colors);
                DeliveryManager.Instance.enableRecipeTimer();
                DeliveryManager.Instance.setMaxWaitingRecipe(4);
                break;
        }

    }

    private ColorBlock ChangeColorToSelected(ColorBlock buttonColors)
    {
        buttonColors.normalColor = Color.green;
        buttonColors.highlightedColor = Color.yellow;
        return buttonColors;
    }

    private void OnStartButtonClick()
    {
        if (KitchenGameManager.Instance.IsGamePlaying() == false &&
            KitchenGameManager.Instance.IsCountdownToStartActive() == false)
        {
            KitchenGameManager.Instance.StartCountdownToStart();
        }
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnGameUnpaused += KitchenGameManager_OnGameUnpaused;
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        Show();
    }

    private void KitchenGameManager_OnGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);

        //resumeButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}