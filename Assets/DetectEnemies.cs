using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEnemies : MonoBehaviour
{

    public int enemiesNear = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        enemiesNear++;
        if (enemiesNear >= 3)
        {
            GameManager.Instance.playerUnits--;
            gameObject.SetActive(false);
            GameManager.Instance.SendPlayerMove(transform.GetSiblingIndex(), new Vector3(-999, -999, -999));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        enemiesNear--;
    }
}
