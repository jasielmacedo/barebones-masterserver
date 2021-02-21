#if MIRROR
using UnityEngine;
using Mirror;

/// <summary>
/// This script represents the changes that you will need to do in your custom
/// network manager
/// </summary>
public class ModifiedNetworkManager : NetworkManager
{
    // Set this in the inspector
    public MirrorGameRoom GameRoom;

    public override void Awake()
    {
        base.Awake();

        if (GameRoom == null)
        {
            Debug.LogError("Game Room property is not set on NetworkManager");
            return;
        }

        // Subscribe to events
        GameRoom.PlayerJoined += OnPlayerJoined;
        GameRoom.PlayerLeft += OnPlayerLeft;
    }

    private void OnPlayerJoined(NetMsfPlayer player)
    {
        var playerGameObject = new GameObject();
        NetworkServer.AddPlayerForConnection(player.Connection, playerGameObject);
    }

    private void OnPlayerLeft(NetMsfPlayer player)
    {
    }
    
}
#endif