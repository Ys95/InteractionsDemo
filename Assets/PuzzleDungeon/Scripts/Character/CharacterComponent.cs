using PuzzleDungeon.Input;
using UnityEngine;

namespace PuzzleDungeon.Character
{
    public abstract class CharacterComponent : MonoBehaviour
    {
        public CharacterHub P_CharacterHub { get; set; }

        public virtual void Initialize()
        {
        }

        public virtual void UpdateAnimator()
        {
        }

        public virtual void ProcessUpdate()
        {
        }

        public virtual void ProcessHubEnable()
        {
        }

        public virtual void ProcessHubDisable()
        {
        }

        public virtual void ProcessFixedUpdate()
        {
        }

        public virtual void ProcessLateUpdate()
        {
        }

        public virtual void SetAnimatorParameters()
        {
        }
    }
}