using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinDetection : MonoBehaviour
{
    public int units  = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider coll){
        units++;
        if(units >= 10){
            GameManager.Instance.Won();
        }
    }

    void OnTriggerExit(Collider coll){
        units--;
    }
}
