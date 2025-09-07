using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public LevelManager levelManager;
    public bool isWalkTrigger = false; // ����Ϊ��·����Ծ������
    public bool isJumpTrigger = false;
    public bool isFinalTrigger = false;
    private bool playerInZone = false;
    private bool tutorialComplete = false;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !tutorialComplete)
        {
            playerInZone = true;
            if (isWalkTrigger)
            {
                levelManager.OnlyWalkTrigger();
            }
            else if(isJumpTrigger)
            {
                levelManager.JumpTutorialTrigger();
            }
            else if(isFinalTrigger)
            {
                levelManager.FinalFallTrigger();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }

    void Update()
    {
        if (!playerInZone || tutorialComplete) return;

        if (isWalkTrigger)
        {
            // ��������ƶ�
            if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveLeft) || 
                KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveRight))
            {
                tutorialComplete = true;
                levelManager.OnTriggerZoneActivated();
            }
        }
        else if (isJumpTrigger)
        {
            // �����Ծ
            if (KeymapManager.Singleton.IsKeyPressed(KeymapManager.Function.MoveUp))
            {
                tutorialComplete = true;
                levelManager.OnTriggerZoneActivated();
            }
        }
    }
}