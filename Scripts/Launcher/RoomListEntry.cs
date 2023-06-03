using UnityEngine;
using UnityEngine.UI;
using WatStudios.DeepestDungeon.Messaging;

namespace WatStudios.DeepestDungeon.Launcher
{
    public class RoomListEntry : MonoBehaviour
    {
        public Text RoomNameText;
        public Text RoomPlayersText;
        public Button JoinRoomButton;

        private Launcher _launcher;
        private string _roomName;

        public void Start()
        {
            JoinRoomButton.onClick.AddListener(() =>
            {
                MessageHub.SendMessage(MessageType.LauncherJoinRoom, _roomName);
            });

            _launcher = GetComponentInParent<Launcher>();
        }

        public void Initialize(string roomName, byte currentPlayers, byte maxPlayers)
        {
            _roomName = roomName;

            RoomNameText.text = roomName;
            RoomPlayersText.text = $"{currentPlayers} / {maxPlayers}";
        }

        public void LoadingBar()
        {
            _launcher.Loading.SetActive(true);
            _launcher.Rooms.SetActive(false);
        }
    }
}