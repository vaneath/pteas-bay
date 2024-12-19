using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeamMembersUI : MonoBehaviour {
    public static TeamMembersUI Instance { get; private set; }

    [SerializeField] private Button backButton;

    private void Awake() {
        Instance = this;

        backButton.onClick.AddListener(() => {
            Hide();
            AboutUsUI.Instance.Show();
        });

        Time.timeScale = 1f;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        Hide();
    }
}