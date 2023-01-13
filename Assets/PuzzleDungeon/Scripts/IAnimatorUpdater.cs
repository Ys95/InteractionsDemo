using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tide
{
    [Serializable]
    public abstract class AnimatorParameter
    {
        [SerializeField] protected string name;

        protected Animator animator;

        public string P_Name => name;

        public void Initialize(Animator animator)
        {
            this.animator = animator;
        }
    }

    [Serializable]
    public class FloatAnimatorParameter : AnimatorParameter
    {
        public float P_FloatValue => animator.GetFloat(name);

        public void UpdateAnimator(float val)
        {
            if(animator==null)
            {
                return;
            }

            animator.SetFloat(name, val);
        }
    }

    [Serializable]
    public class BoolAnimatorParameter : AnimatorParameter
    {
        public bool P_BoolValue => animator.GetBool(name);

        public void UpdateAnimator(bool val)
        {
            if(animator ==null)
            {
                return;
            }

            animator.SetBool(name, val);
        }
    }
}