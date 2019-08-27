using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.AI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    
    private string clientName;
    private int portToConnect = 11000;
    private string password;
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    public InputField clientNameInputField;
    public InputField serverAddressInputField;
    public InputField passwordInputField;
    private CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();

    public GameObject messagePanel;
    public InputField messageInputField;

    public GameObject CellPrefab;

    private string messageFromPlayer = "First Message";

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        culture.NumberFormat.NumberDecimalSeparator = ".";
        ConnectToServerButton();

    }


    public bool ConnectToServer(string host, int port)
    {
        if (socketReady)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;
            Send(messageFromPlayer);
        }
        catch (Exception e)
        {
            Debug.Log("Socket error " + e.Message);
        }

        return socketReady;
    }

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                data = data.Replace("<EOF>","");
                CreateTextCell(data);
                Debug.Log(data );
            }
        }

        if(Input.GetKeyDown(KeyCode.Return)){
            messageFromPlayer = String.Copy(messageInputField.text);
            messageInputField.text = "";
            CreateTextCell(messageFromPlayer);
            ConnectToServerButton();
        }
    }
    void CreateTextCell(string message){
        var newCell = Instantiate(CellPrefab);
        newCell.GetComponent<Text>().text = String.Copy(message);
        newCell.transform.SetParent(messagePanel.transform);
    }
    // Sending message to the server
    public void Send(string data)
    {

    
        if (!socketReady)
            return;

        writer.WriteLine(data + "<EOF>");
        writer.Flush();
        Debug.Log(data);
    }
    private void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }


    public void ConnectToServerButton()
    {
        // password = passwordInputField.text;
        // clientName = clientNameInputField.text;
        CloseSocket();
        try
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("GustavoChaves.local");  
            IPAddress ipAddress =  ipHostInfo.AddressList[0];
            Debug.Log(ipAddress);
            ConnectToServer(ipAddress.ToString(), portToConnect);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

}
