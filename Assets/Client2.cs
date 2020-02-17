using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;
using UnityEngine.UI;

public class Client2 : MonoBehaviour
{

    bool socketReady = false;
    TcpClient socket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;

    string host = "127.0.0.1";
    int port = 6321;

    public GameObject messagePanel;
    public InputField messageInputField;

    public GameObject CellPrefab;

    public string clientName;

    private string messageFromPlayer = "First Message";

    public void SetName(string name)
    {
        clientName = String.Copy(name);
    }

    public void ConectToServer()
    {
        if (socketReady)
            return;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e.Message);

        }
    }
    void CreateTextCell(string message)
    {
        var newCell = Instantiate(CellPrefab);
        newCell.GetComponent<Text>().text = String.Copy(message);
        newCell.transform.SetParent(messagePanel.transform);
    }
    // Start is called before the first frame update
    void Start()
    {
        //ConectToServer();
    }

    // Update is called once per frame
    void Update()
    {
        if(socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if(data != null)
                {
                    StartCoroutine(OnIncomingData(data));
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                messageFromPlayer = String.Copy(messageInputField.text);
                messageInputField.text = "";
                //CreateTextCell(messageFromPlayer);
                Send(messageFromPlayer);
            }
        }
    }

    IEnumerator OnIncomingData(string data)
    {
        print(data);
        if(data == "%NAME")
        {
            Send("&NAME|" + clientName);
            yield break;
        }
        else if(data.Contains("%PLAYER")){
            int playerNumber = Convert.ToInt32(data.Split('|')[1]);
            GameManager.Instance.SetPlayer(playerNumber);
            yield break;
        }
        else if(data.Contains("%OPONENT")){
            int playerNumber = Convert.ToInt32(data.Split('|')[1]);
            GameManager.Instance.SetOponent(playerNumber);
            yield break;
        }
        else if(data.Contains("&MOVE")){
            ReceivePlayerMove(data);
            yield break;
        }
        else if(data.Contains("REMOVE")){
            int playerNumber = Convert.ToInt32(data.Split('|')[1]);
            GameManager.Instance.RemovePlayer(playerNumber);
            yield break;
        }
        else if(data.Contains("TURN")){
            int playerNumber = Convert.ToInt32(data.Split('|')[1]);
            GameManager.Instance.SetTurn(playerNumber);
            yield break;
        }
        else if(data.Contains("WIN")){
            CreateTextCell(String.Format("Player: {0} won, press Esc to play again", data.Split('|')[1]));
            yield break;
        }
        CreateTextCell(data);
        yield break;
    }

    void Send(string data)
    {
        if (!socketReady) return;
        writer.WriteLine(data);
        writer.Flush();

    }

    void CloseSocket()
    {
        if (!socketReady) return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;

    }

    public void SendPlayerMove(int playerNumber, int playerUnit, Vector3 position){
        print(position);
        Send(String.Format("&MOVE|{0}|{1}|{2}|{3}", playerNumber, playerUnit, position.x, position.y));
    }

    public void ReceivePlayerMove(string data){
        int playerNumber = Convert.ToInt32(data.Split('|')[1]);
        int unitNumber = Convert.ToInt32(data.Split('|')[2]);
        float x = float.Parse((data.Split('|')[3]));
        float z = float.Parse((data.Split('|')[4]));

        GameManager.Instance.MoveOponent(playerNumber, unitNumber, x, z);
    }

    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void OnDisable()
    {
        CloseSocket();
    }

    public void Won(int playerNumber){
        Send("WIN|" +  clientName);
    }
}
