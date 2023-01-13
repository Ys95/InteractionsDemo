using System;
using System.Collections;
using Alchemist.Scripts.Scriptable;
using PuzzleDungeon.Character;
using UnityEngine;

namespace PuzzleDungeon.Interactions
{
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
        [SerializeField] private bool                         overrideGrabPropertiesPreset;
        [SerializeField] private GrabbedObjectProperties      overridedGrabProperties;

        private GrabbedBodyInitialProperties _bodyInitialProperties;
        private Collider                     _collider;

        public Rigidbody P_Rigidbody => targetRigidbody;
        public Collider  P_Collider  => _collider;

        private void Awake()
        {
            _collider       = GetComponent<Collider>();
            targetRigidbody = GetComponent<Rigidbody>();
        }

        private GrabbedObjectProperties GetGrabProperties()
        {
            if (overrideGrabPropertiesPreset || grabbedObjectPropertiesPreset == null)
            {
                return overridedGrabProperties;
            }

            return grabbedObjectPropertiesPreset.P_GrabbedObjectProperties;
        }
        
        private IEnumerator CO_Grab()
        {
            var grabProperties = GetGrabProperties();
            
            while (P_InteractionInProgress)
            {
                var distanceToTarget = Vector3.Distance(P_Initiator.P_GrabAnchor.position, targetRigidbody.position);
                if (distanceToTarget >= grabProperties.MaxDistanceFromGrabbedObject)
                {
                    EndInteraction();
                    yield break;
                }

                if (distanceToTarget < 0.1f)
                {
                    yield return null;
                    continue;
                }
                
                if (targetRigidbody.isKinematic)
                {
                    targetRigidbody.MovePosition(Vector3.Lerp(targetRigidbody.position, P_Initiator.P_GrabAnchor.position, Time.deltaTime * grabProperties.GrabForce));
                    yield return null;
                    continue;
                }
                
                var dir = P_Initiator.P_GrabAnchor.position - targetRigidbody.position;
                targetRigidbody.AddForce(dir * grabProperties.GrabForce);
               
                yield return null;
            }
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

            if (grabProperties.ShouldMoveOnlyAnchorWhileGrabbing)
            {
                initiator.MoveOnlyGrabAnchorMode(true, grabProperties.GrabAnchorLocalConstraints);
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
            
            if (grabProperties.ShouldMoveOnlyAnchorWhileGrabbing)
            {
                P_Initiator.MoveOnlyGrabAnchorMode(false);
            }
            
            base.EndInteraction();
        }

        #endregion
    }
}