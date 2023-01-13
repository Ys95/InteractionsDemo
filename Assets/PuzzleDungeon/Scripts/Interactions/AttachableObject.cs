using PuzzleDungeon.Character;
using UnityEngine;

namespace PuzzleDungeon.Interactions
{
    public class ParentableObject : Interactable
    {
        [SerializeField] private Vector3   localPositionWhenParented;
        [SerializeField] private Vector3   localRotationWhenParented;
        
        #region Interactable
        
        public override void PrimaryInteractionButtonReleased()
        {
            EndInteraction();
        }

        public override void StartInteraction(CharacterInteractions initiator)
        {
            base.StartInteraction(initiator);
            transform.SetParent(initiator.P_GrabAnchor);
            transform.localPosition = localPositionWhenParented;
            transform.localEulerAngles = localRotationWhenParented;
        }

        public override void EndInteraction()
        {
            base.EndInteraction();
            transform.SetParent(null);
        }

        #endregion
    }
}