using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 游戏结束菜单管理器 - 负责游戏结束UI的具体行为
/// 这个脚本应该附加到游戏结束菜单的根GameObject上
/// </summary>
public class GameOverMenuController : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Game Over Text")]
    [SerializeField] private TextMeshProUGUI gameOverTitle;
    [SerializeField] private TextMeshProUGUI gameOverMessage;

    [Header("Optional Elements")]
    [SerializeField] private GameObject scoreDisplay; // 可选的分数显示
    [SerializeField] private TextMeshProUGUI scoreText; // 分数文本

    void Start()
    {
        SetupButtons();
    }

    void SetupButtons()
    {
        // 设置按钮监听器
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
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

    /// <summary>
    /// 显示游戏结束菜单时调用，可以传递游戏结束的具体信息
    /// </summary>
    /// <param name="title">游戏结束标题</param>
    /// <param name="message">游戏结束消息</param>
    /// <param name="score">可选的分数</param>
    public void ShowGameOver(string title = null, string message = null, int score = -1)
    {

        // 设置分数
        if (score >= 0 && scoreText != null)
        {
            scoreText.text = $"分数: {score}";
            if (scoreDisplay != null)
                scoreDisplay.SetActive(true);
        }
        else if (scoreDisplay != null)
        {
            scoreDisplay.SetActive(false);
        }

        // 确保第一个按钮被选中（用于手柄导航）
        if (restartButton != null)
            restartButton.Select();
    }

    /// <summary>
    /// 隐藏游戏结束菜单时调用
    /// </summary>
    public void OnMenuHidden()
    {
        // 清理选中状态
        if (restartButton != null)
            restartButton.OnDeselect(null);
    }
}
