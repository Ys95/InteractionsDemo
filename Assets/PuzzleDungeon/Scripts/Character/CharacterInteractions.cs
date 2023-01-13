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
        private struct GrabAnchor
        {
            public Transform Pivot;
            public Transform Anchor;
            public float     MaxMovementSpeed;
            public Vector2   MovementConstraints;

            private Vector3 _defaultPivotLocalPosition;
            private Vector3 _defaultAnchorLocalPosition;
            private Vector2 _defaultMovementConstraints;

            public void Initialize()
            {
                _defaultPivotLocalPosition  = Pivot.localPosition;
                _defaultAnchorLocalPosition = Anchor.localPosition;
                _defaultMovementConstraints = MovementConstraints;
            }

            public void RestoreDefaultConstraints() => MovementConstraints = _defaultMovementConstraints;
            
            public void RepositionPivot(Vector3  position, bool localSpace) => Reposition(Pivot,  position, localSpace);
            
            public void RepositionAnchor(Vector3 position, bool localSpace) => Reposition(Anchor, position, localSpace);

            public void RestoreAnchorPosition() => Reposition(Anchor, _defaultAnchorLocalPosition, true);
            
            public void RestorePivotPosition() => Reposition(Pivot, _defaultPivotLocalPosition, true);
            
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
        
        
        [SerializeField] private Transform        interactionCheckRayStartPoint;
        [SerializeField] private LayerMask        interactionLayers;
        [SerializeField] private float            interactionCheckRayMaxLenght;
        [SerializeField] private float            raycastFrequency;
        [SerializeField] private List<GameObject> interactionsNodes;

        [Header("Grab Anchor")]
        [SerializeField] private GrabAnchor grabAnchor;

        private RaycastHit[]      _raycastHits = new RaycastHit[5];
        private float             _raycastTimer;
        private float             _grabAnchorHorizontalMovement;
        private float             _grabAnchorVerticalMovement;
        private bool              _interactionAllowed;
        private bool              _shouldMoveGrabAnchor;
        private Interactable      _previousInteractableInRange;
        private Interactable      _currentInteractableInRange;
        private Interactable      _currentInteraction;
        private Vector3           _initialAnchorLocalPosition;
        private CharacterRotation _characterRotation;
        
        public Transform P_GrabAnchor => grabAnchor.Anchor;
        
        public Vector2 P_RawLookVector { get; private set; }

        private void SearchForInteractable()
        {
            var size = Physics.RaycastNonAlloc(interactionCheckRayStartPoint.position, interactionCheckRayStartPoint.forward, _raycastHits, interactionCheckRayMaxLenght, interactionLayers, QueryTriggerInteraction.Collide);

            Interactable detectedInteractable = null;

            for (int i = 0; i < size; i++)
            {
                var interactable = _raycastHits[i].collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    detectedInteractable = interactable;
                    break;
                }
            }

            ProcessSearchResult(detectedInteractable);
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

            if (_currentInteraction         != null)
            {
                return;
            }

            _currentInteraction = _currentInteractableInRange;
            _currentInteraction.StopHighlightingObject();
            _currentInteraction.E_InteractionEnded += OnInteractionEndedByInteractable;
            _currentInteraction.StartInteraction(this);
        }

        public void BeginInteraction(Interactable interactable)
        {
            if (_currentInteraction != null)
            {
                return;
            }

            if (_currentInteractableInRange != null)
            {
                _currentInteractableInRange.StopHighlightingObject();
            }

            _currentInteraction = interactable;
            _currentInteraction.E_InteractionEnded += OnInteractionEndedByInteractable;
            _currentInteraction.StartInteraction(this);
        }
        

        private void OnInteractionEndedByInteractable()
        {
            _currentInteraction.E_InteractionEnded -= OnInteractionEndedByInteractable;
            _currentInteraction                    =  null;
        }

        private void EndInteraction()
        {
            if (_currentInteraction == null)
            {
                return;
            }

            _currentInteraction.EndInteraction();
            _currentInteraction = null;
        }
        
        public override void Initialize()
        {
            grabAnchor.Initialize();
            _interactionAllowed = true;
            _characterRotation  = P_CharacterHub.GetCharacterComponent<CharacterRotation>();
        }

        private void OnInteractionStarted() => _interactionAllowed = false;
        private void OnInteractionEnded()   => _interactionAllowed = true;
        
        private void PrimaryInteractionButtonReleased()
        {
            if(_currentInteraction==null)
            {
                return;
            }

            _currentInteraction.PrimaryInteractionButtonReleased();
        }
        
        private void SecondaryInteractionButtonPressed()
        {
            if(_currentInteraction ==null)
            {
                return;
            }

            _currentInteraction.SecondaryInteractionButtonPressed();
        }
        
        private void SecondaryInteractionButtonReleased()
        {
            if(_currentInteraction ==null)
            {
                return;
            }

            _currentInteraction.SecondaryInteractionButtonReleased();
        }

        public void MoveOnlyGrabAnchorMode(bool enable, Vector2? constraints = null)
        {
            if (enable)
            {
                _characterRotation.LockRotation();
                _shouldMoveGrabAnchor = true;
                
                if (constraints.HasValue)
                {
                    grabAnchor.MovementConstraints = constraints.Value;
                }
                
                return;
            }

            _characterRotation.UnlockRotation();
            _shouldMoveGrabAnchor = false;
            grabAnchor.RestoreAnchorPosition();
            grabAnchor.RestorePivotPosition();
            grabAnchor.RestoreDefaultConstraints();
        }

        public void RecenterAnchor(Vector3 pivotPosition, Vector3? grabAnchorPosition = null)
        {
            grabAnchor.RepositionPivot(pivotPosition, false);

            if (grabAnchorPosition.HasValue)
            {
                grabAnchor.RepositionAnchor(grabAnchorPosition.Value, false);
                _grabAnchorVerticalMovement   = grabAnchor.Anchor.localPosition.y;
                _grabAnchorHorizontalMovement = grabAnchor.Anchor.localPosition.x;
            }
            else
            {
                grabAnchor.RestoreAnchorPosition();
                _grabAnchorVerticalMovement   = 0f;
                _grabAnchorHorizontalMovement = 0f;
            }
        }

        public override void ProcessHubEnable()
        {
        }

        public override void ProcessHubDisable()
        {
        }

        private void MoveGrabAnchor()
        {
            if (!_shouldMoveGrabAnchor)
            {
                return;
            }

            _grabAnchorHorizontalMovement += P_RawLookVector.x * (grabAnchor.MaxMovementSpeed * Time.deltaTime);
            _grabAnchorVerticalMovement   += P_RawLookVector.y * (grabAnchor.MaxMovementSpeed * Time.deltaTime);

            _grabAnchorHorizontalMovement = Mathf.Clamp(_grabAnchorHorizontalMovement, -grabAnchor.MovementConstraints.x, grabAnchor.MovementConstraints.x);
            _grabAnchorVerticalMovement   = Mathf.Clamp(_grabAnchorVerticalMovement,   -grabAnchor.MovementConstraints.y, grabAnchor.MovementConstraints.y);
            
            grabAnchor.RepositionAnchor(new Vector3(_grabAnchorHorizontalMovement, _grabAnchorVerticalMovement, 0f), true);
        }

        public override void ProcessUpdate()
        {
            MoveGrabAnchor();
            
            if (!_interactionAllowed || _currentInteraction!=null)
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

        #region IInputReceiver

        public void ReceiveInputUpdate(InputManager input)
        {
            P_RawLookVector = input.P_MouseVector2.P_CurrentValue;
        }

        public void ListenToInputEvents(InputManager input)
        {
            input.P_PrimaryFireButton.E_ButtonPress   += BeginInteraction;
            input.P_PrimaryFireButton.E_ButtonRelease += PrimaryInteractionButtonReleased;

            input.P_SecondaryFireButton.E_ButtonPress   += SecondaryInteractionButtonPressed;
            input.P_SecondaryFireButton.E_ButtonRelease += SecondaryInteractionButtonReleased;
        }

        public void StopListeningToInputEvents(InputManager input)
        {
            input.P_PrimaryFireButton.E_ButtonPress   -= BeginInteraction;
            input.P_PrimaryFireButton.E_ButtonRelease -= PrimaryInteractionButtonReleased;

            input.P_SecondaryFireButton.E_ButtonPress   -= SecondaryInteractionButtonPressed;
            input.P_SecondaryFireButton.E_ButtonRelease -= SecondaryInteractionButtonReleased;
        }

        #endregion
    }
}