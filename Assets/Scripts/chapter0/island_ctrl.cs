using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class island_ctrl : MonoBehaviour
{
    public GameObject fullIsland;
    public GameObject fragmentsParent;
    public GameObject warningUI;
    public GameObject platform;

    private bool isBroken;
    private List<Rigidbody2D> fragmentRBs = new List<Rigidbody2D>();
    private List<float> fallSpeeds = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        //if (warningUI == null)
            warningUI.SetActive(false);

        foreach (Transform fragment in fragmentsParent.transform)
        {
            Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
            if (rb != null) {
                rb.simulated = false;
                rb.gravityScale = 0f;
                fragmentRBs.Add(rb);
            }
            
            fallSpeeds.Add(Random.Range(1f, 3f));
        }

        StartCoroutine(BreakIsland());
    }

    IEnumerator BreakIsland()
    {
        yield return new WaitForSeconds(2f);
        if (warningUI != null)
            warningUI.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (fullIsland != null)
        {
            Destroy(fullIsland);
            Destroy(platform);
        }
            

        for (int i = 0; i < fragmentRBs.Count; i++) {
            fragmentRBs[i].simulated = true;
            fragmentRBs[i].velocity = new Vector2(0, -fallSpeeds[i]);
        }

        isBroken = true;

        
    }

    // Update is called once per frame

    


}
