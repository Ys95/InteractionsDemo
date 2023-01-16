using System;
using System.Collections.Generic;
using PuzzleDungeon.Input;
using PuzzleDungeon.Interactions;
using UnityEngine;

namespace PuzzleDungeon.Character
{
    public class CharacterInteractions : CharacterComponent, IInputReceiver
    {
        [Serializable]
        public struct AnchorMovingModeMouseAxisMapping
        {
            public Axis MouseXAnchorAxis;
            public Axis MouseYAnchorAxis;
            public Axis MouseScrollAnchorAxis;
        }

        /// <summary>
        /// Anchor is player "hand"
        /// </summary>
        [Serializable]
        private struct GrabAnchor
        {
            public Transform Pivot;
            public Transform Anchor;
            public float     MaxMovementSpeed;
            public Vector2   MovementConstraints;

            private Transform _defaultPivotParent;
            private Vector3   _defaultPivotLocalPosition;
            private Vector3   _defaultAnchorLocalPosition;
            private Vector2   _defaultMovementConstraints;

            public void Initialize()
            {
                _defaultPivotParent         = Pivot.parent;
                _defaultPivotLocalPosition  = Pivot.localPosition;
                _defaultAnchorLocalPosition = Anchor.localPosition;
                _defaultMovementConstraints = MovementConstraints;
            }

            public void RestoreDefaultConstraints() => MovementConstraints = _defaultMovementConstraints;

            public void RepositionPivot(Vector3 position, bool localSpace) => Reposition(Pivot, position, localSpace);

            public void RepositionAnchor(Vector3 position, bool localSpace) => Reposition(Anchor, position, localSpace);

            public void RestoreAnchorPosition() => Reposition(Anchor, _defaultAnchorLocalPosition, true);

            public void RestorePivotPosition() => Reposition(Pivot, _defaultPivotLocalPosition, true);

            public void RestorePivotParent()
            {
                Pivot.localEulerAngles = Vector3.zero;
                Pivot.parent           = _defaultPivotParent;
            }

            private void Reposition(Transform transform, Vector3 position, bool localSpace)
            {
                if (localSpace)
                {
                    transform.localPosition = position;
                    return;
                }

                transform.position = position;
            }
        }

        [SerializeField] private Transform interactionCheckRayStartPoint;
        [SerializeField] private LayerMask interactionLayers;
        [SerializeField] private float     interactionCheckRayMaxLenght;
        [SerializeField] private float     maxDistanceToInteractable;
        [SerializeField] private float     raycastFrequency;

        [Header("Grab Anchor")]
        [SerializeField] private GrabAnchor grabAnchor;

        private float                            _raycastTimer;
        private float                            _anchorMovingModeMovementX;
        private float                            _anchorMovingModeMovementY;
        private float                            _anchorMovingModeMovementZ;
        private bool                             _interactionAllowed;
        private bool                             _anchorMovingMode;
        private Interactable                     _previousInteractableInRange;
        private Interactable                     _currentInteractableInRange;
        private Interactable                     _currentInteraction;
        private Vector3                          _initialAnchorLocalPosition;
        private Vector3?                         _localAnchorConstraints;
        private CharacterRotation                _characterRotation;
        private AnchorMovingModeMouseAxisMapping _anchorMovingModeMouseAxisMapping;

        public event Action E_InteractionStarted;
        public event Action E_InteractionEnded;

        public Transform    P_GrabAnchor         => grabAnchor.Anchor;
        public Interactable P_CurrentInteraction => _currentInteraction;

        public Vector2 P_RawLookVector { get; private set; }
        public float   P_ScrollFloat   { get; private set; }

        public void SetLookVector(Vector2 lookVector)
        {
            P_RawLookVector = lookVector;
        }

        public void SetScrollValue(float scroll)
        {
            P_ScrollFloat = scroll;
        }

        /// <summary>
        /// Stop rotating, player mouse moves only anchor
        /// </summary>
        public void EnableAnchorMovingMode(AnchorMovingModeMouseAxisMapping axisMapping, Vector3? localConstraints)
        {
            grabAnchor.Pivot.transform.parent           = null;
            grabAnchor.Pivot.transform.localEulerAngles = Vector3.zero;
            _localAnchorConstraints                     = localConstraints;

            _anchorMovingModeMovementX = 0;
            _anchorMovingModeMovementY = 0;
            _anchorMovingModeMovementZ = 0;

            _characterRotation.LockRotation();
            _anchorMovingMode                 = true;
            _anchorMovingModeMouseAxisMapping = axisMapping;
        }

        public void DisableAnchorMovingMode()
        {
            ResetAnchor();

            _characterRotation.UnlockRotation();
            _anchorMovingMode = false;
        }

        public void RepositionAnchorPivot(Vector3 pivotPosition, Vector3? grabAnchorPosition = null)
        {
            grabAnchor.RepositionPivot(pivotPosition, false);

            if (grabAnchorPosition.HasValue)
            {
                grabAnchor.RepositionAnchor(grabAnchorPosition.Value, false);
                _anchorMovingModeMovementY = grabAnchor.Anchor.localPosition.y;
                _anchorMovingModeMovementX = grabAnchor.Anchor.localPosition.x;
            }
            else
            {
                grabAnchor.RestoreAnchorPosition();
                _anchorMovingModeMovementY = 0f;
                _anchorMovingModeMovementX = 0f;
            }
        }

        public void RepositionAnchor(Vector3 position)
        {
            grabAnchor.RepositionAnchor(position, false);
        }

        public void ResetAnchor()
        {
            grabAnchor.RestorePivotParent();
            grabAnchor.RestorePivotPosition();
            grabAnchor.RestoreDefaultConstraints();
            grabAnchor.RestoreAnchorPosition();
        }

        private void SearchForInteractable()
        {
            var hitSomething = Physics.Raycast(interactionCheckRayStartPoint.position,
                                               interactionCheckRayStartPoint.forward,
                                               out var rayHit,
                                               interactionCheckRayMaxLenght,
                                               interactionLayers);

            Interactable interactable = null;

            if (hitSomething && Vector3.Distance(P_CharacterHub.transform.position, rayHit.collider.transform.position) <= maxDistanceToInteractable)
            {
                interactable = rayHit.collider.GetComponent<Interactable>();
            }

            ProcessSearchResult(interactable);
        }

        private void ProcessSearchResult(Interactable detectedInteractable)
        {
            if (detectedInteractable == _currentInteractableInRange)
            {
                return;
            }

            if (detectedInteractable == null)
            {
                if (_currentInteractableInRange != null)
                {
                    _currentInteractableInRange.StopHighlightingObject();
                }

                _currentInteractableInRange = null;
                return;
            }
            
            _previousInteractableInRange = _currentInteractableInRange;
            if (_previousInteractableInRange != null)
            {
                _previousInteractableInRange.StopHighlightingObject();
            }

            _currentInteractableInRange = detectedInteractable;
            _currentInteractableInRange.HighlightObject();
        }

        private void BeginInteraction()
        {
            if (_currentInteractableInRange == null)
            {
                return;
            }

            if (_currentInteraction != null)
            {
                return;
            }

            _currentInteraction         = _currentInteractableInRange;
            _currentInteractableInRange = null;
            _currentInteraction.StopHighlightingObject();
            _currentInteraction.E_InteractionEnded += OnInteractionEndedByInteractable;
            _currentInteraction.StartInteraction(this);
            E_InteractionStarted?.Invoke();
        }

        public void BeginInteraction(Interactable interactable)
        {
            _currentInteractableInRange = interactable;
            BeginInteraction();
        }

        private void OnInteractionEndedByInteractable()
        {
            _currentInteraction.E_InteractionEnded -= OnInteractionEndedByInteractable;
            _currentInteraction                    =  null;

            E_InteractionEnded?.Invoke();
        }

        private void EndInteraction()
        {
            if (_currentInteraction == null)
            {
                return;
            }

            _currentInteraction.E_InteractionEnded -= OnInteractionEndedByInteractable;
            _currentInteraction.EndInteraction();
            _currentInteraction = null;
            E_InteractionEnded?.Invoke();
        }

        private void PrimaryInteractionButtonReleased()
        {
            if (_currentInteraction == null)
            {
                return;
            }

            _currentInteraction.PrimaryInteractionButtonReleased();
        }

        private void SecondaryInteractionButtonPressed()
        {
            if (_currentInteraction == null)
            {
                return;
            }

            _currentInteraction.SecondaryInteractionButtonPressed();
        }

        private void SecondaryInteractionButtonReleased()
        {
            if (_currentInteraction == null)
            {
                return;
            }

            _currentInteraction.SecondaryInteractionButtonReleased();
        }

        private void ActionButtonPresses()
        {
            if (_currentInteraction == null)
            {
                return;
            }

            _currentInteraction.ActionButtonPressed();
        }

        private void ActionButtonReleased()
        {
            if (_currentInteraction == null)
            {
                return;
            }

            _currentInteraction.ActionButtonReleased();
        }

        private void CalculateAxisMovement(float inputAxis, Axis anchorMapping)
        {
            switch (anchorMapping)
            {
                case Axis.X:
                    _anchorMovingModeMovementX += inputAxis * (grabAnchor.MaxMovementSpeed * Time.deltaTime);
                    break;

                case Axis.Y:
                    _anchorMovingModeMovementY += inputAxis * (grabAnchor.MaxMovementSpeed * Time.deltaTime);
                    break;

                case Axis.Z:
                    _anchorMovingModeMovementZ += inputAxis * (grabAnchor.MaxMovementSpeed * Time.deltaTime);
                    break;

                case Axis.None:
                    break;
            }

            if (_localAnchorConstraints == null)
            {
                return;
            }

            _anchorMovingModeMovementX = Mathf.Clamp(_anchorMovingModeMovementX, -_localAnchorConstraints.Value.x, _localAnchorConstraints.Value.x);
            _anchorMovingModeMovementY = Mathf.Clamp(_anchorMovingModeMovementY, -_localAnchorConstraints.Value.y, _localAnchorConstraints.Value.y);
            _anchorMovingModeMovementZ = Mathf.Clamp(_anchorMovingModeMovementZ, -_localAnchorConstraints.Value.z, _localAnchorConstraints.Value.z);
        }

        private void MoveGrabAnchor()
        {
            if (!_anchorMovingMode)
            {
                return;
            }

            CalculateAxisMovement(P_RawLookVector.x,  _anchorMovingModeMouseAxisMapping.MouseXAnchorAxis);
            CalculateAxisMovement(P_RawLookVector.y,  _anchorMovingModeMouseAxisMapping.MouseYAnchorAxis);
            CalculateAxisMovement(P_ScrollFloat * 10, _anchorMovingModeMouseAxisMapping.MouseScrollAnchorAxis);

            var newRot = transform.rotation.eulerAngles;
            newRot.x                               = 0;
            grabAnchor.Pivot.transform.eulerAngles = newRot;

            grabAnchor.RepositionAnchor(new Vector3(_anchorMovingModeMovementX, _anchorMovingModeMovementY, _anchorMovingModeMovementZ), true);
        }

        private void OnDrawGizmos()
        {
            if (interactionCheckRayStartPoint == null)
            {
                return;
            }

            Matrix4x4 rotationMatrix = interactionCheckRayStartPoint.localToWorldMatrix;
            Gizmos.matrix = rotationMatrix;
            Gizmos.color  = Color.cyan;
            Gizmos.DrawCube(Vector3.zero + new Vector3(0f, 0f, interactionCheckRayMaxLenght / 2f), new Vector3(0.02f, 0.02f, interactionCheckRayMaxLenght));
        }

        #region CharacterComponent

        public override void ProcessUpdate()
        {
            MoveGrabAnchor();

            if (!_interactionAllowed || _currentInteraction != null)
            {
                return;
            }

            if (_raycastTimer <= 0)
            {
                SearchForInteractable();
                _raycastTimer = raycastFrequency;
                return;
            }

            _raycastTimer -= Time.deltaTime;
        }

        public override void Initialize()
        {
            grabAnchor.Initialize();
            _interactionAllowed = true;
            _characterRotation  = P_CharacterHub.GetCharacterComponent<CharacterRotation>();
        }

        #endregion

        #region IInputReceiver

        public void ListenToInputEvents(InputManager input)
        {
            input.P_ScrollFloat.E_ValueUpdated  += SetScrollValue;
            input.P_MouseVector2.E_ValueUpdated += SetLookVector;

            input.P_ActionButton.E_ButtonPress   += ActionButtonPresses;
            input.P_ActionButton.E_ButtonRelease += ActionButtonReleased;

            input.P_PrimaryFireButton.E_ButtonPress   += BeginInteraction;
            input.P_PrimaryFireButton.E_ButtonRelease += PrimaryInteractionButtonReleased;

            input.P_SecondaryFireButton.E_ButtonPress   += SecondaryInteractionButtonPressed;
            input.P_SecondaryFireButton.E_ButtonRelease += SecondaryInteractionButtonReleased;
        }

        public void StopListeningToInputEvents(InputManager input)
        {
            input.P_ScrollFloat.E_ValueUpdated  -= SetScrollValue;
            input.P_MouseVector2.E_ValueUpdated -= SetLookVector;

            input.P_ActionButton.E_ButtonPress   -= ActionButtonPresses;
            input.P_ActionButton.E_ButtonRelease -= ActionButtonReleased;

            input.P_PrimaryFireButton.E_ButtonPress   -= BeginInteraction;
            input.P_PrimaryFireButton.E_ButtonRelease -= PrimaryInteractionButtonReleased;

            input.P_SecondaryFireButton.E_ButtonPress   -= SecondaryInteractionButtonPressed;
            input.P_SecondaryFireButton.E_ButtonRelease -= SecondaryInteractionButtonReleased;
        }

        #endregion
    }
}