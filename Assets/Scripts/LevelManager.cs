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
    public float timerLevel;
    public int barFullNb;

    //A gameobject reference with a 
    public GameObject prefabSceneSprite;
    public GameObject prefabSceneSpriteAnimation;
    public List<GameObject> spriteSceneList = new List<GameObject>();
    
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupValuesFromLevelScriptable(LevelScriptable levelScriptable)
    {
        
    }

    private void CreateBackground()
    {
        foreach (var element in GetSceneSpritesList())
        {
            GameObject sprite = Instantiate(prefabSceneSprite);
            SpriteRenderer spriteR = sprite.GetComponent<SpriteRenderer>();
            spriteR.sprite = element.sprite;
            spriteR.sortingLayerID = element.sortingLayer.id;
        }
    }

    private void CreateAnimatedSprites()
    {
        foreach (var element in GetSceneAnimationsList())
        {
            GameObject sprite = Instantiate(prefabSceneSpriteAnimation);
            sprite.GetComponent<Animation>().clip = element.animationClip;
            sprite.GetComponent<SpriteRenderer>().sortingLayerID = element.sortingLayer.id;
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
    
    private List<SceneElementAnimation> GetSceneAnimationsList()
    {
        List<SceneElementAnimation> listReturn = null;
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
        ChangeStateDay();
    }

    private void ChangeStateDay()
    {
        if (stateDay == StateDay.Day)
        {
            stateDay = StateDay.Night;
        }
        else
        {
            stateDay = StateDay.Day;
        }
    }

    private LevelScriptable GetLevelFromIndex(int index)
    {
        return listLevel[index - 1];
    }
}
