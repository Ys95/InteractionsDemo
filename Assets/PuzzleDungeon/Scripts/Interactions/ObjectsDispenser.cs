using PuzzleDungeon.Character;
using UnityEngine;

namespace PuzzleDungeon.Interactions
{
    public class ObjectsDispenser : Interactable
    {
        [SerializeField] private Interactable dispensedObject;
        [SerializeField] private Transform    spawnPos;
        [SerializeField] private bool         forceInteractionOnSpawn = true;
        
        public override void StartInteraction(CharacterInteractions initiator)
        {
            base.StartInteraction(initiator);
            var obj = Instantiate(dispensedObject, spawnPos.position, Quaternion.identity, null);
            EndInteraction();

            if (forceInteractionOnSpawn)
            {
                initiator.BeginInteraction(obj);
            }
        }
    }
}