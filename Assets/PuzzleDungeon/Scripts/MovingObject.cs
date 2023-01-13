using DG.Tweening;
using UnityEngine;

namespace PuzzleDungeon
{
    public class MovingObject : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private Transform moveTo;
        [SerializeField] private float     movementTime;
        [SerializeField] private Ease      ease;

        private bool      _moved;
        private bool      _tweenInProgress;
        private Vector3   _initialPosition;

        public void SwitchPosition()
        {
            if (!_moved)
            {
                Move();
                return;
            }
            
            GoBack();
        }
        
        public void Move()
        {
            if (_moved || _tweenInProgress)
            {
                return;
            }

            _tweenInProgress = true;
            _initialPosition = transform.position;
            var tween = rigidbody.DOMove(moveTo.position, movementTime);
            tween.SetEase(ease);
            tween.onComplete += () =>
            {
                _moved           = true;
                _tweenInProgress = false;
            };
        }

        public void GoBack()
        {
            if (!_moved || _tweenInProgress)
            {
                return;
            }
            
            _tweenInProgress = true;
            var tween = rigidbody.DOMove(_initialPosition, movementTime);
            tween.SetEase(ease);
            tween.onComplete += () 
                =>
            {
                _moved           = false;
                _tweenInProgress = false;
            };
        }
    }
}