using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAfterWaitTime : MonoBehaviour
{
    public float waitTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, waitTime);
    }

}
