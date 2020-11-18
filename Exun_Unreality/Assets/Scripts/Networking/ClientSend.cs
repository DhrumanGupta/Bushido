using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Game.Networking;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    #region Packets

    public static void WelcomeRecieved()
    {
        using (Packet _packet = new Packet((int) ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write("Berlm");
         
            SendTCPData(_packet);
        }
    }
    #endregion
}
