using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameUI;

    [Header("Menu Controllers (Optional)")]
    // 这些控制器是可选的，可以在Unity编辑器中设置
    // [SerializeField] private PauseMenuController pauseMenuController;
    // [SerializeField] private GameOverMenuController gameOverMenuController;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Game Over Menu Buttons")]
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private Button gameOverMainMenuButton;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // Game state
    private bool isPaused = false;
    private bool isGameOver = false;

    // Input
    private InputAction escapeAction;

    // Singleton pattern for easy access
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        SetupInputActions();
        SetupButtonListeners();
        InitializeUI();
    }

    void OnEnable()
    {
        if (escapeAction != null)
            escapeAction.Enable();
    }

    void OnDisable()
    {
        if (escapeAction != null)
            escapeAction.Disable();
    }

    void SetupInputActions()
    {
        // 尝试使用 KeymapManager 如果可用
        if (KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady)
        {
            // 我们将在 Update 中检查 KeymapManager
            Debug.Log("KeymapManager is available, will check for ESC input in Update");
        }
        else
        {
            // 备用方案：直接创建ESC输入动作
            escapeAction = new InputAction(binding: "<Keyboard>/escape");
            escapeAction.performed += OnEscapePressed;
            escapeAction.Enable();
            Debug.Log("Using fallback ESC input action");
        }
    }

    void SetupButtonListeners()
    {
        // 暂停菜单按钮
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        // 游戏结束菜单按钮
        if (gameOverRestartButton != null)
            gameOverRestartButton.onClick.AddListener(RestartLevel);

        if (gameOverMainMenuButton != null)
            gameOverMainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    void InitializeUI()
    {
        // 初始化时隐藏所有菜单
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gameUI != null)
            gameUI.SetActive(true);

        // 确保游戏正常运行
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
    }

    void Update()
    {
        // 检测 ESC 键输入
        CheckForEscapeInput();
    }

    void CheckForEscapeInput()
    {
        bool escPressed = false;

        // 尝试使用 KeymapManager
        if (KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady)
        {
            escPressed = KeymapManager.Singleton.IsKeyPressed(KeymapManager.Function.ExitMenu);
        }
        // 备用方案：检测系统输入
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            escPressed = true;
        }

        if (escPressed)
        {
            HandleEscapeInput();
        }
    }

    void OnEscapePressed(InputAction.CallbackContext context)
    {
        HandleEscapeInput();
    }

    void HandleEscapeInput()
    {
        // 只有在游戏运行时才能暂停
        if (!isGameOver)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    #region Public Methods

    /// <summary>
    /// 暂停游戏并显示暂停菜单
    /// </summary>
    public void PauseGame()
    {
        if (isGameOver) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
            // 发送消息通知暂停菜单已显示（如果有监听器）
            pauseMenuPanel.SendMessage("OnMenuShown", SendMessageOptions.DontRequireReceiver);
        }

        if (gameUI != null)
            gameUI.SetActive(false);

        Debug.Log("Game Paused");
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
        {
            // 发送消息通知暂停菜单将被隐藏（如果有监听器）
            pauseMenuPanel.SendMessage("OnMenuHidden", SendMessageOptions.DontRequireReceiver);
            pauseMenuPanel.SetActive(false);
        }

        if (gameUI != null)
            gameUI.SetActive(true);

        Debug.Log("Game Resumed");
    }

    /// <summary>
    /// 重新开始当前关卡
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;

        // 重新加载当前场景
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);

        Debug.Log("Level Restarted");
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;

        // 加载主菜单场景
        SceneManager.LoadScene(mainMenuSceneName);

        Debug.Log("Returning to Main Menu");
    }

    /// <summary>
    /// 显示游戏结束界面
    /// </summary>
    /// <param name="title">游戏结束标题</param>
    /// <param name="message">游戏结束消息</param>
    /// <param name="score">可选的分数</param>
    public void ShowGameOver(string title = null, string message = null, int score = -1)
    {
        isGameOver = true;
        isPaused = false;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            // 发送消息通知游戏结束菜单已显示（如果有监听器）
            gameOverPanel.SendMessage("ShowGameOver", new object[] { title, message, score }, SendMessageOptions.DontRequireReceiver);
        }

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SendMessage("OnMenuHidden", SendMessageOptions.DontRequireReceiver);
            pauseMenuPanel.SetActive(false);
        }

        if (gameUI != null)
            gameUI.SetActive(false);

        Debug.Log("Game Over");
    }

    /// <summary>
    /// 隐藏游戏结束界面
    /// </summary>
    public void HideGameOver()
    {
        isGameOver = false;
        Time.timeScale = 1f;

        if (gameOverPanel != null)
        {
            // 发送消息通知游戏结束菜单将被隐藏（如果有监听器）
            gameOverPanel.SendMessage("OnMenuHidden", SendMessageOptions.DontRequireReceiver);
            gameOverPanel.SetActive(false);
        }

        if (gameUI != null)
            gameUI.SetActive(true);

        Debug.Log("Game Over Hidden");
    }

    #endregion

    #region Properties

    public bool IsPaused => isPaused;
    public bool IsGameOver => isGameOver;

    #endregion

    void OnDestroy()
    {
        // 清理输入动作
        if (escapeAction != null)
        {
            escapeAction.performed -= OnEscapePressed;
            escapeAction.Disable();
            escapeAction.Dispose();
        }

        // 确保时间恢复正常
        Time.timeScale = 1f;

        if (Instance == this)
            Instance = null;
    }
}
