using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Event001 : MonoBehaviour
{
    [Tooltip("Text")]
    [SerializeField] private TextMeshPro textMesh;

    // Start is called before the first frame update
    void Start()
    {
        if (textMesh != null)
        {
            textMesh.gameObject.SetActive(false);
        }
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
}
