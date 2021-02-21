using System.Collections;
using Barebones.MasterServer;
using UnityEngine;
using Mirror;

public class NetMsfPlayer
{
    public NetworkConnection Connection { get; private set; }
    public PeerAccountInfoPacket AccountInfo { get; set; }

    public NetMsfPlayer(NetworkConnection connection, PeerAccountInfoPacket accountInfo)
    {
        Connection = connection;
        AccountInfo = accountInfo;
    }

    public string Username { get { return AccountInfo.Username; } }
    public int PeerId { get { return AccountInfo.PeerId; } }

}
