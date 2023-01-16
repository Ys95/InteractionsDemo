using System.Collections;
using System.Collections.Generic;
using PuzzleDungeon.Character;
using PuzzleDungeon.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace PuzzleDungeon.Interactions
{
    public class SpinningMechanism : MonoBehaviour
    {
        [SerializeField] private GrabbableObject spinnerHandle;
        [SerializeField] private Transform       spinPoint;
        [SerializeField] private Rigidbody       spinnedRigidbody;

        [Header("Spinning values")]
        [SerializeField] private float distanceUpdateFrequency = 0.05f;
        [SerializeField] private float minDistanceTraveledToRegister     = 0.02f;
        [SerializeField] private float minDistanceTraveledToRegisterTick = 0.2f;
        [SerializeField] private Axis  directionMonitoringSpinAxis;

        [Space]
        [SerializeField] private UnityEvent<float> spinDistanceUpdate;
        [SerializeField] private UnityEvent spinTickInPositiveDirection;
        [SerializeField] private UnityEvent spinTickInNegativeDirection;

        private float _traveledDistance;
        private float _traveledSinceLastTick;
        private float _spinDirection;

        private void OnEnable()
        {
            spinnerHandle.E_InteractionStarted += OnInteractionStarted;
            spinnerHandle.E_InteractionEnded   += OnInteractionEnded;
        }

        private void OnDisable()
        {
            spinnerHandle.E_InteractionStarted -= OnInteractionStarted;
            spinnerHandle.E_InteractionEnded   -= OnInteractionEnded;
        }

        private void OnInteractionStarted()
        {
            StartCoroutine(CO_MonitorSpinProgress());
        }

        private void OnInteractionEnded()
        {
            spinnerHandle.P_Rigidbody.velocity        = Vector3.zero;
            spinnerHandle.P_Rigidbody.angularVelocity = Vector3.zero;
            spinnedRigidbody.velocity                 = Vector3.zero;
            spinnedRigidbody.angularVelocity          = Vector3.zero;
        }

        private IEnumerator CO_MonitorSpinProgress()
        {
            _traveledDistance      = 0f;
            _traveledSinceLastTick = 0f;

            var previousPos = spinPoint.position;

            while (spinnerHandle.P_InteractionInProgress)
            {
                var distance = Vector3.Distance(previousPos, spinPoint.position);
                previousPos = spinPoint.position;

                if (distance < minDistanceTraveledToRegister)
                {
                    yield return new WaitForSeconds(distanceUpdateFrequency);

                    continue;
                }

                _traveledDistance      += distance;
                _traveledSinceLastTick += distance;
                _spinDirection         =  spinnedRigidbody.angularVelocity.GetVector3Axis(directionMonitoringSpinAxis) >= 0 ? 1 : -1;

                if (_traveledSinceLastTick >= minDistanceTraveledToRegisterTick)
                {
                    spinDistanceUpdate?.Invoke(_traveledDistance);
                    _traveledSinceLastTick = 0;

                    if (_spinDirection >= 0)
                    {
                        spinTickInPositiveDirection?.Invoke();
                    }
                    else
                    {
                        spinTickInNegativeDirection?.Invoke();
                    }
                }

                yield return new WaitForSeconds(distanceUpdateFrequency);
            }
        }
    }
}