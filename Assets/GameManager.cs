using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingletonClass<GameManager>
{

    public int playerNumber;

    public int playerUnits = 18;

    public bool myTurn = false;

    public List<GameObject> units;

    public Client2 client;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))  
            Reload();
    }

    public void SetPlayer(int playerNumber){
        this.playerNumber = playerNumber;
        if(playerNumber == 0) myTurn = true;
        units[playerNumber].SetActive(true);

        //for(var i = 0; i < units[playerNumber].transform.childCount; i++)
        //{
        //    units[playerNumber].transform.GetChild(i).gameObject.AddComponent<Drag>();
        //}
    }

    public void SetOponent(int oponent){
        units[oponent].SetActive(true);
    }

    public void MoveOponent(int playerNumber, int playerUnit, float x, float z){
        if(playerNumber == this.playerNumber) return;
        units[playerNumber].transform.GetChild(playerUnit).localPosition = new Vector3(x, z, 0.2175424f);
    }

    public void SendPlayerMove(int unitNumber, Vector3 position){
        client.SendPlayerMove(playerNumber, unitNumber, position);
    }

    public void RemovePlayer(int playerNumber){
        units[playerNumber].SetActive(false);
    }

    public void SetTurn(int playerNumber){
        if(playerNumber == this.playerNumber){
            myTurn = true;
        }else{
            myTurn = false;
        }
    }

    public void Reload(){
            SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex ) ;
    }

    public void Won(){
        client.Won(playerNumber);
    }
}
