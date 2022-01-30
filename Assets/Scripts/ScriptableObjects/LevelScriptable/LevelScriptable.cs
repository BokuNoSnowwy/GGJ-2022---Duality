using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Animations;
using UnityEngine;

public enum SortingLayerEnum
{
    Front,
    Mid,
    Background
} 

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
    public SortingLayerEnum sortingLayer;
    public int sortingLayerValue = 0;
    public Vector2 posSprite;
}

[Serializable]
public class SceneElementAnimatedSprite
{
    public SceneElementsSprite sprite; 
    public AnimatorController animationController;
    public AnimationClip clip;
    public float delayAnimation;
}

[Serializable]
public class AnimFrame
{
    public Sprite sprite;
    public Vector3 pos;
    public Quaternion rot;

    public SortingLayerEnum sortingLayer;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelScriptable", order = 1)]
public class LevelScriptable : ScriptableObject
{
    [Header("Day Part")] 
    //Characters that will be animated 
    public List<SceneElementAnimatedSprite> animationClipListDay = new List<SceneElementAnimatedSprite>();
    public List<SceneElementsSprite> spriteListDay = new List<SceneElementsSprite>();
    public float timerLevelDay;
    public AudioClip audioLevelDay;
    public List<SFXScene> sfxSceneList = new List<SFXScene>();

    [Header("Night Part")]
    public List<SceneElementAnimatedSprite> animationClipListNight = new List<SceneElementAnimatedSprite>();
    public List<SceneElementsSprite> spriteListNight = new List<SceneElementsSprite>();
    public List<AnimFrame> mashingFrames = new List<AnimFrame>();
    [HideInInspector] public GameObject mashingAnimatedObj;
    
    public int barFullNb;
    public int pointLoseOnMiss;
    
    public float timerLevelNight;

    public AudioClip audioLevelNight;
    //TODO Create an index in the level manager that play the clip depending on the index
    public List<AudioClip> sfxNightList = new List<AudioClip>();

    [Header("Mashing Game")]
    public List<KeyToMash> keyCodesList = new List<KeyToMash>();
    public float timerBeforeKeyChange;
    public float ratioOverTime;
    public int valueToWithdraw;
    
    [Header("Timer End Night Part")]
    public float timerVictoryAnim;
    public float timerLoseAnim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
