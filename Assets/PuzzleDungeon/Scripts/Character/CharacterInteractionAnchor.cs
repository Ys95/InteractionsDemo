using System;
using PuzzleDungeon.Interactions;
using UnityEngine;

namespace PuzzleDungeon.Character
{
    public class CharacterInteractionAnchor : MonoBehaviour
    {
        [SerializeField] private CharacterInteractions characterInteractions;

        [Space]
        [SerializeField] private GameObject anchorModel;

        private void OnEnable()
        {
            characterInteractions.E_InteractionStarted += ShowAnchor;
            characterInteractions.E_InteractionEnded   += HideAnchor;
        }

        private void OnDisable()
        {
            characterInteractions.E_InteractionStarted -= ShowAnchor;
            characterInteractions.E_InteractionEnded   -= HideAnchor;
        }

        private void ShowAnchor()
        {
            anchorModel.gameObject.SetActive(true);
        }

        private void HideAnchor()
        {
            anchorModel.gameObject.SetActive(false);
        }
    }
}