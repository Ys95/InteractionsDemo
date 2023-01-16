using System;
using PuzzleDungeon.Tools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PuzzleDungeon.Input
{
    public class InputManager : Singleton<InputManager>
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
            private StateMachine<ButtonState> State;

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

            public InputButton()
            {
                State = new StateMachine<ButtonState>(ButtonState.Released, false);
            }
        }

        [Serializable]
        public class InputValue<T>
        {
            public T P_PreviousValue { get; private set; }
            public T P_CurrentValue  { get; private set; }

            public event Action<T> E_ValueUpdated;

            public void UpdateValue(T newValue)
            {
                P_PreviousValue = P_CurrentValue;
                P_CurrentValue  = newValue;
                E_ValueUpdated?.Invoke(newValue);
            }
        }

        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private float       mouseSensitivity = 1;

        private InputButton         _primaryFireButton      = new InputButton();
        private InputButton         _secondaryFireButton    = new InputButton();
        private InputButton         _jumpButton             = new InputButton();
        private InputButton         _actionButton           = new InputButton();
        private InputValue<Vector2> _movementVector2        = new InputValue<Vector2>();
        private InputValue<Vector2> _mouseVector2           = new InputValue<Vector2>();
        private InputValue<float>   _scrollFloat            = new InputValue<float>();
        private bool                _mouseSensitivityEdited = false;
        private float               _initialMouseSensitivity;

        public InputButton         P_PrimaryFireButton   => _primaryFireButton;
        public InputButton         P_SecondaryFireButton => _secondaryFireButton;
        public InputButton         P_JumpButton          => _jumpButton;
        public InputButton         P_ActionButton        => _actionButton;
        public InputValue<Vector2> P_MouseVector2        => _mouseVector2;
        public InputValue<Vector2> P_MovementVector2     => _movementVector2;
        public InputValue<float>   P_ScrollFloat         => _scrollFloat;

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

        public void SetScrollVector(InputAction.CallbackContext context)
        {
            var scrollValue = context.ReadValue<float>();

            if (scrollValue > 0)
            {
                _scrollFloat.UpdateValue(1);
            }
            else if (scrollValue < 0)
            {
                _scrollFloat.UpdateValue(-1);
            }
            else
            {
                _scrollFloat.UpdateValue(0);
            }

            Debug.Log(_scrollFloat.P_CurrentValue);
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

        public void SetAction(InputAction.CallbackContext context) => SetButton(_actionButton, context);

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