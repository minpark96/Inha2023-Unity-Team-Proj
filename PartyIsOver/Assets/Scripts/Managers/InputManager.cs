using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;
    public Action<Define.KeyboardEvent> KeyboardAction = null;

    bool _leftMpressed = false;
    float _leftMpressedTime = 0;

    bool _rightMpressed = false;
    float _rightMpressedTime = 0;

    bool _wheelMpressed = false;
    float _wheelMpressedTime = 0;

    bool _hkeyPressed = false;
    float _hkeyPressedTime = 0;

    bool _rkeyPressed = false;
    float _rkeyPressedTime = 0;

    bool _spaceKeyPressed = false;
    float _spaceKeyPressedTime = 0;

    bool _isCharge = false;
    float _chargeTime = 0;
    float _chargeThreshold = 0.2f;

    bool _wkeyPressed = false;
    bool _akeyPressed = false;
    bool _skeyPressed = false;
    bool _dkeyPressed = false;
    bool _lshiftKeyPressed = false;

    Define.KeyboardEvent a;

    public void OnUpdate()
    {
        // UI를 클릭할 때는 캐릭터가 움직이지 않게 함
        //if (EventSystem.current.IsPointerOverGameObject()) 
            //return;

        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        // 이동
        if (KeyboardAction != null)
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (!_wkeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerDown);
                }
                KeyboardAction.Invoke(Define.KeyboardEvent.Press);
                _wkeyPressed = true;
            }
            else
            {
                if (_wkeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.Click);
                }
                _wkeyPressed = false;
            }
        }
        if (KeyboardAction != null)
        {
            if (Input.GetKey(KeyCode.A))
            {
                if (!_akeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerDown);
                }
                KeyboardAction.Invoke(Define.KeyboardEvent.Press);
                _akeyPressed = true;
            }
            else
            {
                if (_akeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.Click);
                }
                _akeyPressed = false;
            }
        }
        if (KeyboardAction != null)
        {
            if (Input.GetKey(KeyCode.S))
            {
                if (!_skeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerDown);
                }
                KeyboardAction.Invoke(Define.KeyboardEvent.Press);
                _skeyPressed = true;
            }
            else
            {
                if (_skeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.Click);
                }
                _skeyPressed = false;
            }
        }
        if (KeyboardAction != null)
        {
            if (Input.GetKey(KeyCode.D))
            {
                if (!_dkeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerDown);
                }
                KeyboardAction.Invoke(Define.KeyboardEvent.Press);
                _dkeyPressed = true;
            }
            else
            {
                if (_dkeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.Click);
                }
                _dkeyPressed = false;
            }
        }
        if (KeyboardAction != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!_lshiftKeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerDown);
                }
                KeyboardAction.Invoke(Define.KeyboardEvent.Press);
                _lshiftKeyPressed = true;
            }
            else
            {
                if (_lshiftKeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.Click);
                }
                _lshiftKeyPressed = false;
            }
        }

        // 스킬
        if (KeyboardAction != null)
        {
            if (Input.GetKey(KeyCode.H))
            {
                if (!_hkeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerDown);
                    _hkeyPressedTime = Time.time;
                }
                KeyboardAction.Invoke(Define.KeyboardEvent.Press);
                _hkeyPressed = true;
            }
            else 
            {
                if (_hkeyPressed)
                {
                    if (Time.time < _hkeyPressedTime + 0.2f)
                        KeyboardAction.Invoke(Define.KeyboardEvent.Click);
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerUp);
                }
                _hkeyPressed = false;
                _hkeyPressedTime = 0;
            }
        }

        if (KeyboardAction != null)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (!_spaceKeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerDown);
                    _spaceKeyPressedTime = Time.time;
                }
                KeyboardAction.Invoke(Define.KeyboardEvent.Press);
                _spaceKeyPressed = true;
            }
            else
            {
                if (_spaceKeyPressed)
                {
                    if (Time.time < _spaceKeyPressedTime + 0.2f)
                        KeyboardAction.Invoke(Define.KeyboardEvent.Click);
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerUp);
                }
                _spaceKeyPressed = false;
                _spaceKeyPressedTime = 0;
            }
        }
        
        if (KeyboardAction != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!_rkeyPressed)
                {
                    KeyboardAction.Invoke(Define.KeyboardEvent.PointerDown);
                    _rkeyPressedTime = Time.time;
                }
                _rkeyPressed = true;
                _chargeTime = 0;
                _isCharge = true;
            }
            else if (Input.GetKey(KeyCode.R))
            {
                if (_isCharge)
                {
                    _chargeTime += Time.deltaTime;
                    if (_chargeTime > _chargeThreshold)
                    {
                        KeyboardAction.Invoke(Define.KeyboardEvent.Hold);
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                if (_rkeyPressed)
                {
                    if (Time.time < _rkeyPressedTime + 0.7f)
                    {
                        KeyboardAction.Invoke(Define.KeyboardEvent.Click);
                    }
                    else if(Time.time > _rkeyPressedTime + 2f)
                    {
                        KeyboardAction.Invoke(Define.KeyboardEvent.Charge);
                    }
                }
                _rkeyPressed = false;
                _isCharge = false;
            }
        }

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_leftMpressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _leftMpressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _leftMpressed = true;
            }
            else 
            {
                if (_leftMpressed)
                {
                    if (Time.time < _leftMpressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _leftMpressed = false;
                _leftMpressedTime = 0;
            }
        }

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(1))
            {
                if (!_rightMpressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _rightMpressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _rightMpressed = true;
            }
            else
            {
                if (_rightMpressed)
                {
                    if (Time.time < _rightMpressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _rightMpressed = false;
                _rightMpressedTime = 0;
            }
        }

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(2))
            {
                if (!_wheelMpressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _wheelMpressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _wheelMpressed = true;
            }
            else
            {
                if (_wheelMpressed)
                {
                    if (Time.time < _wheelMpressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _wheelMpressed = false;
                _wheelMpressedTime = 0;
            }
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
        KeyboardAction = null;
    }
}