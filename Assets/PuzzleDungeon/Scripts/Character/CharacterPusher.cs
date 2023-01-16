using System;
using UnityEngine;

namespace PuzzleDungeon.Character
{
    public class CharacterPusher : CharacterComponent
    {
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private float     pushPower;

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!Tools.Tools.LayerInLayerMask(targetLayers, hit.gameObject.layer))
            {
                return;
            }

            var rigidBody = hit.collider.attachedRigidbody;

            if (rigidBody == null || rigidBody.isKinematic)
            {
                return;
            }

            // Dont push objects below player
            if (hit.moveDirection.y < -0.3)
            {
                return;
            }

            var pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            rigidBody.velocity = pushDirection * pushPower;
        }
    }
}