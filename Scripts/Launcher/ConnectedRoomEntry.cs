using UnityEngine;
using UnityEngine.UI;

namespace WatStudios.DeepestDungeon.Launcher
{
    public class ConnectedRoomEntry : MonoBehaviour
    {
        public Text RoomNameText;
        public Text RoomPlayersText;

        private string _roomName;

        public void Initialize(string roomName, byte currentPlayers, byte maxPlayers)
        {
            _roomName = roomName;

            RoomNameText.text = _roomName;
            RoomPlayersText.text = $"Players: {currentPlayers} / {maxPlayers}";
        }
    }

}

