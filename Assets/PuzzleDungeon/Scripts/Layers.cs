using UnityEngine;

namespace Alchemist
{
    public static class Layers
    {
        public static readonly int InteractedWithInteractable = LayerMask.NameToLayer("InteractedWithInteractable");
        public static readonly int Interactable               = LayerMask.NameToLayer("Interactable");
    }
}