using UnityEngine;
using System.Collections;

public class HeroAnimation : MonoBehaviour
{
    private Animator animator;
    private Hero hero;

    void Start()
    {
        animator = GetComponent<Animator>();
        hero = GetComponent<Hero>();
    }

    void Update()
    {
        updateAnimator();
    }

    private void updateAnimator()
    {
        animator.SetBool("dashing", hero.IsDashing);
        animator.SetBool("standing", hero.IsStanding);
        animator.SetBool("jumping", hero.IsJumping);
        animator.SetBool("shooting", hero.IsShooting);
        animator.SetBool("scaling", hero.IsScaling);
        animator.SetFloat("health", hero.Health);
        animator.SetFloat("speed", hero.Speed);
    }
}
