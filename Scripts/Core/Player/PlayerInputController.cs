using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using WatStudios.DeepestDungeon.Core.Interactables;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Networking;

namespace WatStudios.DeepestDungeon.Core.PlayerLogic
{
    public class PlayerInputController : MonoBehaviourPun
    {
        #region Private Fields
        private Quaternion _characterTargetRot;
        private Quaternion _cameraTargetRot;
        private Vector3 _localStartPosCamera;
        private NetworkPlayer _networkPlayer;
        private Camera _fpsCamera;
        private float _mouseSensitivity;
        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [Header("Mouse")]
        [SerializeField] private bool _invertXAxis;
        [SerializeField] private bool _invertYAxis;
        [SerializeField] private float _minimumX;
        [SerializeField] private float _maximumX;
        [Header("Interactable Configuration")]
        [SerializeField] private float _interactRange;
        [SerializeField] private LayerMask _interactLayerMask;
#pragma warning restore 649
        #endregion

        #region Public Fields
        internal FpsState PrevFpsState { get; private set; }
        internal FpsState FpsState { get; private set; }
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            if (!photonView.IsMine)
                return;
            _networkPlayer = GetComponent<NetworkPlayer>();
            _fpsCamera = _networkPlayer.FpsCamera;
            _characterTargetRot = transform.localRotation;
            _cameraTargetRot = _fpsCamera.transform.localRotation;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 0.1f);
        }

        private void Update()
        {
            if (!photonView.IsMine)
                return;
            if (!NetworkPlayer.AllowPlayerInput)
            {
                _networkPlayer.MoveDirection = Vector3.zero;
                return;
            }

            LookRotation(transform, _fpsCamera.transform);

            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");

            _networkPlayer.MoveDirection = (v * transform.forward + h * _fpsCamera.transform.right);

            if (_networkPlayer.MoveDirection.magnitude > 1f)
                _networkPlayer.MoveDirection = _networkPlayer.MoveDirection.normalized;

            if (Input.GetButtonDown("Jump") && _networkPlayer.IsGrounded && !_networkPlayer.IsJumping)
            {
                _networkPlayer.IsJumping = true;
                if (_networkPlayer.IsCrouching)
                    MessageHub.SendMessage(MessageType.PlayerCrouchKeyPressed);
            }
            if (Input.GetButtonDown("Crouch") && _networkPlayer.IsGrounded && !_networkPlayer.IsJumping)
            {
                if (_networkPlayer.IsWalking)
                    _networkPlayer.IsWalking = false;
                MessageHub.SendMessage(MessageType.PlayerCrouchKeyPressed);
            }
            if (NetworkManager.Instance.ReadyCheckActive)
            {
                if (Input.GetButtonDown("PlayReady"))
                {
                    NetworkManager.Instance.RaiseNetworkEvent(NetworkGameEventCode.ReadyCheckResponse,
                        new Photon.Realtime.RaiseEventOptions
                        {
                            Receivers = Photon.Realtime.ReceiverGroup.All
                        },
                        SendOptions.SendReliable, true);
                }

                if (Input.GetButtonDown("PlayerNotReady"))
                {
                    NetworkManager.Instance.RaiseNetworkEvent(NetworkGameEventCode.ReadyCheckResponse,
                        new Photon.Realtime.RaiseEventOptions
                        {
                            Receivers = Photon.Realtime.ReceiverGroup.All
                        }, SendOptions.SendReliable, false);
                }
            }

            _networkPlayer.IsWalking = Input.GetButton("Walk") && !_networkPlayer.IsCrouching && _networkPlayer.IsGrounded;
            _networkPlayer.IsMoving = v != 0 || h != 0;

            RayCastTrigger.InteractableRayCast(_fpsCamera.transform.position, _fpsCamera.transform.forward, out _, _interactRange, _interactLayerMask.value, QueryTriggerInteraction.Collide, GetComponent<Collider>());

            SetState();
        }

        private void OnEnable()
        {
            MessageHub.Subscribe(MessageType.SensitivityChanged, OnSensitivtyChanged, ActionExecutionScope.Default);
        }

        private void OnDisable()
        {
            MessageHub.Unsubscribe(MessageType.SensitivityChanged, OnSensitivtyChanged);
        }

        #endregion

        #region Messaging Callbacks

        private void OnSensitivtyChanged(Message obj)
        {
            _mouseSensitivity = (float)obj.Data[0];
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Does the mouse looking and the weapon rig looking
        /// </summary>
        /// <param name="character"></param>
        /// <param name="camera"></param>
        internal void LookRotation(Transform character, Transform camera)
        {
            float yRot = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float xRot = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            if (_invertXAxis)
                xRot = -xRot;

            if (_invertYAxis)
                yRot = -yRot;

            _characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            _cameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            _cameraTargetRot = ClampRotationAroundXAxis(_cameraTargetRot);

            character.localRotation = _characterTargetRot;
            camera.localRotation = _cameraTargetRot;
        }

        private void SetState()
        {
            PrevFpsState = FpsState;

            if (!_networkPlayer.IsGrounded)
                FpsState = FpsState.Jumping;
            else
            {
                if (_networkPlayer.IsMoving)
                {
                    if (_networkPlayer.IsWalking)
                        FpsState = FpsState.Walking;
                    else if (_networkPlayer.IsCrouching)
                        FpsState = FpsState.Crouching;
                    else
                        FpsState = FpsState.Running;
                }
                else
                {
                    if (_networkPlayer.IsCrouching)
                        FpsState = FpsState.Staying | FpsState.Crouching;
                    else
                        FpsState = FpsState.Staying;
                }
            }
        }

        /// <summary>
        /// Clamps the Rotation of the X Axis, so the player cant look to much up or down
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, _minimumX, _maximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
        #endregion
    }
}