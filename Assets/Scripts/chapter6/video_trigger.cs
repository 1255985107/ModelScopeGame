using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoTriggerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    [SerializeField] string videoPath;

    // ��ֻ֤����һ��
    private bool hasPlayed = false;

    // �洢��Ҷ������ã��Ա��Ժ����"�����"
    private GameObject playerObject;

    void Start()
    {
        if (videoPlayer != null)
        {
            if (!string.IsNullOrEmpty(videoPath))
			{
				videoPlayer.source = VideoSource.Url;
                
                // 构建完整的StreamingAssets路径
                string fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoPath);
                videoPlayer.url = fullPath;
                Debug.Log("Trigger video path set to: " + fullPath);
			}
            // ������Ƶ���Ž����¼�
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���������δ���Ź� �� �����������
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;      // ���Ϊ�Ѵ���
            playerObject = other.gameObject; // ��¼�����˭

            // 1. �����ֱ����ʧ��ͣ�����壩
            // ͣ�ú���ҿ��������ű�ֹͣ���С�������ײʧЧ
            playerObject.SetActive(false);

            // 2. ������Ƶ
            if (videoPlayer != null)
            {
                Debug.Log("Playing video: " + videoPlayer.url);
                videoPlayer.Play();
            }
        }
    }

    // ��Ƶ���Ž���ʱ�Զ�����
    void OnVideoFinished(VideoPlayer vp)
    {
        // 1. ֹͣ��Ƶ
        vp.Stop();

        Debug.Log("Video finished playing.");

        // 2. ��������³���
        if (playerObject != null)
        {
            playerObject.SetActive(true);
        }

        // (��ѡ) ������벥����Ѵ�����Ҳ���٣����Լ���䣺
        // Destroy(gameObject); 
    }

    void OnDestroy()
    {
        // �Ƴ��¼���������ֹ����
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}