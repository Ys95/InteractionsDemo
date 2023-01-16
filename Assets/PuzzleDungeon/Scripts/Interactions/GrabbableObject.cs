using System;
using System.Collections;
using PuzzleDungeon.Character;
using PuzzleDungeon.Scriptable;
using UnityEngine;

namespace PuzzleDungeon.Interactions
{
    [RequireComponent(typeof(Collider))]
    public class GrabbableObject : Interactable
    {
        [Serializable]
        private struct GrabbedBodyInitialProperties
        {
            public float                Drag;
            public float                AngularDrag;
            public float                Mass;
            public bool                 IsKinematic;
            public bool                 UseGravity;
            public RigidbodyConstraints Constraints;
        }

        [SerializeField] private Rigidbody                    targetRigidbody;
        [SerializeField] private GrabbedObjectPropertiesSetup grabbedObjectPropertiesPreset;
        [Tooltip("Where player 'hands' will go while grabbing. Defaults to rigidbody if empty")]
        [SerializeField] private Transform grabPoint;
        [Tooltip("Defaults to grabPoint if empty")]
        [SerializeField] private Transform objectCenter;

        private GrabbedBodyInitialProperties _bodyInitialProperties;
        private Collider                     _collider;

        public Rigidbody P_Rigidbody => targetRigidbody;
        public Collider  P_Collider  => _collider;

        public GrabbedObjectPropertiesSetup P_GrabbedObjectPropertiesPreset
        {
            get => grabbedObjectPropertiesPreset;
            set => grabbedObjectPropertiesPreset = value;
        }

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider>();
            if (targetRigidbody == null)
            {
                targetRigidbody = GetComponent<Rigidbody>();
            }
        }

        private GrabbedObjectProperties GetGrabProperties()
        {
            return grabbedObjectPropertiesPreset.P_GrabbedObjectProperties;
        }

        private Transform GetGrabPoint() => grabPoint == null ? targetRigidbody.transform : grabPoint;

        private IEnumerator CO_Grab()
        {
            var grabProperties = GetGrabProperties();

            while (P_InteractionInProgress)
            {
                CalculateGrab(grabProperties);
                yield return null;
            }
        }

        private void CalculateGrab(GrabbedObjectProperties grabProperties)
        {
            var distanceToTarget = Vector3.Distance(P_Initiator.P_GrabAnchor.position,             targetRigidbody.position);
            var distanceToPlayer = Vector3.Distance(P_Initiator.P_CharacterHub.transform.position, targetRigidbody.position);
            if (distanceToPlayer >= grabProperties.MaxDistanceFromGrabbedObject)
            {
                EndInteraction();
                return;
            }

            if (distanceToTarget < 0.1f)
            {
                return;
            }

            if (targetRigidbody.isKinematic)
            {
                return;
            }

            var dir = P_Initiator.P_GrabAnchor.position - targetRigidbody.position;
            targetRigidbody.AddForce(dir * (grabProperties.GrabForce * (grabProperties.MultiplyGrabForceByDistance ? distanceToTarget : 1)));
        }

        private void Throw()
        {
            targetRigidbody.AddForce(GetGrabProperties().ThrowForce * P_Initiator.P_GrabAnchor.transform.forward, ForceMode.Impulse);
            EndInteraction();
        }

        #region Interactable

        public override void PrimaryInteractionButtonReleased()
        {
            EndInteraction();
        }

        public override void SecondaryInteractionButtonPressed()
        {
            Throw();
        }

        public override void StartInteraction(CharacterInteractions initiator)
        {
            var grabProperties = GetGrabProperties();

            _bodyInitialProperties.Drag        = targetRigidbody.drag;
            _bodyInitialProperties.AngularDrag = targetRigidbody.angularDrag;
            _bodyInitialProperties.Constraints = targetRigidbody.constraints;
            _bodyInitialProperties.UseGravity  = targetRigidbody.useGravity;

            targetRigidbody.useGravity  = false;
            targetRigidbody.drag        = grabProperties.GrabbedDrag;
            targetRigidbody.angularDrag = grabProperties.GrabbedAngularDrag;
            targetRigidbody.constraints = grabProperties.P_RigidbodyConstraints;

            if (grabProperties.ModifyMouseSensitivity)
            {
                initiator.P_CharacterHub.P_InputManager.TemporaryEditMouseSensitivity(grabProperties.MouseSensitivity);
            }

            if (grabProperties.AnchorMovingMode)
            {
                initiator.EnableAnchorMovingMode(grabProperties.AnchorMovingModeMouseAxisMapping, grabProperties.AnchorMovingModeLocalConstraints);
            }

            if (grabProperties.MoveAnchorOnObjectPosition)
            {
                var grabPoint = GetGrabPoint().position;
                initiator.RepositionAnchorPivot(objectCenter != null ? objectCenter.position : grabPoint, grabPoint);
            }

            base.StartInteraction(initiator);
            StartCoroutine(CO_Grab());
        }

        public override void EndInteraction()
        {
            var grabProperties = GetGrabProperties();

            targetRigidbody.drag        = _bodyInitialProperties.Drag;
            targetRigidbody.angularDrag = _bodyInitialProperties.AngularDrag;
            targetRigidbody.constraints = _bodyInitialProperties.Constraints;
            targetRigidbody.useGravity  = _bodyInitialProperties.UseGravity;
            targetRigidbody.isKinematic = _bodyInitialProperties.IsKinematic;

            if (grabProperties.ModifyMouseSensitivity)
            {
                P_Initiator.P_CharacterHub.P_InputManager.RestoreInitialMouseSensitivity();
            }

            if (grabProperties.AnchorMovingMode)
            {
                P_Initiator.DisableAnchorMovingMode();
            }
            else
            {
                P_Initiator.ResetAnchor();
            }

            base.EndInteraction();
        }

        #endregion
    }
}