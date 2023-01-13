using UnityEngine;

namespace Alchemist.Scripts.Scriptable
{
    [CreateAssetMenu(fileName = "_GrabbedObjectProperties", menuName = "Alchemist/Interactions", order = 0)]
    public class GrabbedObjectPropertiesSetup : ScriptableObject
    {
        [SerializeField] private GrabbedObjectProperties grabbedObjectProperties;

        public GrabbedObjectProperties P_GrabbedObjectProperties => grabbedObjectProperties;
    }
}