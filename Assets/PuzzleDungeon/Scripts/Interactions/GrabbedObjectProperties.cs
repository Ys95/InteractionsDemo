using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemist.Scripts.Scriptable
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
        
        public float GrabForce;
        public float MaxDistanceFromGrabbedObject;

        [Space]
        public float GrabbedDrag;
        public float                      GrabbedAngularDrag;
        public float                      ThrowForce;
        public bool                       MakeObjectKinematic;
        [SerializeField] private RigidbodyConstraintsEditor constraintsEditor;
        public bool                       ShouldMoveOnlyAnchorWhileGrabbing;
        public Vector2              GrabAnchorLocalConstraints;
        public bool                 ModifyMouseSensitivity;
        public float                MouseSensitivity;

        public RigidbodyConstraints P_RigidbodyConstraints => constraintsEditor.GetConstraints();
    }
}