using UnityEngine;
using System;

#if UNITY_EDITOR
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
#elif UNITY_ANDROID
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
#else
    using Windows.Networking;
    using Windows.Networking.Sockets;
    using System.Threading.Tasks;
    using Windows.Storage.Streams;
#endif

public class TCPClient : MonoBehaviour
{

    private String HOST_NAME = "192.168.137.1";
    private int PORT = 8080;

    private GameState _gameState = new GameState();

    // Use this for initialization
    void Start()
    {
        #if UNITY_EDITOR
            startUnityAndroidSocket();
        #elif UNITY_ANDROID
            startUnityAndroidSocket();
        #else
            startUWPSocket();
        #endif
    }


    // Update is called once per frame
    void Update()
    {
        if (_gameState.StateChanged)
        {
            //do something
        }
    }

    #region unity
    #if UNITY_EDITOR || UNITY_ANDROID
    private void startUnityAndroidSocket()
    {
        Thread th = new Thread(HandleConnectionAndNewGameStates);
        th.Start();
    }

    void HandleConnectionAndNewGameStates()
    {
        Console.WriteLine("Hello User!");
        Console.WriteLine("Connecting to Server...");
        TcpClient tcpClient = new TcpClient(HOST_NAME, PORT);
        Console.WriteLine("Connected!");

        NetworkStream stream = tcpClient.GetStream();

        Console.WriteLine("Waiting for player allocation...");
        byte[] rawResponse = new byte[tcpClient.ReceiveBufferSize];
        int responseSize = stream.Read(rawResponse, 0, rawResponse.Length);

        String responseMessage = System.Text.Encoding.ASCII.GetString(rawResponse, 0, responseSize);
        Console.WriteLine("You are player " + responseMessage + "!\n");

        Console.WriteLine("waiting for new gamestates");
        do
        {
            rawResponse = new byte[tcpClient.ReceiveBufferSize];
            responseSize = stream.Read(rawResponse, 0, rawResponse.Length);
            Console.WriteLine("new GameState received");

            responseMessage = System.Text.Encoding.ASCII.GetString(rawResponse, 0, responseSize);
        } while (!responseMessage.Equals("Finished"));



        Byte[] data = System.Text.Encoding.ASCII.GetBytes("hello server");
        stream.Write(data, 0, data.Length);


        stream.Close();
        tcpClient.Close();
        Console.Read();
    }
    #endif
    #endregion

    #region uwp
    #if !UNITY_EDITOR && !UNITY_ANDROID
    private async void startUWPSocket()  //maybe add async
    {
        StreamSocket socket = new StreamSocket();
        HostName hostName = new HostName(HOST_NAME);

        await socket.ConnectAsync(hostName, PORT.ToString());

        DataReader reader = new DataReader(socket.InputStream);
        reader.InputStreamOptions = InputStreamOptions.Partial;
        reader.ByteOrder = ByteOrder.LittleEndian;
        reader.UnicodeEncoding = UnicodeEncoding.Utf8;


        //DataWriter writer = new DataWriter(socket.OutputStream);
        //writer.ByteOrder = ByteOrder.LittleEndian;
        //writer.UnicodeEncoding = UnicodeEncoding.Utf8;
        //byte[] byteMessage = System.Text.Encoding.UTF8.GetBytes("hello from hololens");
        //writer.WriteBytes(byteMessage);

        await reader.LoadAsync(1024);

        String message = reader.ReadString(reader.UnconsumedBufferLength);
        System.Diagnostics.Debug.WriteLine("you are player " + message);

        while (true)
        {
            await reader.LoadAsync(1024);

            message = reader.ReadString(reader.UnconsumedBufferLength);
            System.Diagnostics.Debug.WriteLine("new message: " + message);
        }

    }
    #endif
    #endregion
}

public class GameState
{
    public Boolean StateChanged { get; set; }
}