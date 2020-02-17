using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEnemies : MonoBehaviour
{
    public static bool canDetect = false;

    public int enemiesNear = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canDetect)
            if (enemiesNear >= 6)
            {
                GameManager.Instance.playerUnits--;
                transform.localPosition = new Vector3(-999, -999, -999);
                GameManager.Instance.SendEnemieMove(transform.GetSiblingIndex(), new Vector3(-999, -999, -999));
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        enemiesNear++;
    }

    private void OnTriggerExit(Collider other)
    {
   
        enemiesNear--;
        if (enemiesNear < 0) enemiesNear = 0;
    }
}
