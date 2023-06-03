using UnityEngine;
using Photon.Pun;
using System;

namespace WatStudios.DeepestDungeon.Utility.CustomPhoton
{

    [AddComponentMenu("Photon Networking/Photon IK View")]
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Animator))]
    public class PhotonIKView : MonoBehaviour, IPunObservable
    {
        #region Private Fields
        private PhotonView _photonView;
        private Animator _animator;

        #region Hands and Feet
        private Vector3 _rightHandPosition;
        private Vector3 _leftHandPosition;
        private Vector3 _rightFootPosition;
        private Vector3 _leftFootPosition;

        private Vector3 _rightHandNetworkPosition;
        private Vector3 _leftHandNetworkPosition;
        private Vector3 _rightFootNetworkPosition;
        private Vector3 _leftFootNetworkPosition;

        private Quaternion _rightHandRotation;
        private Quaternion _leftHandRotation;
        private Quaternion _rightFootRotation;
        private Quaternion _leftFootRotation;

        private Quaternion _rightHandNetworkRotation;
        private Quaternion _leftHandNetworkRotation;
        private Quaternion _rightFootNetworkRotation;
        private Quaternion _leftFootNetworkRotation;

        private bool _IKRightHand;
        private bool _IKLeftHand;
        private bool _IKRightFoot;
        private bool _IKLeftFoot;

        private bool _IKRightHandNetwork;
        private bool _IKLeftHandNetwork;
        private bool _IKRightFootNetwork;
        private bool _IKLeftFootNetwork;
        #endregion

        #region Single Bones
        private bool _IKNeck;
        private bool _IKUpperChest;
        private bool _IKChest;

        private bool _IKNetworkNeck;
        private bool _IKNetworkUpperChest;
        private bool _IKNetworkChest;

        private Quaternion _rotNeck;
        private Quaternion _rotUpperChest;
        private Quaternion _rotChest;

        private Quaternion _rotNetworkNeck;
        private Quaternion _rotNetworkUpperChest;
        private Quaternion _rotNetworkChest;
        #endregion
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _photonView = gameObject.GetComponent<PhotonView>();
            _animator = gameObject.GetComponent<Animator>();
        }

        /// <summary>
        /// updates the IK Positions
        /// </summary>
        /// <param name="layerIndex"></param>
        public void OnAnimatorIK(int layerIndex)
        {
            if (!_photonView.IsMine)
            {
                if (_IKRightHandNetwork)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandNetworkPosition);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandNetworkRotation);
                }

                if (_IKLeftHandNetwork)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandNetworkPosition);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandNetworkRotation);
                }

                if (_IKRightFootNetwork)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    _animator.SetIKPosition(AvatarIKGoal.RightFoot, _rightFootNetworkPosition);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                    _animator.SetIKRotation(AvatarIKGoal.RightFoot, _rightFootNetworkRotation);
                }

                if (_IKLeftFootNetwork)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    _animator.SetIKPosition(AvatarIKGoal.LeftFoot, _leftFootNetworkPosition);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                    _animator.SetIKRotation(AvatarIKGoal.LeftFoot, _leftFootNetworkRotation);
                }

                if (_IKNetworkNeck)
                {
                    _animator.SetBoneLocalRotation(HumanBodyBones.Neck, _rotNetworkNeck);
                }

                if (_IKNetworkUpperChest)
                {
                    _animator.SetBoneLocalRotation(HumanBodyBones.UpperChest, _rotNetworkUpperChest);
                }

                if (_IKNetworkChest)
                {
                    _animator.SetBoneLocalRotation(HumanBodyBones.Chest, _rotNetworkChest);
                }
            }
        }
        #endregion

        #region Public Methods
        #region Hands and Feet
        /// <summary>
        /// The Position of the Right Hand will be set by the Skripts who uses IK Positions
        /// and then this position will be synced 
        /// </summary>
        /// <param name="pos"></param>
        public void SetRightHandPosition(Vector3 pos, Quaternion rot)
        {
            _IKRightHand = true;
            _rightHandPosition = pos;
            _rightHandRotation = rot;
        }

        /// <summary>
        /// The Position of the Left Hand will be set by the Skripts who uses IK Positions
        /// and then this position will be synced 
        /// </summary>
        /// <param name="pos"></param>
        public void SetLeftHandPosition(Vector3 pos, Quaternion rot)
        {
            _IKLeftHand = true;
            _leftHandPosition = pos;
            _leftHandRotation = rot;
        }

        /// <summary>
        /// The Position of the Right Foot will be set by the Skripts who uses IK Positions
        /// and then this position will be synced 
        /// </summary>
        /// <param name="pos"></param>
        public void SetRightFootPosition(Vector3 pos, Quaternion rot)
        {
            _IKRightFoot = true;
            _rightFootPosition = pos;
            _rightFootRotation = rot;
        }

        /// <summary>
        /// The Position of the Left Foot will be set by the Skripts who uses IK Positions
        /// and then this position will be synced 
        /// </summary>
        /// <param name="pos"></param>
        public void SetLeftFootPosition(Vector3 pos, Quaternion rot)
        {
            _IKLeftFoot = true;
            _leftFootPosition = pos;
            _leftFootRotation = rot;
        }

        public void DisableRightHand()
        {
            _IKRightHand = false;
        }

        public void DisableLeftHand()
        {
            _IKLeftHand = false;
        }

        public void DisbleRightFoot()
        {
            _IKRightFoot = false;
        }

        public void DisbleLeftFoot()
        {
            _IKLeftFoot = false;
        }
        #endregion

        #region Single Bones
        public void RotateNeck(Quaternion rot)
        {
            _IKNeck = true;
            _rotNeck = rot;
        }

        public void RotateUpperchest(Quaternion rot)
        {
            _IKUpperChest = true;
            _rotUpperChest = rot;
        }

        public void RotateChest(Quaternion rot)
        {
            _IKChest = true;
            _rotChest = rot;
        }

        public void DisableNeck()
        {
            _IKNeck = false;
        }

        public void DisableUpperChest()
        {
            _IKUpperChest = false;
        }

        public void DisableChest()
        {
            _IKChest = false;
        }
        #endregion
        #endregion

        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_IKRightHand);
                stream.SendNext(_IKLeftHand);
                stream.SendNext(_IKRightFoot);
                stream.SendNext(_IKLeftFoot);

                stream.SendNext(_IKNeck);
                stream.SendNext(_IKUpperChest);
                stream.SendNext(_IKChest);

                if (_IKRightHand)
                {
                    stream.SendNext(_rightHandPosition);
                    stream.SendNext(_rightHandRotation);
                }

                if (_IKLeftHand)
                {
                    stream.SendNext(_leftHandPosition);
                    stream.SendNext(_leftHandRotation);
                }

                if (_IKRightFoot)
                {
                    stream.SendNext(_rightFootPosition);
                    stream.SendNext(_rightFootRotation);
                }

                if (_IKLeftFoot)
                {
                    stream.SendNext(_leftFootPosition);
                    stream.SendNext(_leftFootRotation);
                }

                if (_IKNeck)
                    stream.SendNext(_rotNeck);

                if (_IKUpperChest)
                    stream.SendNext(_rotUpperChest);

                if (_IKChest)
                    stream.SendNext(_rotChest);
            }
            else
            {
                _IKRightHandNetwork = (bool)stream.ReceiveNext();
                _IKLeftHandNetwork = (bool)stream.ReceiveNext();
                _IKRightFootNetwork = (bool)stream.ReceiveNext();
                _IKLeftFootNetwork = (bool)stream.ReceiveNext();

                _IKNetworkNeck = (bool)stream.ReceiveNext();
                _IKNetworkUpperChest = (bool)stream.ReceiveNext();
                _IKNetworkChest = (bool)stream.ReceiveNext();

                if (_IKRightHandNetwork)
                {
                    _rightHandNetworkPosition = (Vector3)stream.ReceiveNext();
                    _rightHandNetworkRotation = (Quaternion)stream.ReceiveNext();
                }

                if (_IKLeftHandNetwork)
                {
                    _leftHandNetworkPosition = (Vector3)stream.ReceiveNext();
                    _leftHandNetworkRotation = (Quaternion)stream.ReceiveNext();
                }

                if (_IKRightFootNetwork)
                {
                    _rightFootNetworkPosition = (Vector3)stream.ReceiveNext();
                    _rightFootNetworkRotation = (Quaternion)stream.ReceiveNext();
                }

                if (_IKLeftFootNetwork)
                {
                    _leftFootNetworkPosition = (Vector3)stream.ReceiveNext();
                    _leftFootNetworkRotation = (Quaternion)stream.ReceiveNext();
                }

                if (_IKNetworkNeck)
                    _rotNetworkNeck = (Quaternion)stream.ReceiveNext();

                if (_IKNetworkUpperChest)
                    _rotNetworkUpperChest = (Quaternion)stream.ReceiveNext();

                if (_IKNetworkChest)
                    _rotNetworkChest = (Quaternion)stream.ReceiveNext();
            }
        }

        #endregion
    }
}