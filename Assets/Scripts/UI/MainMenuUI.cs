using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {


    [SerializeField] private Button playButton;
    [SerializeField] private Button aboutUsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button optionsButton;


    private void Awake() {
        playButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        });
        aboutUsButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.AboutUsScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        optionsButton.onClick.AddListener(() => {
            Hide();
            OptionsUI.Instance.Show(Show);
        });

        Time.timeScale = 1f;
    }
    
    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
