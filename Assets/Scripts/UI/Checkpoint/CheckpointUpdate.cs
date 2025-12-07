using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointUpdate : GenericTriggerZone
{
    [SerializeField] int chapter = 0, index = 0;
    // Start is called before the first frame update
    void Start()
    {
        requiresInput = false;
        onTriggerEnter.AddListener(UpdateCheckpoint);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void UpdateCheckpoint()
	{
		CheckpointManager.Singleton.SetSavedCheckpoint(chapter, index);
	}
}
