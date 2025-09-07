using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public LevelManager levelManager;
    public bool isWalkTrigger = false; // ÉèÖÃÎª×ßÂ·»òÌøÔ¾´¥·¢Æ÷
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
            // ¼ì²â×óÓÒÒÆ¶¯
            if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveLeft) || 
                KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveRight))
            {
                tutorialComplete = true;
                levelManager.OnTriggerZoneActivated();
            }
        }
        else if (isJumpTrigger)
        {
            // ¼ì²âÌøÔ¾
            if (KeymapManager.Singleton.IsKeyPressed(KeymapManager.Function.MoveUp))
            {
                tutorialComplete = true;
                levelManager.OnTriggerZoneActivated();
            }
        }
    }
}