using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    public bool onScene;
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
        fadePanel.DOFade(0, 1f).OnComplete(() =>
        {
            PlayScene();
            fadePanel.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (onScene)
        {
            switch (stateDay)
            {
                //Day
                case StateDay.Day :
                    timerLevel -= Time.deltaTime;
                    if (timerLevel <= 0)
                    {
                        NextScene();
                    }
                    break;
                //Night
                case StateDay.Night :
                    if (onAnimationEndNight)
                    {
                        timerEndNight -= Time.deltaTime;
                        if (timerEndNight <= 0)
                        {
                            NextScene();
                            onAnimationEndNight = false;
                        }
                    }
                    else
                    {
                        timerLevel -= Time.deltaTime;
                        if (timerLevel <= 0)
                        {
                            onAnimationEndNight = true;
                            //Change animation
                            PlayLoseAnim();
                        }
                    }
                    break;
            }
        }
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
        

    }

    private void PlayScene()
    {
        timerLevel = GetLevelFromIndex(indexLevel).timerLevelDay;
        onScene = true;
        CreateBackground();
        CreateAnimatedSprites();
        //SFX
        SetupSFX();
        //Timer
        SetupTimer();
    }

    private void SetupTimer()
    {
        LevelScriptable level = GetLevelFromIndex(indexLevel);
        if (stateDay == StateDay.Day)
            timerLevel = level.timerLevelDay;
        else
            timerLevel = level.timerLevelNight;
    }

    //TODO setup la musique et les sfx
    public void SetupSFX()
    {
        //LevelScriptable scriptable = GetLevelFromIndex(indexLevel);
    }

    private void CreateBackground()
    {
        foreach (var element in GetSceneSpritesList())
        {
            GameObject sprite = Instantiate(prefabSceneSprite);
            SpriteRenderer spriteR = sprite.GetComponent<SpriteRenderer>();
            sprite.transform.position = element.posSprite;
            spriteR.sprite = element.sprite;
            spriteR.sortingLayerID = SortingLayer.NameToID(element.sortingLayer.ToString());

            spriteSceneList.Add(sprite);
        }
    }

    //Create Sprites that will play animations
    private void CreateAnimatedSprites()
    {
        Debug.Log(stateDay);
        foreach (var element in GetSceneAnimationsList())
        {
            GameObject sprite = Instantiate(prefabSceneSpriteAnimation);
            AnimatedSprite spriteAnim = sprite.GetComponent<AnimatedSprite>();
            
            //Sprite setup
            sprite.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID(element.sprite.sortingLayer.ToString());
            sprite.transform.position = element.sprite.posSprite;
            
            //Get all the animations and put them in the clip
            //Anim end setup
            spriteAnim.clip = element.clip;
            spriteAnim.animatorController = element.animationClip;

            sprite.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID(element.sprite.sortingLayer.ToString());
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
                return GetLevelFromIndex(indexLevel).animationClipListNight;
        }
        return listReturn;
    }


    //Destroy on fade out
    private void NextScene()
    {
        onScene = false;
        Sequence sequence = DOTween.Sequence();
        fadePanel.gameObject.SetActive(true);
        sequence.Append(fadePanel.DOFade(1, 1f).OnComplete(() =>
            {
                foreach (var spriteGO in spriteSceneList)
                {
                    Destroy(spriteGO);
                }
                spriteSceneList.Clear();

                foreach (var spriteGo in spriteSceneAnimationList)
                {
                    Destroy(spriteGo);
                }
                spriteSceneAnimationList.Clear();
                
                ChangeStateDay();
                SetupValuesFromLevelScriptable(GetLevelFromIndex(indexLevel));
                PlayScene();
            }));
        sequence.Append(fadePanel.DOFade(0, 1f).SetEase(Ease.InQuint)).OnComplete(()=>fadePanel.gameObject.SetActive(false));
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
