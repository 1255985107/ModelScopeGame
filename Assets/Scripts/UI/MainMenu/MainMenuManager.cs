using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;

    [Tooltip("Settings Menu Manager")]
    public SettingsMenuManager settingsMenuManager;

    [Tooltip("Start Game Scene Name")]
    public string startGameSceneName = "Level2";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(startButton != null, "Start Button is not assigned in the inspector");
        Debug.Assert(settingsButton != null, "Settings Button is not assigned in the inspector");
        Debug.Assert(quitButton != null, "Quit Button is not assigned in the inspector");
        Debug.Assert(settingsMenuManager != null, "SettingsMenuManager is not assigned in the inspector");

        startButton.onClick.AddListener(OnStartButtonClicked);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        Debug.Log("Listeners added to buttons");

        settingsMenuManager.CloseSettingsMenu();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnStartButtonClicked()
    {
        Debug.Log("Start Button Clicked");
        // Add logic to start the game
        SceneManager.LoadScene(startGameSceneName);
    }

    void OnSettingsButtonClicked()
    {
        Debug.Log("Settings Button Clicked");
        settingsMenuManager.OpenSettingsMenu();
    }

    void OnQuitButtonClicked()
    {
        Debug.Log("Quit Button Clicked");
        Application.Quit();
    }

    public void DisableButtons()
    {
        startButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    public void EnableButtons()
    {
        startButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }
}
