using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointLoader : MonoBehaviour
{
    public List<Transform> checkpoints;
    public PlayerController playerController;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void LoadCheckpoint(int checkpointIndex)
    {
        Debug.Log("Loading checkpoint: " + checkpointIndex);
        if (checkpointIndex < 0 || checkpointIndex >= checkpoints.Count)
        {
            Debug.LogError("Invalid checkpoint index: " + checkpointIndex);
            return;
        }
        Transform checkpoint = checkpoints[checkpointIndex];
        playerController.gameObject.transform.position = checkpoint.position;
        return;
    }
}
