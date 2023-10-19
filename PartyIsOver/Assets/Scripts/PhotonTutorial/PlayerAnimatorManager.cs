using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Tutorial
{
    public class PlayerAnimatorManager : MonoBehaviour
    {
        #region Private Fields

        [SerializeField]
        private float _directionDampTime = 0.25f;
        private Animator _animator;

        #endregion

        #region MonoBehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
            if (!_animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!_animator)
            {
                return;
            }

            // deal with Jumping
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            // only allow jumping if we are running.
            if (stateInfo.IsName("Base Layer.Run"))
            {
                // When using trigger parameter
                if (Input.GetButtonDown("Fire2"))
                {
                    _animator.SetTrigger("Jump");
                }
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (v < 0)
            {
                v = 0;
            }
            _animator.SetFloat("Speed", h * h + v * v);
            _animator.SetFloat("Direction", h, _directionDampTime, Time.deltaTime);
        }

        #endregion
    }
}