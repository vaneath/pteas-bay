using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AboutUsUI : MonoBehaviour {

    public static AboutUsUI Instance { get; private set; }

    [SerializeField] private Button backButton;
    [SerializeField] private Button moreButton;

    private void Awake() {
        Instance = this;

        backButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        moreButton.onClick.AddListener(() => {
            Hide();
            TeamMembersUI.Instance.Show();
        });

        Time.timeScale = 1f;
    }

    public void Show()
    {
        //this.onCloseButtonAction = onCloseButtonAction;

        gameObject.SetActive(true);

        //soundEffectsButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}