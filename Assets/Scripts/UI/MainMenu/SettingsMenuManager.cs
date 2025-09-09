using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.InputSystem;

public class SettingsMenuManager : MonoBehaviour
{
    [Tooltip("Main Menu Manager")]
    public MainMenuManager mainMenuManager;
    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button backButton;
    [SerializeField] private Button resetAllButton;
    [SerializeField] private Button applyButton;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private Toggle muteToggle;
    
    [Header("Display Settings")]
    [SerializeField] private Toggle fullscreenToggle;
    
    [Header("Key Binding Settings")]
    [SerializeField] private Transform keyBindingParent;
    [SerializeField] private GameObject keyBindingItemPrefab;
    [SerializeField] private Button resetKeysButton;
    
    [Header("Settings")]
    [SerializeField] private float defaultMasterVolume = 0.8f;
    [SerializeField] private float defaultMusicVolume = 0.8f;
    [SerializeField] private float defaultSFXVolume = 0.8f;

    // Audio mixer parameter names
    private const string MASTER_VOLUME_PARAM = "MasterVolume";
    private const string MUSIC_VOLUME_PARAM = "MusicVolume";
    private const string SFX_VOLUME_PARAM = "SFXVolume";
    
    // PlayerPrefs keys
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string MUTE_KEY = "Mute";
    private const string FULLSCREEN_KEY = "Fullscreen";
    private List<KeyBindingItem> keyBindingItems = new List<KeyBindingItem>();

    // Settings cache
    private float currentMasterVolume;
    private float currentMusicVolume;
    private float currentSFXVolume;
    private bool currentMute;
    private bool currentFullscreen;
    
    void Start()
    {
        InitializeSettings();
        SetupEventListeners();
        LoadSettings();
    }
    
    void InitializeSettings()
    {
        // Initialize key bindings
        SetupKeyBindings();
        
        // Set default values
        if (masterVolumeSlider != null) masterVolumeSlider.value = defaultMasterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = defaultMusicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = defaultSFXVolume;
        if (fullscreenToggle != null) fullscreenToggle.isOn = Screen.fullScreen;
    }
    
    void SetupEventListeners()
    {
        // Audio controls
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        if (muteToggle != null)
            muteToggle.onValueChanged.AddListener(OnMuteToggled);
        
        // Display controls
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        
        // Button controls
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);

        if (resetAllButton != null)
            resetAllButton.onClick.AddListener(OnResetAllButtonClicked);
        
        if (resetKeysButton != null)
            resetKeysButton.onClick.AddListener(OnResetKeysButtonClicked);
    }
    
    void SetupKeyBindings()
    {
        if (keyBindingParent == null || keyBindingItemPrefab == null) return;
        
        // Clear existing items
        // foreach (Transform child in keyBindingParent)
        // {
        //     Destroy(child.gameObject);
        // }
        // keyBindingItems.Clear();
        
        // Wait for KeymapManager to be ready
        StartCoroutine(SetupKeyBindingsCoroutine());
    }
    
    IEnumerator SetupKeyBindingsCoroutine()
    {
        // Wait for KeymapManager to be ready
        while (KeymapManager.Singleton == null || !KeymapManager.Singleton.IsReady)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Create key binding items
        KeymapManager.Function[] functions = KeymapManager.Singleton.GetAllFunctions();
        
        foreach (var function in functions)
        {
            GameObject item = Instantiate(keyBindingItemPrefab, keyBindingParent);
            KeyBindingItem keyBindingItem = item.GetComponent<KeyBindingItem>();
            
            if (keyBindingItem != null)
            {
                keyBindingItem.Initialize(function);
                keyBindingItems.Add(keyBindingItem);
            }
        }
    }

    #region Audio Methods

    void OnMasterVolumeChanged(float value)
    {
        currentMasterVolume = value;
        UpdateVolume(MASTER_VOLUME_PARAM, value);

        if (masterVolumeText != null)
            masterVolumeText.text = Mathf.RoundToInt(value * 100).ToString() + "%";
    }
    
    void OnMusicVolumeChanged(float value)
    {
        currentMusicVolume = value;
        UpdateVolume(MUSIC_VOLUME_PARAM, value);

        if (musicVolumeText != null)
            musicVolumeText.text = Mathf.RoundToInt(value * 100).ToString() + "%";
    }
    
    void OnSFXVolumeChanged(float value)
    {
        currentSFXVolume = value;
        UpdateVolume(SFX_VOLUME_PARAM, value);

        if (sfxVolumeText != null)
            sfxVolumeText.text = Mathf.RoundToInt(value * 100).ToString() + "%";
    }
    
    void OnMuteToggled(bool isMuted)
    {
        currentMute = isMuted;
        UpdateMute(isMuted);
    }

    void UpdateVolume(string param, float volume)
    {
        if (audioMixer != null)
        {
            // Convert 0-1 range to decibel range (-80 to 0)
            float dbValue = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
            audioMixer.SetFloat(param, dbValue);
        }
    }
    
    void UpdateMute(bool isMuted)
    {
        if (audioMixer != null)
        {
            if (isMuted)
            {
                audioMixer.SetFloat(MASTER_VOLUME_PARAM, -80f);
                audioMixer.SetFloat(MUSIC_VOLUME_PARAM, -80f);
                audioMixer.SetFloat(SFX_VOLUME_PARAM, -80f);
            }
            else
            {
                UpdateVolume(MASTER_VOLUME_PARAM, currentMasterVolume);
                UpdateVolume(MUSIC_VOLUME_PARAM, currentMusicVolume);
                UpdateVolume(SFX_VOLUME_PARAM, currentSFXVolume);
            }
        }
    }
    
    #endregion
    
    #region Display Methods
    
    void OnFullscreenToggled(bool isFullscreen)
    {
        currentFullscreen = isFullscreen;
        // Apply immediately for fullscreen
        Screen.fullScreen = isFullscreen;
    }
    
    #endregion
    
    #region Button Methods
    
    void OnBackButtonClicked()
    {
        SaveSettings();
        CloseSettingsMenu();
    }
    
    void OnResetAllButtonClicked()
    {
        ResetAllSettings();
    }
    
    void OnResetKeysButtonClicked()
    {
        ResetKeyBindings();
    }
    
    #endregion
    
    #region Settings Management
    
    void LoadSettings()
    {
        // Load audio settings
        currentMasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, defaultMasterVolume);
        currentMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume);
        currentSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultSFXVolume);
        currentMute = PlayerPrefs.GetInt(MUTE_KEY, 0) == 1;
        
        // Load display settings
        currentFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, Screen.fullScreen ? 1 : 0) == 1;
        
        // Apply loaded settings to UI
        if (masterVolumeSlider != null) masterVolumeSlider.value = currentMasterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = currentMusicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = currentSFXVolume;
        if (muteToggle != null) muteToggle.isOn = currentMute;
        if (fullscreenToggle != null) fullscreenToggle.isOn = currentFullscreen;
        
        // Apply audio settings
        UpdateVolume(MASTER_VOLUME_PARAM, currentMasterVolume);
        UpdateVolume(MUSIC_VOLUME_PARAM, currentMusicVolume);
        UpdateVolume(SFX_VOLUME_PARAM, currentSFXVolume);
        UpdateMute(currentMute);
        
        // Update text displays
        if (masterVolumeText != null)
            masterVolumeText.text = Mathf.RoundToInt(currentMasterVolume * 100).ToString() + "%";
        if (musicVolumeText != null)
            musicVolumeText.text = Mathf.RoundToInt(currentMusicVolume * 100).ToString() + "%";
        if (sfxVolumeText != null)
            sfxVolumeText.text = Mathf.RoundToInt(currentSFXVolume * 100).ToString() + "%";
    }
    
    void SaveSettings()
    {
        // Save audio settings
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, currentMasterVolume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, currentMusicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, currentSFXVolume);
        PlayerPrefs.SetInt(MUTE_KEY, currentMute ? 1 : 0);
        
        // Save display settings
        PlayerPrefs.SetInt(FULLSCREEN_KEY, currentFullscreen ? 1 : 0);
        
        PlayerPrefs.Save();
        
        // Save key bindings
        KeymapPrefManager keymapPrefManager = FindObjectOfType<KeymapPrefManager>();
        if (keymapPrefManager != null)
        {
            keymapPrefManager.SaveKeymapToPlayerPrefs();
        }
    }
    
    void ResetAllSettings()
    {
        // Reset audio settings
        currentMasterVolume = defaultMasterVolume;
        currentMusicVolume = defaultMusicVolume;
        currentSFXVolume = defaultSFXVolume;
        currentMute = false;
        
        // Reset display settings
        currentFullscreen = true;
        
        // Update UI
        if (masterVolumeSlider != null) masterVolumeSlider.value = currentMasterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = currentMusicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = currentSFXVolume;
        if (muteToggle != null) muteToggle.isOn = currentMute;
        if (fullscreenToggle != null) fullscreenToggle.isOn = currentFullscreen;
        
        // Apply settings
        UpdateVolume(MASTER_VOLUME_PARAM, currentMasterVolume);
        UpdateVolume(MUSIC_VOLUME_PARAM, currentMusicVolume);
        UpdateVolume(SFX_VOLUME_PARAM, currentSFXVolume);
        UpdateMute(currentMute);
        
        // Reset key bindings
        ResetKeyBindings();
        
        Debug.Log("All settings reset to default values");
    }
    
    void ResetKeyBindings()
    {
        if (KeymapManager.Singleton != null)
        {
            KeymapManager.Singleton.ResetKeys();
            
            // Refresh key binding UI
            foreach (var item in keyBindingItems)
            {
                if (item != null)
                {
                    item.RefreshDisplay();
                }
            }
            
            Debug.Log("Key bindings reset to default");
        }
    }
    
    #endregion
    
    #region Public Interface
    
    public void OpenSettingsMenu()
    {
        if (settingsPanel != null)
        {
            if (mainMenuManager != null)
                mainMenuManager.DisableButtons();
            gameObject.SetActive(true);
            LoadSettings();
        }
    }
    
    public void CloseSettingsMenu()
    {
        if (settingsPanel != null)
        {
            if (mainMenuManager != null)
                mainMenuManager.EnableButtons();
            gameObject.SetActive(false);
        }
    }
    
    public void ToggleSettingsMenu()
    {
        if (settingsPanel != null)
        {
            if (settingsPanel.activeSelf)
                CloseSettingsMenu();
            else
                OpenSettingsMenu();
        }
    }
    
    #endregion
    
    void OnDestroy()
    {
        SaveSettings();
    }
}
