using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogStarter : MonoBehaviour
{
    public DialogTrigger dialogTrigger;
    public float delayTime = 3.0f;

    public void StartDelayedDialog()
    {
        if (dialogTrigger != null)
        {
            StartCoroutine(TriggerAfterDelay(delayTime));
        }
        else
                    {
            Debug.LogWarning("DialogTrigger component is not assigned.");
        }

    }

    private IEnumerator TriggerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogTrigger.TriggerDialog();
        Debug.Log("Dialog triggered after delay.");
    }
    // Start is called before the first frame update
    void Start()
    {
       StartDelayedDialog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
