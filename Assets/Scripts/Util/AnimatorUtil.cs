using UnityEngine;

namespace Util
{
    public static class AnimatorUtil
    {
        /// <summary>
        /// Reset all animation keys (triggers, booleans, integers) of an animation
        /// and set the animation to the default animation state.
        /// </summary>
        /// <param name="animator">Animator to be reset.</param>
        /// <param name="defaultAnimationState">Name of the default animation state.</param>
        public static void ResetAnimator(Animator animator, string defaultAnimationState)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Trigger:
                        animator.ResetTrigger(param.name);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(param.name, false);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(param.name, 0);
                        break;
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(param.name, 0f);
                        break;
                }
            }
        
            animator.Play(defaultAnimationState);
        }
    }
}
