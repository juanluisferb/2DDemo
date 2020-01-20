using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossIsVulnerable : MonoBehaviour {


    private void OnEnable()
    {
        StartCoroutine(EndVulnerableTime());

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator EndVulnerableTime()
    {
        yield return new WaitForSeconds(1.0f);
    }
}
