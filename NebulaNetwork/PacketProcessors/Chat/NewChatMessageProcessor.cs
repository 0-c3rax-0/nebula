﻿#region

using System;
using NebulaAPI.Packets;
using NebulaModel.Logger;
using NebulaModel.Networking;
using NebulaModel.Packets;
using NebulaModel.Packets.Chat;
using NebulaWorld;
using NebulaWorld.MonoBehaviours.Local.Chat;

#endregion

namespace NebulaNetwork.PacketProcessors.Chat;

[RegisterPacketProcessor]
internal class NewChatMessageProcessor : PacketProcessor<NewChatMessagePacket>
{
    protected override void ProcessPacket(NewChatMessagePacket packet, NebulaConnection conn)
    {
        if (ChatManager.Instance == null)
        {
            Log.Warn("Unable to process chat packet, chat window assets were not loaded properly");
            return;
        }

        if (IsHost)
        {
            var player = Multiplayer.Session.Network.PlayerManager?.GetPlayer(conn);
            Multiplayer.Session.Network.PlayerManager?.SendPacketToOtherPlayers(packet, player);
        }

        var sentAt = packet.SentAt == 0 ? DateTime.Now : DateTime.FromBinary(packet.SentAt);
        ChatManager.Instance.SendChatMessage(
            string.IsNullOrEmpty(packet.UserName)
                ? $"[{sentAt:HH:mm}] {packet.MessageText}"
                : $"[{sentAt:HH:mm}] [{packet.UserName}] : {packet.MessageText}", packet.MessageType);
    }
}
