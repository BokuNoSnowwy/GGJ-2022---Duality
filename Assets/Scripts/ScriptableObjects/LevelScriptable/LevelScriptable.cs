using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class SFXScene
{
    public AudioClip audioClip;
    public float timerToPlay;
}

[Serializable]
public class SceneElementsSprite
{
    public Sprite sprite;
    public SortingLayer sortingLayer;
}

[Serializable]
public class SceneElementAnimation
{
    public AnimationClip animationClip;
    public SortingLayer sortingLayer;
    public AnimationClip victoryClip;
    public AnimationClip loseClip;

}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelScriptable", order = 1)]
public class LevelScriptable : ScriptableObject
{
    [Header("Day Part")] 
    //Characters that will be animated 
    public List<SceneElementAnimation> animationClipListDay = new List<SceneElementAnimation>();
    public List<SceneElementsSprite> spriteListDay = new List<SceneElementsSprite>();
    public float timerLevelDay;
    public AudioClip audioLevelDay;
    public List<SFXScene> sfxSceneList = new List<SFXScene>(); 

    [Header("Night Part")]
    public List<SceneElementAnimation> animationClipListNight = new List<SceneElementAnimation>();
    public List<SceneElementsSprite> spriteListNight = new List<SceneElementsSprite>();
    public int barFullNb;
    
    public float timerLevelNight;
    //TODO Create an index in the level manager that will check the touch the player will need to press
    public List<KeyCode> keyCodesList = new List<KeyCode>();

    public AudioClip audioLevelNight;
    //TODO Create an index in the level manager that play the clip depending on the index
    public List<AudioClip> sfxNightList = new List<AudioClip>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
