using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public AnimationClip clip;
    public AnimationClip victoryClip;
    public AnimationClip loseClip;

    private Animation _animation;

    public float delayAnimation;
    // Start is called before the first frame update
    void Start()
    {
        _animation = GetComponent<Animation>();
        
        //Get all the animations and put them in the clip
        _animation.AddClip(clip,clip.name);
        if (loseClip != null)
            _animation.AddClip(loseClip,loseClip.name);
        if (victoryClip != null)
            _animation.AddClip(victoryClip, victoryClip.name);
        
        Invoke("PlayClip",delayAnimation);
    }

    public void PlayClip()
    {
        _animation.Play(clip.name);
    }

    public void PlayVictoryClip()
    {
        _animation.Stop();
        _animation.Play(victoryClip.name);
    }
    public void PlayLoseClip()
    {
        _animation.Stop();
        _animation.Play(loseClip.name);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
