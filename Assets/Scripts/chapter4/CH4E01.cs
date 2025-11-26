using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CH4E01 : MonoBehaviour
{
    [Tooltip("Text")]
    [SerializeField] private TextMeshPro textMesh;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform TransEndpoint;

    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private float arrivalThreshold = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        if (textMesh != null)
        {
            textMesh.gameObject.SetActive(false);
        }
        Debug.Assert(playerController != null, "PlayerController is not assigned in the inspector");
        Debug.Assert(TransEndpoint != null, "TransEndpoint is not assigned in the inspector");
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OpenText()
    {
        Debug.Log("Event001 triggered");
        if (textMesh != null)
        {
            textMesh.gameObject.SetActive(true);
        }
    }

    public void CloseText()
    {
        Debug.Log("Event001 closed");
        if (textMesh != null)
        {
            textMesh.gameObject.SetActive(false);
        }
    }

    public void Interact()
	{
		StartCoroutine(TeleportPlayer());
	}

    private IEnumerator TeleportPlayer()
    {
        yield return new WaitForSeconds(0.1f);

        while (Vector2.Distance(playerController.transform.position, TransEndpoint.position) > arrivalThreshold)
        {
            playerController.transform.position = Vector2.MoveTowards(
                playerController.transform.position,
                TransEndpoint.position,
                moveSpeed * 10f * Time.deltaTime);
            
            yield return null;
        }

        playerController.transform.position = TransEndpoint.position;
    }
}
