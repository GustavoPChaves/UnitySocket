using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;

public class Server : GenericSingletonClass<Server>
{
    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    public static int port = 6321;
    private TcpListener server;
    private bool serverStarted;

    private int currentPlayer = 0;


    protected override void SingletonAwake()
    {
        //#if UNITY_EDITOR
        DontDestroyOnLoad (this.gameObject);
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            StartListening();
            serverStarted = true;
            Debug.Log("Server Started on port: " + port.ToString());
        }
        catch(Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
        //#endif
    }

    void Update()
    {
        if (!serverStarted)
        {
            return;
        }

        foreach(var client in clients)
        {
            if (!IsConnected(client.tcp))
            {
                client.tcp.Close();
                disconnectList.Add(client);
                continue;
            }
            else
            {
                NetworkStream stream = client.tcp.GetStream();
                if (stream.DataAvailable)
                {
                    StreamReader reader = new StreamReader(stream, true);
                    string data = reader.ReadLine();

                    if(data != null)
                    {
                        OnIncomingData(client, data);
                    }
                }
            }
        }

        for(int i = 0; i < disconnectList.Count - 1; i++)
        {
            Broadcast(disconnectList[i].clientName + " Has Disconnected", clients);
            Broadcast("REMOVE|" + i.ToString(), clients);
            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
            
            if(clients. Count == 1){
                Broadcast("WIN|" + clients[0].clientName, clients);
                Broadcast("TURN|99", clients);
            }
        }
    }

    void Broadcast(string data, List<ServerClient> clients)
    {
        foreach(var client in clients)
        {
            try
            {
                StreamWriter writer = new StreamWriter(client.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
                writer.Flush();
            }
            catch(Exception e)
            {
                Debug.Log("Whrite error: " + e.Message + " to client: " + client.clientName);
            }
        }
    }
    void Broadcast(string data, ServerClient client)
    {

        try
        {
            StreamWriter writer = new StreamWriter(client.tcp.GetStream());
            writer.WriteLine(data);
            writer.Flush();
            writer.Flush();
        }
        catch (Exception e)
        {
            Debug.Log("Whrite error: " + e.Message + " to client: " + client.clientName);
        }

    }

    void OnIncomingData(ServerClient client, string data)
    {
        Debug.Log(data);
        if (data.Contains("&NAME"))
        {
            client.clientName = data.Split('|')[1];
            Broadcast(client.clientName + " Has Connected", clients);
            
            return;
        }
        if (data.Contains("MOVE")){
            Broadcast(data, clients);
            
            ControlTurn();
            return;
        }
        if (data.Contains("WIN")){
            Broadcast(data, clients);
            Broadcast("TURN|99", clients);

            return;
        }
        Broadcast(client.clientName + ": " + data, clients);
    }

    void ControlTurn(){
        currentPlayer++;
        if(currentPlayer >= clients.Count) currentPlayer = 0;
        Broadcast(clients[currentPlayer].clientName + " Turno", clients);
        Broadcast("TURN|" + currentPlayer.ToString(), clients);
        
            
    }

    void SetOponents(){
        for(var i = 0; i < clients.Count; i++){
            Broadcast("%OPONENT|" + i.ToString(), clients);
        }
    }

    bool IsConnected(TcpClient client)
    {
        try
        {
            if(client != null && client.Client != null && client.Client.Connected){
                if(client.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }
    void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();
        
        Broadcast("%NAME", clients[clients.Count - 1]);

        Broadcast("%PLAYER|" + (clients.Count - 1).ToString(), clients[clients.Count - 1]);

        SetOponents();


        //Broadcast(clients[clients.Count - 1].clientName + " Has Connected", clients);

    }

    void ResetGame(){
        currentPlayer = 0;
    }
}
public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}