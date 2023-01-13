using PuzzleDungeon.Character;
using UnityEngine;
using UnityEngine.Events;

namespace PuzzleDungeon.Interactions
{
    public class InteractableEventButton : Interactable
    {
        [SerializeField] private UnityEvent buttonPress;

        public override void StartInteraction(CharacterInteractions initiator)
        {
            base.StartInteraction(initiator);
            buttonPress?.Invoke();
            EndInteraction();
        }
    }
}