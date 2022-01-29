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

    [Header("Audio")] 
    public AudioSource bgmSource;
    public List<AudioSource> sfxSourceList = new List<AudioSource>();
    public GameObject audioSourcePrefab;
    
    
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
        barFullNb = actualLevel.barFullNb;
        keyCodesList = actualLevel.keyCodesList;
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

            foreach (var audioGO in sfxSourceList)
            {
                Destroy(audioGO);
            }
            sfxSourceList.Clear();
            bgmSource.clip = null;
                
            ChangeStateDay();
            SetupValuesFromLevelScriptable(GetLevelFromIndex(indexLevel));
            PlayScene();
        }));
        sequence.Append(fadePanel.DOFade(0, 1f).SetEase(Ease.InQuint)).OnComplete(()=>fadePanel.gameObject.SetActive(false));
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
            spriteAnim.animatorController = element.animationController;

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
