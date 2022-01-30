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
    public LevelScriptable actualLevel;
    public static LevelManager Instance;

    public MashingGame mashingGame;
    
    [Header("Audio")] 
    public AudioSource bgmSource;
    public List<AudioSource> sfxSourceList = new List<AudioSource>();
    public GameObject audioSourcePrefab;
    
    //Ater Night
    public AudioClip victoryClip;
    public AudioClip loseClip;
    
    
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
    public List<KeyToMash> keyCodesList = new List<KeyToMash>();
    
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
        
        indexLevel = 1;
        stateDay = StateDay.Day;
        SetupValuesFromLevelScriptable(GetLevelFromIndex(indexLevel));
        fadePanel.DOFade(0, 0.5f).OnComplete(() =>
        {
            PlayScene();
            fadePanel.gameObject.SetActive(false);
        });
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
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
                            mashingGame.HideSlider(true);
                            onAnimationEndNight = false;
                        }
                    }
                    else
                    {
                        timerLevel -= Time.deltaTime;
                        if (timerLevel <= 0)
                        {
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
        actualLevel = levelScriptable;

        switch (stateDay)
        {
            case StateDay.Day :
                timerLevel = actualLevel.timerLevelDay;
                break;
            case StateDay.Night :
                timerLevel = actualLevel.timerLevelDay;
                break;
        }

        //Button Masher
        //TODO Utiliser le truc de kilian 
        //barFullNb = actualLevel.barFullNb;
        keyCodesList = actualLevel.keyCodesList;
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
            DestroySFX();
            ChangeStateDay();
            SetupValuesFromLevelScriptable(GetLevelFromIndex(indexLevel));
            PlayScene();
            
        }));
        sequence.Append(fadePanel.DOFade(0, 1f).SetEase(Ease.InQuint)).OnComplete(()=>
        {
            fadePanel.gameObject.SetActive(false);
            if (stateDay == StateDay.Night)
            {
                CreateMashingAnimatedSprite();
                Debug.Log("MashingGame Start");
                mashingGame.StartNight();
            }
        });
    }



    private void SetupTimer()
    {
        //LevelScriptable level = GetLevelFromIndex(indexLevel);
        if (stateDay == StateDay.Day)
            timerLevel = actualLevel.timerLevelDay;
        else
            timerLevel = actualLevel.timerLevelNight;
    }

    //SFX/BGM
    
    //TODO setup la musique et les sfx
    public void SetupSFX()
    {
        //Audio
        if (stateDay == StateDay.Day)
        {
            foreach (var audio in actualLevel.sfxSceneList)
            {
                AudioSource audioSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
                audioSource.clip = audio.audioClip;
                audioSource.PlayDelayed(audio.timerToPlay);
                sfxSourceList.Add(audioSource);
            }

            if (actualLevel.audioLevelDay != null)
            {
                Debug.Log("Put clip");
                bgmSource.clip = actualLevel.audioLevelDay;
                bgmSource.Play();
            }
        }
        else
        {
            if (actualLevel.audioLevelNight != null)
            {
                bgmSource.clip = actualLevel.audioLevelNight;
                bgmSource.Play();
            }
        }
    }
    
    private void StopSFX()
    {
        foreach (var audioSource in sfxSourceList)
        {
            audioSource.Stop();
        }
        bgmSource.Stop();
    }


    private void CreateBackground()
    {
        //Sprites
        foreach (var element in GetSceneSpritesList())
        {
            GameObject sprite = Instantiate(prefabSceneSprite);
            SpriteRenderer spriteR = sprite.GetComponent<SpriteRenderer>();
            sprite.transform.position = element.posSprite;
            spriteR.sprite = element.sprite;
            spriteR.sortingLayerID = SortingLayer.NameToID(element.sortingLayer.ToString());
            spriteR.sortingOrder = element.sortingLayerValue;

            spriteSceneList.Add(sprite);
        }

    }

    //Create Sprites that will play animations
    private void CreateAnimatedSprites()
    {
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
            spriteAnim.animatorController = element.animationController;

            sprite.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID(element.sprite.sortingLayer.ToString());
            sprite.GetComponent<SpriteRenderer>().sortingOrder = element.sprite.sortingLayerValue;
            //Add the GO to the list 
            spriteSceneAnimationList.Add(sprite);
        }
    }

    private void CreateMashingAnimatedSprite()
    {
        GameObject sprite = Instantiate(prefabSceneSprite);
        sprite.GetComponent<SpriteRenderer>().sprite = actualLevel.mashingFrames[0].sprite;

        sprite.transform.position = actualLevel.mashingFrames[0].pos;
        sprite.transform.rotation = actualLevel.mashingFrames[0].rot;
        
        spriteSceneList.Add(sprite);
        actualLevel.mashingAnimatedObj = sprite;
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


    private void DestroySFX()
    {
        foreach (var audioGO in sfxSourceList)
        {
            Destroy(audioGO);
        }
        sfxSourceList.Clear();
        bgmSource.clip = null;
    }
    
    public void PlayVictoryAnim()
    {
        timerEndNight = actualLevel.timerVictoryAnim;
        onAnimationEndNight = true;
        
        DestroySFX();
        
        //Change BGM 
        bgmSource.Stop();
        bgmSource.clip = victoryClip;
        bgmSource.Play();
        
        foreach (var spriteGO in spriteSceneAnimationList)
        {
            spriteGO.GetComponent<AnimatedSprite>().PlayVictoryClip();
        }
    }

    public void PlayLoseAnim()
    {
        timerEndNight = actualLevel.timerLoseAnim;
        onAnimationEndNight = true;
        
        DestroySFX();
        
        //Change BGM 
        bgmSource.Stop();
        bgmSource.clip = loseClip;
        bgmSource.Play();
        
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
            if (indexLevel < listLevel.Count)
                indexLevel++;
            else
                Debug.Log("EndGame");//EndGame //TODO Change scene 
            stateDay = StateDay.Day;
        }
    }

    private LevelScriptable GetLevelFromIndex(int index)
    {
        return listLevel[index - 1];
    }
}
