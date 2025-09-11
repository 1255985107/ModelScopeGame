using UnityEngine;
using UnityEngine.SceneManagement; // 引入场景管理命名空间

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // 下一个场景的名字
    [SerializeField] private int nextSceneIndex = -1; // 或者用 Build Index

    private void OnTriggerEnter2D(Collider2D other) // 如果是3D改成 OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 确保只有玩家触发
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else if (nextSceneIndex >= 0)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }
}
