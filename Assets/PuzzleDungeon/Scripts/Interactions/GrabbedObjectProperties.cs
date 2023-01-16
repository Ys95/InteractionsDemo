using System;
using PuzzleDungeon.Character;
using UnityEngine;

namespace PuzzleDungeon.Interactions
{
    [Serializable]
    public struct GrabbedObjectProperties
    {
        [Serializable]
        private struct RigidbodyConstraintsEditor
        {
            [SerializeField] private bool _freezePositionX;
            [SerializeField] private bool _freezePositionY;
            [SerializeField] private bool _freezePositionZ;

            [Space]
            [SerializeField] private bool _freezeRotationX;
            [SerializeField] private bool _freezeRotationY;
            [SerializeField] private bool _freezeRotationZ;

            public RigidbodyConstraints GetConstraints()
            {
                RigidbodyConstraints c = RigidbodyConstraints.None;

                if (_freezePositionX)
                {
                    c = c | RigidbodyConstraints.FreezePositionX;
                }

                if (_freezePositionY)
                {
                    c = c | RigidbodyConstraints.FreezePositionY;
                }

                if (_freezePositionZ)
                {
                    c = c | RigidbodyConstraints.FreezePositionZ;
                }

                if (_freezeRotationX)
                {
                    c = c | RigidbodyConstraints.FreezeRotationX;
                }

                if (_freezeRotationY)
                {
                    c = c | RigidbodyConstraints.FreezeRotationY;
                }

                if (_freezeRotationZ)
                {
                    c = c | RigidbodyConstraints.FreezeRotationZ;
                }

                return c;
            }
        }

        [Header("Grab")]
        public float GrabForce;
        public bool  MultiplyGrabForceByDistance;
        public float ThrowForce;
        public float MaxDistanceFromGrabbedObject;

        [Header("Grabbed body")]
        public float GrabbedDrag;
        public                   float                      GrabbedAngularDrag;
        [SerializeField] private RigidbodyConstraintsEditor constraintsEditor;

        [Header("Anchor")]
        public bool AnchorMovingMode;
        public CharacterInteractions.AnchorMovingModeMouseAxisMapping AnchorMovingModeMouseAxisMapping;
        public bool                                                   MoveAnchorOnObjectPosition;
        public Vector3                                                AnchorMovingModeLocalConstraints;

        [Header("Mouse")]
        public bool ModifyMouseSensitivity;
        public float MouseSensitivity;

        public RigidbodyConstraints P_RigidbodyConstraints => constraintsEditor.GetConstraints();
    }
}