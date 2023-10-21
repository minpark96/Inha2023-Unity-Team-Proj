using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Tutorial
{
    public class PlayerUI : MonoBehaviour
    {
        #region Private Serializable Fields

        [Tooltip("UI Text to display Player's Name")]
        [SerializeField]
        private TMP_Text _playerNameText;

        [Tooltip("UI Slider to display Player's Health")]
        [SerializeField]
        private Slider _playerHealthSlider;

        [Tooltip("Pixel offset from the player target")]
        [SerializeField]
        private Vector3 _screenOffset = new Vector3(0f, 30f, 0f);

        #endregion

        #region Private Fields

        private PlayerManager _target;
        float _characterControllerHeight = 0f;
        Transform _targetTransform;
        Renderer _targetRenderer;
        CanvasGroup _canvasGroup;
        Vector3 _targetPosition;

        #endregion

        #region Public Fields

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }

        void Update()
        {
            // Reflect the Player Health
            if (_playerHealthSlider != null)
            {
                _playerHealthSlider.value = _target.Health;
            }

            // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
            if (_target == null)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        void LateUpdate()
        {
            // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
            if (_targetRenderer != null)
            {
                this._canvasGroup.alpha = _targetRenderer.isVisible ? 1f : 0f;
            }

            // #Critical
            // Follow the Target GameObject on screen.
            if (_targetTransform != null)
            {
                _targetPosition = _targetTransform.position;
                _targetPosition.y += _characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(_targetPosition) + _screenOffset;
            }
        }

        #endregion

        #region Public Methods

        public void SetTarget(PlayerManager target)
        {
            if (target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }
            // Cache references for efficiency
            _target = target;

            _targetTransform = this._target.GetComponent<Transform>();
            _targetRenderer = this._target.GetComponent<Renderer>();
            CharacterController characterController = _target.GetComponent<CharacterController>();
            // Get data from the Player that won't change during the lifetime of this Component
            if (characterController != null)
            {
                _characterControllerHeight = characterController.height;
            }

            if (_playerNameText != null)
            {
                _playerNameText.text = _target.photonView.Owner.NickName;
            }
        }

        #endregion
    }
}