using System;
using Alchemist;
using PuzzleDungeon.Character;
using UnityEngine;

namespace PuzzleDungeon.Interactions
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] protected Outline outline;
        [SerializeField] protected bool    changeLayerOnInteraction = true;

        protected bool _isHighlighted;
        protected int  _defaultLayer;

        public event Action E_InteractionStarted;
        public event Action E_InteractionEnded;
        
        public          CharacterInteractions P_Initiator             { get; protected set; }
        public          bool                  P_InteractionInProgress { get; protected set; }

        public virtual void HighlightObject()
        {
            if(_isHighlighted || outline==null)
            {
                return;
            }

            outline.enabled = true;
            _isHighlighted = true;
        }

        public virtual void StopHighlightingObject()
        {
            if(!_isHighlighted)
            {
                return;
            }

            outline.enabled = false;
            _isHighlighted  = false;
        }
        
        public virtual void PrimaryInteractionButtonReleased()
        {
        }
        
        public virtual void SecondaryInteractionButtonPressed()
        {
        }
        
        public virtual void SecondaryInteractionButtonReleased()
        {
        }

        public virtual void StartInteraction(CharacterInteractions initiator)
        {
            P_InteractionInProgress = true;
            P_Initiator             = initiator;
            E_InteractionStarted?.Invoke();
            _defaultLayer    = gameObject.layer;
            if (changeLayerOnInteraction)
            {
                gameObject.layer = Layers.InteractedWithInteractable;
            }
        }

        public virtual void EndInteraction()
        {
            P_InteractionInProgress = false;
            E_InteractionEnded?.Invoke();
            P_Initiator      = null;
            gameObject.layer = _defaultLayer;
        }
    }
}