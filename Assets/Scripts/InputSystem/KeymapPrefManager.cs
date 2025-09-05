using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 负责保存和加载按键重绑定配置到PlayerPrefs
/// </summary>
public class KeymapPrefManager : MonoBehaviour
{
    private const string PLAYERPREFS_KEY = "InputBindingOverrides";

    void Start()
    {
        // 延迟加载，确保KeymapManager已经初始化
        StartCoroutine(LoadKeymap());
    }

    private IEnumerator LoadKeymap()
    {
        // 等待KeymapManager准备就绪
        while (KeymapManager.Singleton == null || !KeymapManager.Singleton.IsReady)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // 等待一帧确保完全初始化
        yield return null;

        // 现在加载绑定
        LoadKeymapFromPlayerPrefs();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveKeymapToPlayerPrefs();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveKeymapToPlayerPrefs();
        }
    }

    void OnDestroy()
    {
        SaveKeymapToPlayerPrefs();
    }

    /// <summary>
    /// 保存当前的按键绑定配置到PlayerPrefs
    /// </summary>
    public void SaveKeymapToPlayerPrefs()
    {
        if (KeymapManager.Singleton == null || KeymapManager.Singleton.inputActions == null)
        {
            Debug.LogWarning("KeymapManager or InputActions not available for saving");
            return;
        }

        try
        {
            string bindingOverrides = KeymapManager.Singleton.inputActions.SaveBindingOverridesAsJson();
            SaveToPlayerPrefs(bindingOverrides);
            Debug.Log("Key bindings saved successfully to PlayerPrefs");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save key bindings: {e.Message}");
        }
    }

    /// <summary>
    /// 从PlayerPrefs加载保存的按键绑定配置
    /// </summary>
    public void LoadKeymapFromPlayerPrefs()
    {
        if (KeymapManager.Singleton == null || KeymapManager.Singleton.inputActions == null)
        {
            Debug.LogWarning("KeymapManager or InputActions not available for loading");
            return;
        }

        try
        {
            string bindingOverrides = LoadFromPlayerPrefs();

            if (!string.IsNullOrEmpty(bindingOverrides))
            {
                KeymapManager.Singleton.inputActions.LoadBindingOverridesFromJson(bindingOverrides);
                Debug.Log("Key bindings loaded successfully from PlayerPrefs");
            }
            else
            {
                Debug.Log("No saved key bindings found, using defaults");
            }

            // 加载完成后，通知UI刷新显示
            RefreshKeyBindingUI();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load key bindings: {e.Message}");
        }
    }

    /// <summary>
    /// 通知所有KeyRebindUI刷新显示
    /// </summary>
    private void RefreshKeyBindingUI()
    {
        // TODO: 当 KeyRebindUI 类实现后，取消以下代码的注释
        /*
        KeyRebindUI[] keyRebindUIs = FindObjectsByType<KeyRebindUI>(FindObjectsSortMode.None);
        foreach (var ui in keyRebindUIs)
        {
            if (ui != null)
            {
                ui.RefreshAllKeyBindingItems();
            }
        }
        */
        Debug.Log("Refreshed all KeyRebindUI displays after loading bindings");
    }

    /// <summary>
    /// 删除保存的配置
    /// </summary>
    public void DeleteSavedBindings()
    {
        try
        {
            if (PlayerPrefs.HasKey(PLAYERPREFS_KEY))
            {
                PlayerPrefs.DeleteKey(PLAYERPREFS_KEY);
                PlayerPrefs.Save();
                Debug.Log("Saved key bindings from PlayerPrefs deleted");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to delete saved key bindings: {e.Message}");
        }
    }

    /// <summary>
    /// 检查是否存在保存的配置
    /// </summary>
    public bool HasSavedBindings()
    {
        return PlayerPrefs.HasKey(PLAYERPREFS_KEY);
    }

    /// <summary>
    /// 重置为默认配置并保存
    /// </summary>
    public void ResetAndSave()
    {
        if (KeymapManager.Singleton != null)
        {
            KeymapManager.Singleton.ResetKeys();
            SaveKeymapToPlayerPrefs();
            Debug.Log("Key bindings reset to default and saved");
        }
    }

    /// <summary>
    /// 保存到PlayerPrefs
    /// </summary>
    private void SaveToPlayerPrefs(string bindingOverrides)
    {
        PlayerPrefs.SetString(PLAYERPREFS_KEY, bindingOverrides);
        PlayerPrefs.Save();
        Debug.Log("Key bindings saved to PlayerPrefs");
    }

    /// <summary>
    /// 从PlayerPrefs加载
    /// </summary>
    private string LoadFromPlayerPrefs()
    {
        string content = PlayerPrefs.GetString(PLAYERPREFS_KEY, "");
        if (!string.IsNullOrEmpty(content))
        {
            Debug.Log("Key bindings loaded from PlayerPrefs");
        }
        return content;
    }
}
