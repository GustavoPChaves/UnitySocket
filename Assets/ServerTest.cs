using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

// State object for receiving data from remote device.  
public class StateObject {  
    // Client socket.  
    public Socket workSocket = null;  
    // Size of receive buffer.  
    public const int BufferSize = 256;  
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];  
    // Received data string.  
    public StringBuilder sb = new StringBuilder();  
}  

public class ServerTest : MonoBehaviour
{
    string message = "";
    private Thread clientReceiveThread, sendMessageThread; 
    void Start(){
        ConnectToTcpServer();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Return)){
            SendMessage("Vai");
        }
    }

    private void ConnectToTcpServer () { 		
        try {  			
            
            clientReceiveThread = new Thread (new ThreadStart(StartClient)); 			
            clientReceiveThread.IsBackground = true; 			
            clientReceiveThread.Start();  		
            } 		
        catch (Exception e) { 			
            Debug.Log("On client connect exception " + e); 		
        } 	
    } 
    private void SendMessage (string message) { 		
        try {  			
            this.message = message;
            sendMessageThread = new Thread (new ThreadStart(SendThread)); 			
            sendMessageThread.IsBackground = true; 			
            sendMessageThread.Start();  		
            } 		
        catch (Exception e) { 			
            Debug.Log("On client connect exception " + e); 		
        } 	
    } 
    void SendThread(){
        try{

  
            // Send test data to the remote device.  
            Send(client, message);  
            sendDone.WaitOne();  
  
            // Receive the response from the remote device.  
            Receive(client);  
            receiveDone.WaitOne();  
  
            // Write the response to the console.  
            Debug.Log(string.Format("Response received : {0}", response));  

            
            // Release the socket.  
            // client.Shutdown(SocketShutdown.Both);  
            // Debug.Log(client.Connected);
            // client.Close();  
            // Debug.Log(client.Connected);
  
        } catch(SocketException e){
            Debug.Log(e.ErrorCode.ToString());
            ConnectToTcpServer();
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        } 
    }
  
//public class AsynchronousClient {  
    // The port number for the remote device.  
    private const int port = 11000;  
  
    // ManualResetEvent instances signal completion.  
    private static ManualResetEvent connectDone =   
        new ManualResetEvent(false);  
    private static ManualResetEvent sendDone =   
        new ManualResetEvent(false);  
    private static ManualResetEvent receiveDone =   
        new ManualResetEvent(false);  
  
    // The response from the remote device.  
    private static String response = String.Empty;  
    private static IPHostEntry ipHostInfo;  
    private static IPAddress ipAddress;
    private static IPEndPoint remoteEP; 
    private static Socket client;  

    private void StartClient() {  
        // Connect to a remote device.  
        try {  
            // Establish the remote endpoint for the socket.  
            // The name of the   
            // remote device is "localhost".  
             ipHostInfo = Dns.GetHostEntry("GustavoChaves.local");  
             ipAddress =  ipHostInfo.AddressList[0];  //IPAddress.Parse("127.0.0.1");
            Debug.Log(ipAddress);
             remoteEP = new IPEndPoint(ipAddress, port);  
  
            // Create a TCP/IP socket.  
             client = new Socket(ipAddress.AddressFamily,  
                SocketType.Stream, ProtocolType.Tcp);  

            // Connect to the remote endpoint.  
            client.BeginConnect( remoteEP,   
                new AsyncCallback(ConnectCallback), client);  
            connectDone.WaitOne();  
  
            SendMessage("Teste");
        } catch(SocketException e){
            Debug.Log(e.ErrorCode.ToString());
            ConnectToTcpServer();
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        } 

    }  
  
    private static void ConnectCallback(IAsyncResult ar) {  
        try {  
            // Retrieve the socket from the state object.  
            Socket client = (Socket) ar.AsyncState;  
  
            // Complete the connection.  
            client.EndConnect(ar);  
  
            Debug.Log(string.Format("Socket connected to {0}",  
                client.RemoteEndPoint.ToString()));  
  
            // Signal that the connection has been made.  
            connectDone.Set();  
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        }  
    }  
  
    private static void Receive(Socket client) {  
        try {  
            // Create the state object.  
            StateObject state = new StateObject();  
            state.workSocket = client;  
  
            // Begin receiving the data from the remote device.  
            client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
                new AsyncCallback(ReceiveCallback), state);  
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        }  
    }  
  
    private static void ReceiveCallback( IAsyncResult ar ) {  
        try {  
            // Retrieve the state object and the client socket   
            // from the asynchronous state object.  
            StateObject state = (StateObject) ar.AsyncState;  
            Socket client = state.workSocket;  
  
            // Read data from the remote device.  
            int bytesRead = client.EndReceive(ar);  
  
            if (bytesRead > 0) {  
                // There might be more data, so store the data received so far.  
            state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));  
  
                // Get the rest of the data.  
                client.BeginReceive(state.buffer,0,StateObject.BufferSize,0,  
                    new AsyncCallback(ReceiveCallback), state);  
            } else {  
                // All the data has arrived; put it in response.  
                if (state.sb.Length > 1) {  
                    response = state.sb.ToString();  
                }  
                // Signal that all bytes have been received.  
                receiveDone.Set();  
            }  
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        }  
    }  
  
    private static void Send(Socket client, String data) {  
        // Convert the string data to byte data using ASCII encoding.  
        data = data + "<EOF>";
        byte[] byteData = Encoding.ASCII.GetBytes(data);  
  
        // Begin sending the data to the remote device.  
        client.BeginSend(byteData, 0, byteData.Length, 0,  
            new AsyncCallback(SendCallback), client);  
    }  
  
    private static void SendCallback(IAsyncResult ar) {  
        try {  
            // Retrieve the socket from the state object.  
            Socket client = (Socket) ar.AsyncState;  
  
            // Complete sending the data to the remote device.  
            int bytesSent = client.EndSend(ar);  
            Debug.Log(string.Format("Sent {0} bytes to server.", bytesSent));  
  
            // Signal that all bytes have been sent.  
            sendDone.Set();  
        } catch (Exception e) {  
            Debug.Log(e.ToString());  
        }  
    }  



}  
//}

//     #region private members 	
//     private TcpClient socketConnection; 	
//     private Thread clientReceiveThread; 	
// 	#endregion  	

//     // ManualResetEvent instances signal completion.  
//     private static ManualResetEvent connectDone =   
//         new ManualResetEvent(false);  
//     private static ManualResetEvent sendDone =   
//         new ManualResetEvent(false);  
//     private static ManualResetEvent receiveDone =   
//         new ManualResetEvent(false);  
// // Use this for initialization 	
//     void Start () {
//         ConnectToTcpServer();     
// 	}  	
// // Update is called once per frame
//     void Update () {         
//         if (Input.GetKeyDown(KeyCode.Space)) {             
//             //SendMessage();         
//             Send(socketConnection, "Funciona");
// 		}     
// 	}  	
//     /// <summary> 	
//     /// Setup socket connection. 	
//     /// </summary> 	
//     private void ConnectToTcpServer () { 		
//         try {  			
            
//             clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
//             clientReceiveThread.IsBackground = true; 			
//             clientReceiveThread.Start();  		
//             } 		
//         catch (Exception e) { 			
//             Debug.Log("On client connect exception " + e); 		
//         } 	
//     }  	
//     /// <summary> 	
//     /// Runs in background clientReceiveThread; Listens for incomming data. 	
//     /// </summary>     
//     private void ListenForData() { 		
//         try { 			
//             socketConnection = new TcpClient("127.0.0.1", 13000);  			
//             Byte[] bytes = new Byte[1024];             
//             while (true) { 				
//                 // Get a stream object for reading 				
//                 using (NetworkStream stream = socketConnection.GetStream()) { 					
//                     int length; 					
//                     // Read incomming stream into byte arrary. 					
//                     while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 						
//                         var incommingData = new byte[length]; 						
//                         Array.Copy(bytes, 0, incommingData, 0, length); 						
//                         // Convert byte array to string message. 						
//                         string serverMessage = Encoding.ASCII.GetString(incommingData); 						
//                         Debug.Log("server message received as: " + serverMessage); 					
//                     } 				
//                 } 			
//             }         
//         }         
//         catch (SocketException socketException) {             
//             Debug.Log("Socket exception: " + socketException);         
//         }     
//     }  	
//     /// <summary> 	
//     /// Send message to server using socket connection. 	
//     /// </summary> 	
//     private void SendMessage() {         
//         if (socketConnection == null) {             
//             return;         
//         }  		
//         try { 			
//             // Get a stream object for writing. 			
//             NetworkStream stream = socketConnection.GetStream(); 			
//             if (stream.CanWrite) {                 
//                 string clientMessage = "This is a message from one of your clients."; 				
//                 // Convert string message to byte array.                 
//                 byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage); 				
//                 // Write byte array to socketConnection stream.                 
//                 stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
//                 Debug.Log("Client sent his message - should be received by server");             
//             }         
//         } 		
//         catch (SocketException socketException) {             
//             Debug.Log("Socket exception: " + socketException);         
//         }     
// 	} 
//     private static void Send(Socket client, String data) {  
//         // Convert the string data to byte data using ASCII encoding.  
//         byte[] byteData = Encoding.ASCII.GetBytes(data);  
  
//         // Begin sending the data to the remote device.  
//         client.BeginSend(byteData, 0, byteData.Length, 0,  
//             new AsyncCallback(SendCallback), client);  
//     }  
//     private static void SendCallback(IAsyncResult ar) {  
//             try {  
//                 // Retrieve the socket from the state object.  
//                 Socket client = (Socket) ar.AsyncState;  
    
//                 // Complete sending the data to the remote device.  
//                 int bytesSent = client.EndSend(ar);  
//                 Debug.Log("Sent {0} bytes to server.", bytesSent);  
    
//                 // Signal that all bytes have been sent.  
//                 sendDone.Set();  
//             } catch (Exception e) {  
//                 Debug.Log(e.ToString());  
//             }  
//         } 
//}
