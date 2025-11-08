using UnityEngine;

public class ShowImageOnTrigger : MonoBehaviour
{
    public GameObject imageToShow;

    // 一个私有变量，用来跟踪玩家是否在触发区域内
    private bool isPlayerInZone = false;

    // 当有物体进入触发器时，这个函数会被调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的是否是玩家 (通过标签判断)
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            Debug.Log("玩家已进入区域");
        }
    }

    // 当有物体离开触发器时，这个函数会被调用
    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查离开的是否是玩家
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            // 当玩家离开时，总是隐藏图片，这是一个好的体验
            imageToShow.SetActive(false);
            Debug.Log("玩家已离开区域");
            // 你可以在这里隐藏 "按 E 交互" 的提示
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 检查玩家是否在区域内，并且按下了 E 键
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            // 在显示和隐藏之间切换
            // 如果图片当前是激活的，就隐藏它；如果是隐藏的，就显示它。
            bool isActive = imageToShow.activeSelf;
            imageToShow.SetActive(!isActive);

            Debug.Log("按下了E键，图片状态切换为：" + !isActive);
        }
    }
}
