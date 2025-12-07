using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Singleton;

    public List<string> levelNames;

    private int savedLevelIndex = 0, savedCheckpointIndex = 1;

    void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        LoadSavedCheckpoint();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }

    public void SetSavedCheckpoint(int levelIndex, int checkpointIndex)
    {
        if (levelIndex < savedLevelIndex) return;
        if (levelIndex == savedLevelIndex && checkpointIndex <= savedCheckpointIndex) return;
        savedLevelIndex = levelIndex;
        savedCheckpointIndex = checkpointIndex;
    }
    
    public void LoadSavedCheckpoint()
    {
        Debug.Log("Loading saved checkpoint: Level " + savedLevelIndex + ", Checkpoint " + savedCheckpointIndex);
        if(savedLevelIndex < 0 || savedLevelIndex >= levelNames.Count)
        {
            Debug.LogError("Invalid saved level index: " + savedLevelIndex);
            return;
        }

        // 注册回调，等场景加载完成再执行查找和移动
        SceneManager.sceneLoaded += OnSceneLoadedForCheckpoint;
        SceneManager.LoadScene(levelNames[savedLevelIndex]);
    }

    private void OnSceneLoadedForCheckpoint(Scene scene, LoadSceneMode mode)
    {
        // 取消订阅，避免重复调用
        SceneManager.sceneLoaded -= OnSceneLoadedForCheckpoint;

        // 等一帧确保所有 Start/初始化都完成，再去寻找 CheckpointLoader
        StartCoroutine(InvokeLoadCheckpointNextFrame());
    }

    private IEnumerator InvokeLoadCheckpointNextFrame()
    {
        yield return null; // 等待一帧

        CheckpointLoader loader = FindObjectOfType<CheckpointLoader>();
        if (loader != null)
        {
            loader.LoadCheckpoint(savedCheckpointIndex);
        }
        else
        {
            Debug.LogWarning("CheckpointLoader not found in loaded scene.");
        }
    }

}
