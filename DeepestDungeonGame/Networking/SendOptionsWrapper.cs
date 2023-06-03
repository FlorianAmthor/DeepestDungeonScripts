using ExitGames.Client.Photon;
using System;

[Serializable]
public class SendOptionsWrapper
{
    public static readonly SendOptions SendReliable;
    public static readonly SendOptions SendUnreliable;

    public DeliveryMode DeliveryMode;
    public bool Encrypt;
    public byte Channel;
    public bool Reliability { get; set; }
}
