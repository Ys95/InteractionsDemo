using PuzzleDungeon.Input;
using UnityEngine;

namespace PuzzleDungeon.Character
{
    public class CharacterLook : CharacterComponent, IInputReceiver
    {
        [SerializeField] private Transform lookingRoot;
        [SerializeField] private float     lookSpeed;

        private float             _horizontalRotation;
        private float             _verticalRotation;
        private CharacterMovement _characterMovement;

        public Vector2 P_RawLookVector { get; private set; }

        public float P_HorizontalLookRotation => _horizontalRotation;
        public float P_VerticalLookRotation   => _verticalRotation;

        public override void Initialize()
        {
            
        }

        private void Look()
        {
            var lookVector = P_RawLookVector;
            lookVector *= (lookSpeed * Time.deltaTime);

            _horizontalRotation += lookVector.x;
            _verticalRotation   -= lookVector.y;
            _verticalRotation   =  Mathf.Clamp(_verticalRotation, -90f, 90f);

            var newRot = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0);
            lookingRoot.rotation  = newRot;
        }

        public void SetLookVector(Vector2 value)
        {
            P_RawLookVector = value;
        }
        
        public override void ProcessUpdate()
        {
           Look();
        }

        public override void ProcessLateUpdate()
        {
          //  Look();
        }
        
        public override void ProcessFixedUpdate()
        {
          //  Look();
        }

        private void OnDrawGizmos()
        {
            if(lookingRoot==null)
            {
                return;
            }

            #region Look direction

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(lookingRoot.position, lookingRoot.rotation, lookingRoot.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.color  = Color.magenta;
            Gizmos.DrawCube(new Vector3(0f, 0f, 0.75f), new Vector3(0.05f, 0.05f, 1.5f));

            #endregion
        }
        
        #region IInputReceiver

        public void ReceiveInputUpdate(InputManager input)
        {
            P_RawLookVector = input.P_MouseVector2.P_CurrentValue;
        }

        public void ListenToInputEvents(InputManager input)
        {
          //  input.P_MouseVector2.E_ValueUpdated += SetLookVector;
        }

        public void StopListeningToInputEvents(InputManager input)
        {
          //  input.P_MouseVector2.E_ValueUpdated -= SetLookVector;
        }

        #endregion
    }
}