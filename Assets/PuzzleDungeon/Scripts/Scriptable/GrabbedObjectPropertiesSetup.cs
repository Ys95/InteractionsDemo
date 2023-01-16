using PuzzleDungeon.Interactions;
using UnityEngine;

namespace PuzzleDungeon.Scriptable
{
    [CreateAssetMenu(fileName = "_GrabbedObjectProperties", menuName = "PuzzleDungeon/Interactions", order = 0)]
    public class GrabbedObjectPropertiesSetup : ScriptableObject
    {
        [SerializeField] private GrabbedObjectProperties grabbedObjectProperties;

        public GrabbedObjectProperties P_GrabbedObjectProperties => grabbedObjectProperties;
    }
}