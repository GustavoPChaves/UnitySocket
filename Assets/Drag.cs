using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {
    
    private Vector3 screenPoint; private Vector3 offset;

    bool firstMove = true;
    void Start () {

    }

    
    void Update () {
        
    }



    void OnMouseDown()
    {
        if(!GameManager.Instance.myTurn) return;

        if(firstMove){

            firstMove = false;
            if(GameManager.Instance.playerNumber == 0)
                GameManager.Instance.SendPlayerMove(transform.GetSiblingIndex(), transform.position);
            
        }
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset =  transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,screenPoint.z));
    }

    void OnMouseDrag()
    {
        if(!GameManager.Instance.myTurn) return;
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    void OnMouseUp(){
        if(!GameManager.Instance.myTurn) return;
        GameManager.Instance.SendPlayerMove(transform.GetSiblingIndex(), transform.localPosition);
    }

}