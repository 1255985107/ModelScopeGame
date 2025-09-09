using UnityEngine;

public class ActivateMonstersTrigger : MonoBehaviour
{
    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是玩家进入，并且这个触发器尚未被激活过
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            // 标记为已触发，确保这个逻辑只执行一次
            hasBeenTriggered = true;

            //Debug.Log("玩家触发陷阱，所有怪物开始下落！");

            // 1. 在整个场景中，找到所有带 "Monster" 标签的游戏对象
            GameObject[] allMonsters = GameObject.FindGameObjectsWithTag("Monster");

            // 2. 遍历每一个找到的怪物
            foreach (GameObject monster in allMonsters)
            {
                // 3. 获取该怪物的刚体(Rigidbody2D)组件
                Rigidbody2D rb = monster.GetComponent<Rigidbody2D>();

                // 4. 如果找到了刚体，就将其类型从 'Kinematic' 改为 'Dynamic'
                if (rb != null)
                {
                    // 这个动作会立即“开启”怪物的重力，使其开始自由落体
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
            }

            // （可选）触发器完成任务后可以禁用自己，以节省一丁点性能
            gameObject.SetActive(false);
        }
    }
}