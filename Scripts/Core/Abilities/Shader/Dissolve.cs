using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WatStudios.DeepestDungeon.Messaging;

namespace WatStudios.DeepestDungeon.AbilitySystem.FX
{
    public class Dissolve : MonoBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private GameObject _dissolvingObject;
        [SerializeField] private Shader _dissolveShader;
        [SerializeField] [Range(0, 5)] private float _speedMultiplier = 1;
        [SerializeField] [ColorUsage(true, true)] private Color _edgeColor;
        [SerializeField] private float _edgeWidth;
        [SerializeField] private float _noiseScale;
        [SerializeField] private bool _keepToggeling;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private List<GameObject> _gameObjects;
        private Material[] _materials;
        private Material[] _dissolveMaterials;
        private float _dissolveValue;
        private Coroutine _appearCoroutine;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            if (_dissolvingObject == null)
                _dissolvingObject = gameObject;

            _gameObjects = new List<GameObject>();

            if (GetComponent<Renderer>() != null)
                _gameObjects.Add(gameObject);

            foreach (Transform child in _dissolvingObject.transform)
            {
                if (child.GetComponent<Renderer>() != null)
                    _gameObjects.Add(child.gameObject);
            }

            _materials = new Material[_gameObjects.Count];
            _dissolveMaterials = new Material[_gameObjects.Count];

            for (int i = 0; i < _dissolveMaterials.Length; i++)
            {
                _dissolveMaterials[i] = new Material(_dissolveShader);
            }

            for (int i = 0; i < _gameObjects.Count; i++)
            {
                _materials[i] = _gameObjects[i].GetComponent<Renderer>().material;

                _dissolveMaterials[i].CopyPropertiesFromMaterial(_materials[i]);
                _dissolveMaterials[i].SetColor("_EdgeColor", _edgeColor);
                _dissolveMaterials[i].SetFloat("_EdgeWidth", _edgeWidth);
                _dissolveMaterials[i].SetFloat("_NoiseScale", _noiseScale);
            }
            _dissolveValue = 1;
            StartCoroutine(AppearCoroutine());
        }

        private void OnEnable()
        {
            MessageHub.Subscribe(MessageType.DissolveRoutine, OnDissolveRoutine, ActionExecutionScope.Default);
        }

        private void OnDisable()
        {
            MessageHub.Unsubscribe(MessageType.DissolveRoutine, OnDissolveRoutine);
        }
        #endregion

        #region Messaging Callbacks
        private void OnDissolveRoutine(Message obj)
        {
            if (_dissolveValue == 1)
                StartCoroutine(AppearCoroutine());
            else if (_dissolveValue == -1)
                StartCoroutine(DissolveCoroutine());
        }
        #endregion

        #region Private Methods
        private IEnumerator AppearCoroutine()
        {
            while (true)
            {
                for (int i = 0; i < _gameObjects.Count; i++)
                {
                    if (_gameObjects[i].GetComponent<Renderer>().sharedMaterial == _dissolveMaterials[i])
                        break;

                    _gameObjects[i].GetComponent<Renderer>().sharedMaterial = _dissolveMaterials[i];
                }

                _dissolveValue -= (Time.deltaTime * _speedMultiplier);

                foreach (Material mat in _dissolveMaterials)
                    mat.SetFloat("_DissolveValue", _dissolveValue);

                if (_dissolveValue <= -1)
                {
                    _dissolveValue = -1;

                    foreach (Material mat in _dissolveMaterials)
                        mat.SetFloat("_DissolveValue", _dissolveValue);

                    for (int i = 0; i < _gameObjects.Count; i++)
                    {
                        _gameObjects[i].GetComponent<Renderer>().sharedMaterial = _materials[i];
                    }

                    if (_keepToggeling)
                    {
                        StartCoroutine(DissolveCoroutine());
                        break;
                    }
                    else
                        break;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield break;
        }

        private IEnumerator DissolveCoroutine()
        {
            while (true)
            {
                for (int i = 0; i < _gameObjects.Count; i++)
                {
                    if (_gameObjects[i].GetComponent<Renderer>().sharedMaterial == _dissolveMaterials[i])
                        break;

                    _gameObjects[i].GetComponent<Renderer>().sharedMaterial = _dissolveMaterials[i];
                }

                _dissolveValue += (Time.deltaTime * _speedMultiplier);

                foreach (Material mat in _dissolveMaterials)
                    mat.SetFloat("_DissolveValue", _dissolveValue);

                if (_dissolveValue >= 1)
                {
                    _dissolveValue = 1;

                    foreach (Material mat in _dissolveMaterials)
                        mat.SetFloat("_DissolveValue", _dissolveValue);

                    if (_keepToggeling)
                    {
                        StartCoroutine(AppearCoroutine());
                        break;
                    }
                    else
                        break;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield break;
        }
        #endregion
    }
}