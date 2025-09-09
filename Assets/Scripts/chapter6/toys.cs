using UnityEngine;

public class ShowImageOnTrigger : MonoBehaviour
{
    // 在编辑器里把你要显示的图片（或UI物体）拖进去
    public GameObject imageToShow;

    void Start()
    {
        // 游戏开始时，为了保险起见，先把它隐藏掉
        // 如果你希望一开始就是隐藏的，这行代码很有用
        if (imageToShow != null)
        {
            imageToShow.SetActive(false);
        }
    }

    // 当玩家进入触发区域时
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是不是玩家
        if (other.CompareTag("Player"))
        {
            // 直接显示图片
            if (imageToShow != null)
            {
                imageToShow.SetActive(true);
                Debug.Log("玩家进入，图片显示");
            }
        }
    }

    // 当玩家离开触发区域时
    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查是不是玩家
        if (other.CompareTag("Player"))
        {
            // 直接隐藏图片
            if (imageToShow != null)
            {
                imageToShow.SetActive(false);
                Debug.Log("玩家离开，图片消失");
            }
        }
    }
}