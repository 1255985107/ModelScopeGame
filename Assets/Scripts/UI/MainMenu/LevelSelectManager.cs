using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [Tooltip("Main Menu Manager")]
    public MainMenuManager mainMenuManager;

    [Header("UI References")]
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private Button backButton;

    void Start()
    {
        // 绑定返回按钮（如果有）
        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseLevelSelectMenu);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    // 被主菜单调用以打开关卡选择
    public void OpenLevelSelectMenu()
    {
        if (levelSelectPanel == null)
        {
            Debug.LogWarning("LevelSelectPanel is not assigned.");
            return;
        }

        Debug.Log("Level Select Menu Opened");
        // 禁用主菜单上的按钮，避免在选择关卡时误触
        if (mainMenuManager != null)
            mainMenuManager.DisableButtons();

        levelSelectPanel.SetActive(true);
    }

    // 关闭关卡选择并恢复主菜单
    public void CloseLevelSelectMenu()
    {
        if (levelSelectPanel == null)
        {
            Debug.LogWarning("LevelSelectPanel is not assigned.");
            return;
        }

        levelSelectPanel.SetActive(false);

        if (mainMenuManager != null)
            mainMenuManager.EnableButtons();
    }

    // 切换开关
    public void ToggleLevelSelectMenu()
    {
        if (levelSelectPanel == null) return;

        if (levelSelectPanel.activeSelf)
            CloseLevelSelectMenu();
        else
            OpenLevelSelectMenu();
    }
}
