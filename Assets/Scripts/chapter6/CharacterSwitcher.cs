// CharacterSwitcher.cs
using UnityEngine;
using System.Collections.Generic;
//using System.Diagnostics;

[System.Serializable]
public class CharacterGroup
{
    public string groupName; 
    public List<GameObject> charactersInGroup; // 这个组里包含的所有角色
}

public class CharacterSwitcher : MonoBehaviour
{
    // 在 Inspector 中指定初始玩家
    public GameObject initialPlayer;

    public List<CharacterGroup> characterGroups;

    void Start()
    {
        // 游戏开始时，禁用所有组里所有角色的 PlayerController
        foreach (CharacterGroup group in characterGroups)
        {
            foreach (GameObject character in group.charactersInGroup)
            {
                var controller = character.GetComponent<PlayerController>();
                if (controller != null)
                {
                    controller.enabled = false;
                    Debug.LogWarning("已禁用: " + character.name);
                }
                else {
                    Debug.LogWarning(character.name + " 没有 PlayerController 组件！");
                }
            }
        }

        // 确保初始玩家的控制器是启用的
        if (initialPlayer != null)
        {
            var controller = initialPlayer.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.enabled = true;
            }

            else
            {
                Debug.LogWarning(initialPlayer.name + " 没有 PlayerController 组件！");
            }
        }
        else
        {
            Debug.LogWarning("没有指定初始玩家！");
        }
    }

    public void ActivateCharacterGroup(GameObject triggerCharacter)
    {
        // 1. 遍历我们所有的角色组
        foreach (CharacterGroup group in characterGroups)
        {
            // 2. 检查当前这个组里是否包含我们传入的那个“关键”角色
            if (group.charactersInGroup.Contains(triggerCharacter))
            {
                Debug.Log("找到了包含 " + triggerCharacter.name + " 的组: " + group.groupName + "。正在激活整个组...");
                
                // 3. 如果找到了，就遍历这个组里的每一个角色，并启用他们的控制器
                foreach (GameObject characterToActivate in group.charactersInGroup)
                {
                    var controller = characterToActivate.GetComponent<PlayerController>();
                    if (controller != null)
                    {
                        controller.enabled = true;
                        Debug.Log("已激活: " + characterToActivate.name);
                    }
                }
                
                // 4. 任务完成，退出循环
                return; 
            }
        }

        // 如果循环结束都没找到，说明触发器设置有误
        Debug.LogWarning("没有找到任何包含 " + triggerCharacter.name + " 的角色组！");
    }
}