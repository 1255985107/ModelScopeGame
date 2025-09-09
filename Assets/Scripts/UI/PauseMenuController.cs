using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 暂停菜单管理器 - 负责暂停菜单UI的具体行为
/// 这个脚本应该附加到暂停菜单的根GameObject上
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button settingsButton; // 可选的设置按钮

    [Header("Settings")]
    [SerializeField] private SettingsMenuManager settingsMenuManager; // 可选的设置菜单

    void Start()
    {
        SetupButtons();
    }

    void SetupButtons()
    {
        // 设置按钮监听器
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
    }

    void OnResumeClicked()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ResumeGame();
    }

    void OnRestartClicked()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.RestartLevel();
    }

    void OnMainMenuClicked()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ReturnToMainMenu();
    }

    void OnSettingsClicked()
    {
        if (settingsMenuManager != null)
            settingsMenuManager.OpenSettingsMenu();
    }

    /// <summary>
    /// 在菜单显示时调用，用于初始化状态
    /// </summary>
    public void OnMenuShown()
    {
        // 确保第一个按钮被选中（用于手柄导航）
        if (resumeButton != null)
            resumeButton.Select();
    }

    /// <summary>
    /// 在菜单隐藏时调用，用于清理状态
    /// </summary>
    public void OnMenuHidden()
    {
        // 清理选中状态
        if (resumeButton != null)
            resumeButton.OnDeselect(null);
    }
}
