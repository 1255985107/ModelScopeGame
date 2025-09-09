using UnityEngine;
using System.Collections;

public class MovingBookPlatform : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("平台将向右移动的距离")]
    public float moveDistanceX = 5f;

    [Tooltip("平台将向上移动的距离")]
    public float moveDistanceY = 3f;

    [Tooltip("完成整个移动过程所需的时间（秒）")]
    public float moveDuration = 2f;

    [Tooltip("移动曲线的弧度高度，值越大，曲线越弯曲")]
    public float arcHeight = 2f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool hasBeenTriggered = false;

    // 当有物体进入触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进来的是否是Player，并且平台尚未被触发过
        //Debug.Log("触发器检测到碰撞体: " + other.name+ hasBeenTriggered);
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            // 标记为已触发，防止重复启动
            hasBeenTriggered = true;

            //Debug.Log("玩家已触发平台！");

            // 启动移动协程，并将玩家的Transform传递过去
            StartCoroutine(MoveRoutine(other.transform));
        }
    }

    // 负责平滑移动的协程
    private IEnumerator MoveRoutine(Transform playerTransform)
    {
        // --- 准备阶段 ---

        // 记录起始和目标位置
        startPosition = transform.position;
        endPosition = startPosition + new Vector3(moveDistanceX, moveDistanceY, 0);

        // 将玩家变成平台的子对象，这样玩家就会跟随平台移动
        playerTransform.parent = this.transform;

        // (可选) 在这里可以暂时禁用玩家的移动脚本，防止玩家在移动过程中自己乱动
        // playerTransform.GetComponent<PlayerController>().enabled = false;

        float elapsedTime = 0f;

        // --- 移动循环 ---

        while (elapsedTime < moveDuration)
        {
            // 累加时间
            elapsedTime += Time.deltaTime;

            // 计算当前进度（0到1之间）
            float progress = Mathf.Clamp01(elapsedTime / moveDuration);
            // 使用SmoothStep让移动的开始和结束更平滑
            float smoothedProgress = Mathf.SmoothStep(0, 1, progress);

            // --- 核心曲线计算 ---
            // 1. 计算线性的X和Y位置
            Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, smoothedProgress);

            // 2. 计算弧线的高度偏移
            // Mathf.Sin(progress * Mathf.PI) 会生成一个从0 -> 1 -> 0的平滑曲线
            float arc = arcHeight * Mathf.Sin(progress * Mathf.PI);

            // 3. 将弧线高度加到Y轴上
            currentPos.y += arc;

            // 应用最终计算出的位置
            transform.position = currentPos;

            // 等待下一帧
            yield return null;
        }

        // --- 清理阶段 ---

        // 确保平台精确地停在目标位置
        transform.position = endPosition;

        // 将玩家“释放”，解除父子关系
        playerTransform.parent = null;

        // (可选) 重新启用玩家的移动脚本
        // playerTransform.GetComponent<PlayerController>().enabled = true;
    }
}