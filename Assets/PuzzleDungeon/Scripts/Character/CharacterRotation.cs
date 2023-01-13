using System;
using PuzzleDungeon.Input;
using UnityEngine;

namespace PuzzleDungeon.Character
{
    public class CharacterRotation : CharacterComponent, IInputReceiver
    {
        [Serializable]
        struct RotationSettings
        {
            enum RotationMode
            {
                Transform,
                RigidBody,
            }

            [SerializeField] private RotationMode rotationMode;
            [SerializeField] private Transform    rotatingTransform;
            [SerializeField] private Rigidbody    rotatingRigidbody;
            [SerializeField] private bool         useLocalRotation;
            [SerializeField] private bool         clampRotation;
            [SerializeField] private float        minRotation;
            [SerializeField] private float        maxRotation;

            public Vector3 P_EulerAngles
            {
                get
                {
                    if (rotationMode == RotationMode.RigidBody)
                    {
                        return rotatingRigidbody.rotation.eulerAngles;
                    }

                    return useLocalRotation ? rotatingTransform.localEulerAngles : rotatingTransform.eulerAngles;
                }
            }

            public Quaternion P_Rotation
            {
                get
                {
                    if (rotationMode == RotationMode.RigidBody)
                    {
                        return rotatingRigidbody.rotation;
                    }

                    return useLocalRotation ? rotatingTransform.localRotation : rotatingTransform.rotation;
                }
            }

            public Transform P_RotatingTransform => rotatingTransform;
            public Rigidbody P_RotatingRigidbody => rotatingRigidbody;

            public float ClampRotationValue(float value) => clampRotation ? Mathf.Clamp(value, minRotation, maxRotation) : value;
            
            public void SetRotation(Quaternion newRot)
            {
                if (rotationMode == RotationMode.RigidBody)
                {
                    rotatingRigidbody.MoveRotation(newRot);
                    return;
                }
                
                if (useLocalRotation)
                {
                    rotatingTransform.localRotation = newRot;
                    return;
                }
                rotatingTransform.rotation = newRot;
            }
        }

        [SerializeField]                   private float            speed;
        [SerializeField] [Range(0, 0.99f)] private float            smoothing;
        [SerializeField]                   private RotationSettings rotatingHorizontally;
        [SerializeField]                   private RotationSettings rotatingVertically;

        private float _horizontalRotation;
        private float _verticalRotation;
        private float _horizontalVelocity;
        private float _verticalVelocity;
        private bool  _rotationAllowed;

        public Vector2 P_RawLookVector { get; private set; }

        public void SetLookVector(Vector2 value)
        {
            P_RawLookVector = value;
        }
        
        public override void Initialize()
        {
            _horizontalRotation = rotatingHorizontally.P_EulerAngles.x;
            _verticalRotation   = rotatingVertically.P_EulerAngles.y;
            _rotationAllowed    = true;
        }

        private void Rotate()
        {
            if(!_rotationAllowed) return;
            
            _horizontalRotation = rotatingHorizontally.ClampRotationValue(_horizontalRotation + (P_RawLookVector.x * speed * Time.deltaTime));
            _verticalRotation   = rotatingVertically.ClampRotationValue(_verticalRotation     - (P_RawLookVector.y * speed * Time.deltaTime));

            var newHorizontalQuaternion = Quaternion.Euler(rotatingHorizontally.P_EulerAngles.x, _horizontalRotation,                rotatingHorizontally.P_EulerAngles.z);
            var newVerticalQuaternion   = Quaternion.Euler(_verticalRotation,                    rotatingVertically.P_EulerAngles.y, rotatingVertically.P_EulerAngles.z);

            rotatingHorizontally.SetRotation(Quaternion.Slerp(newHorizontalQuaternion, rotatingHorizontally.P_Rotation, smoothing));
            rotatingVertically.SetRotation(Quaternion.Slerp(newVerticalQuaternion,     rotatingVertically.P_Rotation,   smoothing));
        }

        public void LockRotation() => _rotationAllowed = false;

        public void UnlockRotation() => _rotationAllowed = true;

        public override void ProcessUpdate()
        {
            Rotate();
        }
        
        #region IInputReceiver

        public void ReceiveInputUpdate(InputManager input)
        {
            SetLookVector(input.P_MouseVector2.P_CurrentValue);
        }

        public void ListenToInputEvents(InputManager input)
        {
           // input.P_MouseVector2.E_ValueUpdated += SetLookVector;
        }

        public void StopListeningToInputEvents(InputManager input)
        {
            //input.P_MouseVector2.E_ValueUpdated -= SetLookVector;
        }

        #endregion
    }
}