using UnityEngine;
using UnityEngine.SceneManagement; // ���볡�����������ռ�

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // ��һ������������
    [SerializeField] private int nextSceneIndex = -1; // ������ Build Index

    private void OnTriggerEnter2D(Collider2D other) // �����3D�ĳ� OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ȷ��ֻ����Ҵ���
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
