using UnityEngine;

public class FrameAnimationController : MonoBehaviour
{
    private Animator animator;
    private bool hasPlayedFirstAnimation = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Zanusi1 0");
    }

    void Update()
    {
        if (!hasPlayedFirstAnimation && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.75f)
        {
            hasPlayedFirstAnimation = true;
            animator.Play("zanusi2");
        }
    }
}
