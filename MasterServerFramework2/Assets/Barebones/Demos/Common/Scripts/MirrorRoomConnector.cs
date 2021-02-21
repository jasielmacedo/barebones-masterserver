using System;
using System.Collections;
using Barebones.Logging;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MirrorRoomConnector : RoomConnector
{
    public HelpBox _header = new HelpBox()
    {
        Text = "This script handles room access, and tries to connect to Mirror game server " +
               "(by using Network Manager). It will be used when client receives an access to game.",
        Height = 50
    };

    /// <summary>
    ///     Log level of connector
    /// </summary>
    public LogLevel LogLevel = LogLevel.Warn;

    public NetworkManager NetworkManager;

    protected Coroutine WaitConnectionCoroutine;
    public BmLogger Logger = Msf.Create.Logger(typeof(MirrorRoomConnector).Name);

    public static RoomAccessPacket RoomAccess;

    [Tooltip("If we can't connect in the given amount of time, it will be considered a failed attempt to connect")]
    public float ConnectionTimeout = 5f;

    public bool SwitchScenesIfWrongScene = true;

    public SceneField ConnectionFailedScene;
    public SceneField DisconnectedScene;

    protected override void Awake()
    {
        base.Awake();
        Logger.LogLevel = LogLevel;
        NetworkManager = NetworkManager ?? FindObjectOfType<NetworkManager>();
    }

    protected virtual void Start()
    {
        // If we currently have a room access 
        // (it might have been set in a previous scene)
        if (RoomAccess != null)
        {
            if (SceneManager.GetActiveScene().name == RoomAccess.SceneName)
            {
                // If we're atthe correct scene
                ConnectToGame(RoomAccess);
            }
            else if (SwitchScenesIfWrongScene)
            {
                // Switch to correct scene 
                SceneManager.LoadScene(RoomAccess.SceneName);
            }
        }
    }

    public override void ConnectToGame(RoomAccessPacket access)
    {
        if (SwitchScenesIfWrongScene
            && SceneManager.GetActiveScene().name != access.SceneName)
        {
            // Save the access
            RoomAccess = access;

            // Switch to correct scene 
            SceneManager.LoadScene(access.SceneName);
            return;
        }

        NetworkManager = NetworkManager ?? FindObjectOfType<NetworkManager>();
        
        // Remove the data after 
        RoomAccess = null;

        // Just in case
        NetworkManager.maxConnections = 999;

        Logger.Debug("Trying to connect to server at address: " + access.RoomIp + ":" + access.RoomPort);

        if (!NetworkClient.isConnected)
        {
            // If we're not connected already
            NetworkManager.networkAddress = access.RoomIp;

            // getting the mirror's default transport layer
            var transport = NetworkManager.GetComponent<kcp2k.KcpTransport>();
            transport.Port = (ushort)access.RoomPort;

            NetworkManager.StartClient();
        }

        if (WaitConnectionCoroutine != null)
            StopCoroutine(WaitConnectionCoroutine);

        WaitConnectionCoroutine = StartCoroutine(WaitForConnection(access));
    }

    protected virtual void OnFailedToConnect()
    {
        if (ConnectionFailedScene != null)
            SceneManager.LoadScene(DisconnectedScene.SceneName);
    }

    public IEnumerator WaitForConnection(RoomAccessPacket access)
    {
        NetworkManager = NetworkManager ?? FindObjectOfType<NetworkManager>();
        var transport = NetworkManager.GetComponent<kcp2k.KcpTransport>();

        Logger.Debug("Connecting to game server... " + NetworkManager.networkAddress + ":" +
                     transport.Port);

        var timeUntilTimeout = ConnectionTimeout;

        // Wait until we connect
        while (!NetworkClient.isConnected)
        {
            yield return null;
            timeUntilTimeout -= Time.deltaTime;

            if (timeUntilTimeout < 0)
            {
                Logger.Warn("Client failed to connect to game server: " + access);
                OnFailedToConnect();
                yield break;
            }
        }

        Logger.Info("Connected to game server, about to send access");
        
        // Connected, send the token
        NetworkClient.connection.Send<TokenMessage>(new TokenMessage(access.Token));

        // While connected
        while (NetworkClient.isConnected)
            yield return null;

        // At this point, we're no longer connected
        if (DisconnectedScene.IsSet()) 
            SceneManager.LoadScene(DisconnectedScene.SceneName);
    }
}