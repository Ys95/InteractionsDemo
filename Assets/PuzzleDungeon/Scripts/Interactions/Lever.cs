using System;
using System.Collections;
using PuzzleDungeon.Character;
using PuzzleDungeon.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace PuzzleDungeon.Interactions
{
    public class Lever : MonoBehaviour
    {
        public enum LeverState
        {
            On,
            Off,
            Neutral
        }

        [SerializeField] private HingeJoint      hingeJoint;
        [SerializeField] private GrabbableObject handle;

        [Header("Axis")]
        [SerializeField] private Axis monitoredAxis;
        [SerializeField] private MinMaxFloat axisAngleRangeToSwitchOn;
        [SerializeField] private MinMaxFloat axisAngleRangeToSwitchOff;

        [Space]
        [SerializeField] private UnityEvent leverStateOn;
        [SerializeField] private UnityEvent leverStateOff;
        [SerializeField] private UnityEvent leverStateNeutral;

        private StateMachine<LeverState> _leverState;

        public StateMachine<LeverState> P_LeverState => _leverState;

        private void Awake()
        {
            _leverState = new StateMachine<LeverState>(LeverState.Neutral, true);
            DetectState();
        }

        private void OnEnable()
        {
            handle.E_InteractionStarted += OnHandleGrabbed;
            _leverState.E_StateChanged  += OnLeverStateChange;
        }

        private void OnDisable()
        {
            handle.E_InteractionStarted -= OnHandleGrabbed;
            _leverState.E_StateChanged  -= OnLeverStateChange;
        }

        private void OnLeverStateChange()
        {
            switch (_leverState.CurrentState)
            {
                case LeverState.On:
                    leverStateOn?.Invoke();
                    break;

                case LeverState.Off:
                    leverStateOff?.Invoke();
                    break;

                case LeverState.Neutral:
                    leverStateNeutral?.Invoke();
                    break;
            }
        }

        private void OnHandleGrabbed() => StartCoroutine(CO_MonitorAngle());

        private IEnumerator CO_MonitorAngle()
        {
            while (handle.P_InteractionInProgress)
            {
                DetectState();
                yield return new WaitForFixedUpdate();
            }
        }

        private void DetectState()
        {
            if (hingeJoint.angle >= axisAngleRangeToSwitchOn.Min && hingeJoint.angle <= axisAngleRangeToSwitchOn.Max)
            {
                _leverState.ChangeState(LeverState.On);
                return;
            }

            if (hingeJoint.angle >= axisAngleRangeToSwitchOff.Min && hingeJoint.angle <= axisAngleRangeToSwitchOff.Max)
            {
                _leverState.ChangeState(LeverState.Off);
                return;
            }

            _leverState.ChangeState(LeverState.Neutral);
        }

        private void OnDrawGizmos()
        {
            if (_leverState == null)
            {
                return;
            }

            switch (_leverState.CurrentState)
            {
                case LeverState.On:
                    Gizmos.color = Color.green;
                    break;

                case LeverState.Off:
                    Gizmos.color = Color.red;
                    break;

                case LeverState.Neutral:
                    Gizmos.color = Color.yellow;
                    break;
            }

            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}