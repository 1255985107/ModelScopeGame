// SwitchTrigger.cs
using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    // 引用中央切换器
    public CharacterSwitcher switcher;

    // 这个触发器要激活哪个角色 
    public GameObject characterToActivate;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player"))
        {
            return;
        }

        if (switcher != null && characterToActivate != null)
        {
            switcher.ActivateCharacterGroup(characterToActivate);
            
            triggered = true;
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("触发器没有正确设置 Switcher 或 Character To Activate！");
        }
    }
}