using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTriggerTwoPlayers : MonoBehaviour
{
    [Header("场景设置")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private int nextSceneIndex = -1;

    [Header("触发玩家标签")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string player1Tag = "Player1";

    private bool playerInTrigger = false;
    private bool player1InTrigger = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) playerInTrigger = true;
        else if (other.CompareTag(player1Tag)) player1InTrigger = true;

        CheckAndLoadScene();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) playerInTrigger = false;
        else if (other.CompareTag(player1Tag)) player1InTrigger = false;
    }

    private void CheckAndLoadScene()
    {
        if (playerInTrigger && player1InTrigger)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
            else if (nextSceneIndex >= 0)
                SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
