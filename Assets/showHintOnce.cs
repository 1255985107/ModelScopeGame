using UnityEngine;
using System.Collections;

public class ShowHintOnce : MonoBehaviour
{
    public GameObject panel;    // 拖 Panel 进来
    public float delay = 4f;    // 延迟秒数
    private bool hasPressedE = false;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false); // 一开始隐藏

        StartCoroutine(ShowHintAfterDelay());
    }

    void Update()
    {
        if (!hasPressedE && Input.GetKeyDown(KeyCode.E))
        {
            hasPressedE = true;
            if (panel != null)
                panel.SetActive(false); // 按下 E 后隐藏
        }
    }

    IEnumerator ShowHintAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (!hasPressedE && panel != null)
            panel.SetActive(true); // 延迟后显示
    }
}