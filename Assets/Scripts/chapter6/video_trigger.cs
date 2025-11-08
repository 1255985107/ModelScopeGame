using UnityEngine;
using UnityEngine.Video; 

public class VideoTriggerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;


    private bool isPlayerInZone = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
          
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;

            // 确保 videoPlayer 已经链接
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop(); // 停止播放，屏幕会自动恢复显示封面
            }
        }
    }

    void Update()
    {
        // 如果玩家在区域内并按下了 E 键
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            // 确保 videoPlayer 已经链接
            if (videoPlayer == null)
            {
                Debug.LogError("VideoPlayer没有被指定！");
                return;
            }

            // 检查视频当前是否正在播放
            if (videoPlayer.isPlaying)
            {
                // 如果在播放，就停止它
                videoPlayer.Stop();
            }
            else
            {
                // 如果没在播放，就开始播放
                videoPlayer.Play();
            }
        }
    }
}