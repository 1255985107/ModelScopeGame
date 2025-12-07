using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTimer : MonoBehaviour
{
    [SerializeField] Transform endpointA, endpointB;
    [SerializeField] PlayerController player;

    private float defaultFixedDeltaTime;

    void Awake()
    {
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // 等待一帧，避免被 UIManager 初始化重置
        yield return null;

        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
