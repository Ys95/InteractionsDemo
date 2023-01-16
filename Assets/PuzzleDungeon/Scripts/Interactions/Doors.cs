using System;
using UnityEngine;

namespace PuzzleDungeon.Interactions
{
    public class Doors : MonoBehaviour
    {
        [SerializeField] private HingeJoint      hinge;
        [SerializeField] private GrabbableObject handle1;
        [SerializeField] private GrabbableObject handle2;
        [SerializeField] private bool            doorsLocked;

        private bool _doorClosed;
        private bool _interacting;

        public void UnlockDoors() => doorsLocked = false;

        public void LockDoors()
        {
            if (!_doorClosed)
            {
                return;
            }

            doorsLocked = true;
        }

        private void CloseDoors()
        {
            if (_doorClosed)
            {
                return;
            }

            if (_interacting)
            {
                return;
            }

            var newLimits = hinge.limits;
            newLimits.max = 2;
            hinge.limits  = newLimits;
            _doorClosed   = true;
        }

        private void AllowToOpenDoors()
        {
            if (doorsLocked)
            {
                return;
            }

            if (!_doorClosed)
            {
                return;
            }

            var newLimits = hinge.limits;
            newLimits.max = 90;
            hinge.limits  = newLimits;
            _doorClosed   = false;
        }

        private void OnGrabEnded()
        {
            _interacting = false;
        }

        private void OnGrabStarted()
        {
            _interacting = true;
            AllowToOpenDoors();
        }

        private void OnEnable()
        {
            if (handle1 != null)
            {
                handle1.E_InteractionStarted += OnGrabStarted;
                handle1.E_InteractionEnded   += OnGrabEnded;
            }

            if (handle2 != null)
            {
                handle2.E_InteractionStarted += OnGrabStarted;
                handle2.E_InteractionEnded   += OnGrabEnded;
            }
        }

        private void OnDisable()
        {
            if (handle1 != null)
            {
                handle1.E_InteractionStarted -= OnGrabStarted;
                handle1.E_InteractionEnded   -= OnGrabEnded;
            }

            if (handle2 != null)
            {
                handle2.E_InteractionStarted -= OnGrabStarted;
                handle2.E_InteractionEnded   -= OnGrabEnded;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform != handle1.P_Rigidbody.transform)
            {
                return;
            }

            CloseDoors();
        }
    }
}