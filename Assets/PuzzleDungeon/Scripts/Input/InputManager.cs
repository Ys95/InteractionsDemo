using System;
using Tide;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PuzzleDungeon.Input
{
    public class InputManager : Tide.Singleton<InputManager>
    {
        public enum ButtonState
        {
            Released,
            Pressed,
            Held,
        }

        [Serializable]
        public class InputButton
        {
            private States<ButtonState> State;

            public ButtonState P_CurrentState  => State.CurrentState;
            public ButtonState P_PreviousState => State.PreviousState;

            public void TriggerButtonPress()
            {
                if (State.CurrentState == ButtonState.Pressed)
                {
                    return;
                }

                State.ChangeState(ButtonState.Pressed);
                E_ButtonPress?.Invoke();
            }

            public void TriggerButtonHeld()
            {
                if (State.CurrentState == ButtonState.Held)
                {
                    return;
                }

                State.ChangeState(ButtonState.Held);
                E_ButtonHold?.Invoke();
            }

            public void TriggerButtonReleased()
            {
                if (State.CurrentState == ButtonState.Released)
                {
                    return;
                }

                State.ChangeState(ButtonState.Released);
                E_ButtonRelease?.Invoke();
            }

            public event Action E_ButtonPress;
            public event Action E_ButtonHold;
            public event Action E_ButtonRelease;

            public InputButton(ButtonState initialState)
            {
                State = new States<ButtonState>(initialState, false);
            }
        }

        [Serializable]
        public class InputVector2
        {
            public Vector2 P_PreviousValue { get; private set; }
            public Vector2 P_CurrentValue  { get; private set; }

            public event Action<Vector2> E_ValueUpdated;

            public void UpdateValue(Vector2 newValue)
            {
                P_PreviousValue = P_CurrentValue;
                P_CurrentValue  = newValue;
                E_ValueUpdated?.Invoke(newValue);
            }
        }

        [SerializeField] private InputActionAsset inputAction;
        [SerializeField] private PlayerInput      playerInput;
        [SerializeField] private float            mouseSensitivity = 1;

        private InputButton  _primaryFireButton      = new InputButton(ButtonState.Released);
        private InputButton  _secondaryFireButton    = new InputButton(ButtonState.Released);
        private InputButton  _jumpButton             = new InputButton(ButtonState.Released);
        private InputVector2 _movementVector2        = new InputVector2();
        private InputVector2 _mouseVector2           = new InputVector2();
        private bool         _mouseSensitivityEdited = false;
        private float        _initialMouseSensitivity;

        public InputButton  P_PrimaryFireButton   => _primaryFireButton;
        public InputButton  P_SecondaryFireButton => _secondaryFireButton;
        public InputButton  P_JumpButton          => _jumpButton;
        public InputVector2 P_MouseVector2        => _mouseVector2;
        public InputVector2 P_MovementVector2     => _movementVector2;

        public float P_MouseSensitivity
        {
            get => mouseSensitivity;
            set => mouseSensitivity = value;
        }

        public void TemporaryEditMouseSensitivity(float newSensitivity)
        {
            if (_mouseSensitivityEdited)
            {
                Debug.Log("Mouse sensitivity already edited!");
                return;
            }

            _mouseSensitivityEdited  = true;
            _initialMouseSensitivity = mouseSensitivity;

            mouseSensitivity = newSensitivity;
        }

        public void RestoreInitialMouseSensitivity()
        {
            if (!_mouseSensitivityEdited)
            {
                Debug.Log("Mouse sensitivity not edited!");
                return;
            }

            mouseSensitivity        = _initialMouseSensitivity;
            _mouseSensitivityEdited = false;
        }

        public void SetMovementVector(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                _movementVector2.UpdateValue(Vector2.zero);
            }
            else
            {
                _movementVector2.UpdateValue(context.ReadValue<Vector2>());
            }
        }

        public void SetMouseVector(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                _mouseVector2.UpdateValue(Vector2.zero);
            }
            else
            {
                _mouseVector2.UpdateValue(context.ReadValue<Vector2>() * mouseSensitivity);
            }
        }

        public void SetJump(InputAction.CallbackContext context) => SetButton(_jumpButton, context);

        public void SetPrimaryFire(InputAction.CallbackContext context) => SetButton(_primaryFireButton, context);

        public void SetSecondaryFire(InputAction.CallbackContext context) => SetButton(_secondaryFireButton, context);
        
        private void SetButton(InputButton button, InputAction.CallbackContext context)
        {
            if (context.started)
            {
                button.TriggerButtonPress();
            }
            else if (context.performed)
            {
                button.TriggerButtonHeld();
            }
            else if (context.canceled)
            {
                button.TriggerButtonReleased();
            }
        }
    }
}