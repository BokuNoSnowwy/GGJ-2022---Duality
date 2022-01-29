using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StateDay
{
    Day,
    Night
}

public class LevelManager : MonoBehaviour
{
    public List<LevelScriptable> listLevel = new List<LevelScriptable>();
    public static LevelManager Instance;

    [Header("UI Gestion")] 
    public Image fadePanel;
    public float timerFadePanel;

    [Header("Level Management")] 
    public StateDay stateDay;
    public int indexLevel;
    //level timer management
    public bool onLevelTimer;
    public float timerLevel;
    public int barFullNb;
    public List<KeyCode> keyCodesList = new List<KeyCode>();
    
    //Audio Source
    
    //Animation End Night
    public bool onAnimationEndNight;
    public float timerEndNight;
    
    public GameObject prefabSceneSprite;
    public GameObject prefabSceneSpriteAnimation;
    public List<GameObject> spriteSceneList = new List<GameObject>();
    public List<GameObject> spriteSceneAnimationList = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        indexLevel = 1;
        stateDay = StateDay.Day;
        SetupValuesFromLevelScriptable(GetLevelFromIndex(indexLevel));
        PlayScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (onAnimationEndNight)
        {
            timerEndNight -= Time.deltaTime;
            if (timerEndNight <= 0)
            {
                NextScene();
                onAnimationEndNight = false;
            }
        }

        /*
        if (onLevelTimer)
        {
            timerLevel -= Time.deltaTime;
            if (timerLevel <= 0)
            {
                onLevelTimer = false;
            }
        }
        */
    }

    private void SetupValuesFromLevelScriptable(LevelScriptable levelScriptable)
    {
        switch (stateDay)
        {
            case StateDay.Day :
                timerLevel = levelScriptable.timerLevelDay;
                break;
            case StateDay.Night :
                timerLevel = levelScriptable.timerLevelDay;
                break;
        }

        //Button Masher
        //TODO Utiliser le truc de kilian 
        barFullNb = levelScriptable.barFullNb;
        keyCodesList = levelScriptable.keyCodesList;
        
        //SFX
        SetupSFX();
    }

    private void PlayScene()
    {
        CreateBackground();
        CreateAnimatedSprites();
    }

    //TODO setup la musique et les sfx
    public void SetupSFX()
    {
        LevelScriptable scriptable = GetLevelFromIndex(indexLevel);
    }

    private void CreateBackground()
    {
        foreach (var element in GetSceneSpritesList())
        {
            GameObject sprite = Instantiate(prefabSceneSprite);
            SpriteRenderer spriteR = sprite.GetComponent<SpriteRenderer>();
            spriteR.sprite = element.sprite;
            spriteR.sortingLayerID = SortingLayer.NameToID(element.sortingLayer.ToString());

            spriteSceneList.Add(sprite);
        }
    }

    //Create Sprites that will play animations
    private void CreateAnimatedSprites()
    {
        foreach (var element in GetSceneAnimationsList())
        {
            GameObject sprite = Instantiate(prefabSceneSpriteAnimation);
            Animation animation = sprite.GetComponent<Animation>();
            AnimatedSprite spriteAnim = sprite.GetComponent<AnimatedSprite>();
            
            Debug.Log(element.animationClip);
            
            //Anim end setup
            spriteAnim.clip = element.animationClip;
            spriteAnim.loseClip = element.loseClip;
            spriteAnim.victoryClip = element.victoryClip;
            spriteAnim.delayAnimation = element.delayAnimation;
            
            /*
            //Get all the animations and put them in the clip
            animation.AddClip(element.animationClip,element.animationClip.name);
            if (element.loseClip != null)
                animation.AddClip(element.loseClip,element.loseClip.name);
            if (element.victoryClip != null)
                animation.AddClip(element.victoryClip, element.victoryClip.name);
                */
            
            sprite.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID(element.sortingLayer.ToString());
            //Add the GO to the list 
            spriteSceneAnimationList.Add(sprite);
        }
    }

    private List<SceneElementsSprite> GetSceneSpritesList()
    {
        List<SceneElementsSprite> listReturn = null;
        switch (stateDay)
        {
            case StateDay.Day :
                return GetLevelFromIndex(indexLevel).spriteListDay;
            case StateDay.Night :
                return GetLevelFromIndex(indexLevel).spriteListNight;
        }
        return listReturn;
    }
    
    private List<SceneElementAnimatedSprite> GetSceneAnimationsList()
    {
        List<SceneElementAnimatedSprite> listReturn = null;
        switch (stateDay)
        {
            case StateDay.Day :
                return GetLevelFromIndex(indexLevel).animationClipListDay;
            case StateDay.Night :
                return GetLevelFromIndex(indexLevel).animationClipListDay;
        }
        return listReturn;
    }


    //Destroy on fade out
    private void NextScene()
    {
        //TODO Make the fade out here
        foreach (var spriteGO in spriteSceneList)
        {
            Destroy(spriteGO);
        }

        foreach (var spriteGo in spriteSceneAnimationList)
        {
            Destroy(spriteGo);
        }
        
        ChangeStateDay();
        SetupValuesFromLevelScriptable(GetLevelFromIndex(indexLevel));
        //TODO Create new background
        
    }


    public void PlayVictoryAnim()
    {
        foreach (var spriteGO in spriteSceneAnimationList)
        {
            spriteGO.GetComponent<AnimatedSprite>().PlayVictoryClip();
        }
    }

    public void PlayLoseAnim()
    {
        foreach (var spriteGO in spriteSceneAnimationList)
        {
            spriteGO.GetComponent<AnimatedSprite>().PlayLoseClip();
        }
    }

    private void ChangeStateDay()
    {
        if (stateDay == StateDay.Day)
        {
            stateDay = StateDay.Night;
        }
        else
        {
            //New day
            indexLevel++;
            stateDay = StateDay.Day;
        }
    }

    private LevelScriptable GetLevelFromIndex(int index)
    {
        return listLevel[index - 1];
    }
}
