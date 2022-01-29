using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    /*

    public AnimationClip victoryClip;
    public AnimationClip loseClip;
    */
    public AnimationClip clip;
    public AnimatorController animatorController;
    public Animator animator;

    private Animation _animation;

    public float delayAnimation;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        /*
        _animation = GetComponent<Animation>();
        
        //Get all the animations and put them in the clip
        _animation.AddClip(clip,clip.name);
        if (loseClip != null)
            _animation.AddClip(loseClip,loseClip.name);
        if (victoryClip != null)
            _animation.AddClip(victoryClip, victoryClip.name);
            */

        Invoke("PlayClip",delayAnimation);
    }

    public void PlayClip()
    {
        animator.runtimeAnimatorController = animatorController;
        //animator.Play(clip.name);
    }

    public void PlayVictoryClip()
    {
        animator.SetTrigger("Victory");
    }
    public void PlayLoseClip()
    {
        animator.SetTrigger("Lose");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
