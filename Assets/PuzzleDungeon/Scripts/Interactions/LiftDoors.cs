using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace PuzzleDungeon.Interactions
{
    public class LiftDoors : MonoBehaviour
    {
        [SerializeField] private Transform doors;
        [SerializeField] private float     localYPositionWhenOpened;
        [SerializeField] private float     localYPositionWhenClosed;

        [Header("Tween movement")]
        [SerializeField] private float movementTime;
        [SerializeField] private Ease movementEase;

        [Header("Manual movement")]
        [SerializeField] private float distancePerMovementCall;
        [SerializeField] private float manualLiftSpeed;

        private TweenerCore<Vector3, Vector3, VectorOptions> _movementTween;
        private float                                        _manualLiftAmount;
        private float                                        _manualLiftDirection;
        private float                                        _maxManualMovementDown;
        private float                                        _maxManualMovementUp;

        public void Open()
        {
            if (_movementTween != null)
            {
                _movementTween.Kill();
            }

            _movementTween = doors.DOLocalMoveY(localYPositionWhenOpened, movementTime);
            _movementTween.SetEase(movementEase);
            _movementTween.Play();
        }

        public void Close()
        {
            if (_movementTween != null)
            {
                _movementTween.Kill();
            }

            _movementTween = doors.DOLocalMoveY(localYPositionWhenClosed, movementTime);
            _movementTween.SetEase(movementEase);
            _movementTween.Play();
        }

        public void ManualMoveDown()
        {
            if (_manualLiftDirection >= 0)
            {
                _manualLiftAmount = 0;
            }

            _manualLiftAmount    = 0;
            _manualLiftDirection = -1;

            var currentYPosition = doors.localPosition.y;
            _maxManualMovementDown = Mathf.Abs(localYPositionWhenClosed - currentYPosition);
            _manualLiftAmount      = Mathf.Min(_manualLiftAmount        + distancePerMovementCall, _maxManualMovementDown);
        }

        public void ManualMoveUp()
        {
            if (_manualLiftDirection < 0)
            {
                _manualLiftAmount = 0;
            }

            _manualLiftAmount    = 0;
            _manualLiftDirection = 1;

            var currentYPosition = doors.localPosition.y;
            _maxManualMovementUp = Mathf.Abs(localYPositionWhenOpened - currentYPosition);
            _manualLiftAmount    = Mathf.Min(_manualLiftAmount        + distancePerMovementCall, _maxManualMovementUp);
        }

        private void Update()
        {
            if (_manualLiftAmount <= 0)
            {
                return;
            }

            var moveAmount = manualLiftSpeed * Time.deltaTime;
            _manualLiftAmount -= moveAmount;

            var currentYPosition = doors.localPosition.y;
            var newYPosition     = currentYPosition + (moveAmount * _manualLiftDirection);

            newYPosition        = Mathf.Clamp(newYPosition, localYPositionWhenClosed, localYPositionWhenOpened);
            doors.localPosition = new Vector3(doors.localPosition.x, newYPosition, doors.localPosition.z);
        }
    }
}