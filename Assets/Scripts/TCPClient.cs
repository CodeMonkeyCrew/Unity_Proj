using UnityEngine;
using System;
using Assets.Scripts;


#if UNITY_EDITOR || UNITY_ANDROID
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
    private const String HOST_NAME = "192.168.1.19";
    private const int PORT = 8080;

    private GameState _gameState;
    private HUD _HUD;

    // Use this for initialization
    void Start()
    {
        _gameState = new GameState();
        _HUD = new HUD();
        
        //build up a connection to the server, depending from what platform the game is started
        #if UNITY_EDITOR
            Screen.orientation = ScreenOrientation.Landscape;
            StartUnityAndroidSocket();
        #elif UNITY_ANDROID
            Screen.orientation = ScreenOrientation.Landscape;
            StartUnityAndroidSocket();
        #else
            StartUWPSocket();
        #endif
    }


    // Update is called once per frame
    void Update()
    {
        //whenever a new GameState is received update the HUD        
        if (_gameState.StateChanged)
        {
            _HUD.Update(_gameState);
            _gameState.StateChanged = false;
        }
    }

    #region unity & andoid
    #if UNITY_EDITOR || UNITY_ANDROID
    private void StartUnityAndroidSocket()
    {
        Thread th = new Thread(HandleConnectionAndNewGameStates);
        th.Start();
    }

    private void HandleConnectionAndNewGameStates()
    {
        //build up connection
        TcpClient tcpClient = new TcpClient(HOST_NAME, PORT);
        NetworkStream stream = tcpClient.GetStream();
        tcpClient.ReceiveBufferSize = 5;

        byte[] rawResponse;
        int responseSize;

        //wait for response to get which player i am
        rawResponse = new byte[tcpClient.ReceiveBufferSize];
        responseSize = stream.Read(rawResponse, 0, rawResponse.Length);
        String responseMessage = System.Text.Encoding.ASCII.GetString(rawResponse, 0, responseSize);
        int playerNo;
        if (int.TryParse(responseMessage, out playerNo))
        {
            _gameState.PlayerNo = playerNo;
        }

        //wait for new gamestates and on receive update _gameState
        while(true)
        {
            rawResponse = new byte[tcpClient.ReceiveBufferSize];
            responseSize = stream.Read(rawResponse, 0, rawResponse.Length);
            _gameState.Update(rawResponse);
        };

        stream.Close();
        tcpClient.Close();
//        Console.Read();
    }
    #endif
    #endregion

    #region uwp
#if !UNITY_EDITOR && !UNITY_ANDROID
    private async void startUWPSocket()  
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

        //await reader.LoadAsync(1024);

        //String message = reader.ReadString(reader.UnconsumedBufferLength);
        //System.Diagnostics.Debug.WriteLine("you are player " + message);

        byte[] rawGameState;
        while (true)
        {
            rawGameState = new byte[2];
            await reader.LoadAsync(1024);
            //message = reader.ReadString(reader.UnconsumedBufferLength);
            reader.ReadBytes(rawGameState);
            _gameState.Update(rawGameState);
            //System.Diagnostics.Debug.WriteLine("new message: " + message);
        }

    }
    #endif
    #endregion
}

