using UnityEngine;
using UnityEngine.Video;

public class VideoTriggerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    // 保证只触发一次
    private bool hasPlayed = false;

    // 存储玩家对象引用，以便稍后把它"变回来"
    private GameObject playerObject;

    void Start()
    {
        if (videoPlayer != null)
        {
            // 订阅视频播放结束事件
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查条件：未播放过 且 碰到的是玩家
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;      // 标记为已触发
            playerObject = other.gameObject; // 记录玩家是谁

            // 1. 让玩家直接消失（停用物体）
            // 停用后，玩家看不见、脚本停止运行、物理碰撞失效
            playerObject.SetActive(false);

            // 2. 播放视频
            if (videoPlayer != null)
            {
                videoPlayer.Play();
            }
        }
    }

    // 视频播放结束时自动调用
    void OnVideoFinished(VideoPlayer vp)
    {
        // 1. 停止视频
        vp.Stop();

        // 2. 让玩家重新出现
        if (playerObject != null)
        {
            playerObject.SetActive(true);
        }

        // (可选) 如果你想播放完把触发器也销毁，可以加这句：
        // Destroy(gameObject); 
    }

    void OnDestroy()
    {
        // 移除事件监听，防止报错
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}