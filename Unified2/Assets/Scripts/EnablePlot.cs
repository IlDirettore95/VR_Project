using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePlot : MonoBehaviour
{
    private LevelPlot1 plot;

    // Start is called before the first frame update
    void Start()
    {
        plot = GetComponent<LevelPlot1>();

        StartCoroutine(Enable());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Enable()
    {
        yield return new WaitForSeconds(2f);

        plot.enabled = true;
    }
}
