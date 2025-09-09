using UnityEngine;
using UnityEngine.UI; // 引入 UI 命名空间

public class HealthManager : MonoBehaviour
{
    // 注意：这里改成了 RawImage 数组
    public RawImage[] heartImages;

    [Header("Color Settings")]
    public Color fullHealthColor = new Color(1f, 1f, 1f, 1f); // 满血颜色 (原本样子)
    public Color emptyHealthColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // 空血颜色 (暗灰色且半透明)
    public Color damageFlashColor = new Color(1f, 0f, 0f, 1f); // 受伤闪烁颜色

    [Header("Flash Settings")]
    public float minFlashSpeed = 2.0f;
    public float maxFlashSpeed = 10.0f;

    private float targetHealth;

    void OnEnable()
    {
        Debug.Log("HealthManager: OnEnable called. Subscribing to PlayerHealth events.");
        PlayerHealth.onHealthChanged += OnHealthChanged;
    }

    void OnDisable()
    {
        PlayerHealth.onHealthChanged -= OnHealthChanged;
    }

    void OnHealthChanged(float current, float max)
    {
        Debug.Log($"HealthManager: OnHealthChanged received. Current: {current}, Max: {max}");
        targetHealth = current;
        UpdateHeartsVisuals();
    }

    void Update()
    {
        // 持续刷新以播放闪烁动画
        UpdateHeartsVisuals();
    }

void UpdateHeartsVisuals()
    {
        // ================================================================
        // 修改部分：定义每颗心代表多少血
        // 如果 1颗心 = 2血，这里就写 2
        // ================================================================
        float healthPerHeart = 2.0f; 

        // 把总血量换算成 "心的数量"
        // 例如：10.5 血 / 2 = 5.25 颗心
        float visibleHealth = targetHealth / healthPerHeart;

        int fullHearts = Mathf.FloorToInt(visibleHealth); // 5 (前5颗满)
        float fraction = visibleHealth - fullHearts;      // 0.25 (第6颗剩 25%)

        for (int i = 0; i < heartImages.Length; i++)
        {
            RawImage heartImg = heartImages[i];

            // 1. 这一格是满血 (索引 0~4)
            if (i < fullHearts)
            {
                heartImg.color = fullHealthColor;
            }
            // 2. 这一格正在被扣 (索引 5) -> 闪烁效果
            else if (i == fullHearts && fraction > 0f)
            {
                // 注意：这里 fraction 代表的是 "这颗心里剩多少比例"
                // 比如剩 0.25 (即0.5血)，闪烁速度会很快
                
                float currentFlashSpeed = Mathf.Lerp(maxFlashSpeed, minFlashSpeed, fraction);
                
                // 闪烁算法
                float alpha = (Mathf.Sin(Time.time * currentFlashSpeed) + 1f) / 2f;
                
                // 如果你想让闪烁时更明显，可以在 Empty 和 Full 之间闪，或者 Full 和 Red 之间闪
                heartImg.color = Color.Lerp(fullHealthColor, damageFlashColor, alpha);
            }
            // 3. 这一格是空的 (索引 6及以后)
            else
            {
                heartImg.color = emptyHealthColor;
            }
        }
    }
}