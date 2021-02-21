using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        var player = Instantiate(playerPrefab);

        var customPlayer = playerPrefab.GetComponent<CustomPlayer>();
        customPlayer.UsernameMesh.text = "Random" + (int) (Random.value*100);

        NetworkServer.AddPlayerForConnection(conn, player.gameObject, customPlayer.netIdentity.assetId);

    }
}
