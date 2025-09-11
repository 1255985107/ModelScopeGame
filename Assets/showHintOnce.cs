using UnityEngine;
using System.Collections;

public class ShowHintOnce : MonoBehaviour
{
    public GameObject panel;    // �� Panel ����
    public float delay = 4f;    // �ӳ�����
    private bool hasPressedE = false;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false); // һ��ʼ����

        StartCoroutine(ShowHintAfterDelay());
    }

    void Update()
    {
        if (!hasPressedE && Input.GetKeyDown(KeyCode.E))
        {
            hasPressedE = true;
            if (panel != null)
                panel.SetActive(false); // ���� E ������
        }
    }

    IEnumerator ShowHintAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (!hasPressedE && panel != null)
            panel.SetActive(true); // �ӳٺ���ʾ
    }
}