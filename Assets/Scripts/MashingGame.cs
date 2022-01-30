using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum GameState
{
    OutGame,
    InGame,
    PauseGame,
}

public class MashingGame : MonoBehaviour
{
    [Header("Game Rules")]
    [Tooltip("Le temps que dure la nuit")] public float gameTimer;
    private bool _hasWon;
    [Tooltip("État du jeu")] public GameState actualState;
    [Tooltip("Décompte avant le commencement du jeu")] public float startGameTimer;
    [Tooltip("Texte du décompte (temporaire)")] public TMP_Text startGameTxt;
    
    [Header("Mashing")]
    [Tooltip("Liste des touches qui peuvent être à masher")] public List<KeyToMash> keyCodeList;
    [Tooltip("La touche à masher")] public KeyToMash keyToMash;
    [Tooltip("Le temps avant chaque changement de lettre")] public float timerBeforeKeyChange;
    private float _chrono;
    private int _indexFrame;
    public GameObject spriteKeyToMash;

    [Header("Jauge")]
    [Tooltip("La jauge")] public Slider jauge;
    [Tooltip("La valeur à ajouter si on appuie sur la bonne touche")] public float valueToAdd;
    [Tooltip("La valeur à retirer si on appuie sur la mauvaise touche")] public float valueToWithdraw;
    [Tooltip("A quel point la jauge descend si on ne fait rien")] public float ratioOverTime;

    private LevelManager _levelManager;
    private List<AnimFrame> nightFrames;
    private GameObject objMashingAnimated;
    
    
    void Start()
    {
        /*
        if (keyCodeList.Count > 0)
        {
            int randomInt = Random.Range(0, keyCodeList.Count);
            keyToMash = keyCodeList[randomInt];
            keyCodeList.RemoveAt(randomInt);
        }
        */
        _levelManager = LevelManager.Instance;
        actualState = GameState.PauseGame;
    }

    public void StartNight()
    {
        actualState = GameState.OutGame;
        _hasWon = false;
        _chrono = 0f;
        HideSlider(false);
        jauge.value = 0f;
        startGameTimer = 3f;
        startGameTxt.text = null;
        gameTimer = _levelManager.actualLevel.timerLevelNight - startGameTimer;
        //TODO Récup valeurs du LevelManager
        timerBeforeKeyChange = _levelManager.actualLevel.timerBeforeKeyChange;
        jauge.maxValue = _levelManager.actualLevel.barFullNb;
        ratioOverTime = _levelManager.actualLevel.ratioOverTime;
        valueToAdd = 1;
        valueToWithdraw = _levelManager.actualLevel.valueToWithdraw;
        _indexFrame = 0;
        nightFrames = new List<AnimFrame>(_levelManager.actualLevel.mashingFrames);
        objMashingAnimated = _levelManager.actualLevel.mashingAnimatedObj;
        
        
        keyCodeList = new List<KeyToMash>(_levelManager.actualLevel.keyCodesList);
        int i = Random.Range(0, keyCodeList.Count - 1);
        keyToMash = keyCodeList[i];
        keyCodeList.RemoveAt(i);
    }

    public void LaunchGame()
    {
        startGameTxt.text = null;
        startGameTimer = 3f;
        actualState = GameState.InGame;

        spriteKeyToMash = Instantiate(_levelManager.prefabSceneSprite);
        PrintSpriteOnScreen();
    }

    public void PrintSpriteOnScreen()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(
            Random.Range(0 + keyToMash.spriteKeyNotMash.rect.width / 2, Screen.width - keyToMash.spriteKeyNotMash.rect.width / 2),
            Random.Range(0 + keyToMash.spriteKeyNotMash.rect.height / 2, Screen.height - keyToMash.spriteKeyNotMash.rect.height / 2)));
        pos.z = 0;
        
        spriteKeyToMash.transform.position = pos;
        spriteKeyToMash.GetComponent<SpriteRenderer>().sprite = keyToMash.spriteKeyNotMash;
        spriteKeyToMash.GetComponent<SpriteRenderer>().sortingOrder = 10;
        spriteKeyToMash.GetComponent<SpriteRenderer>().sortingLayerName = "Front";
    }

    
    void Update()
    {
        if (actualState == GameState.InGame)
        {
            jauge.value -= ratioOverTime * Time.deltaTime;

            _chrono += Time.deltaTime;
            if (_chrono >= timerBeforeKeyChange)
            {
                NewKey();
                PrintSpriteOnScreen();
                _chrono = 0f;
            }

            if (!_hasWon)
            {
                gameTimer -= Time.deltaTime;
            }

            if (jauge.value >= jauge.maxValue - valueToAdd)
            {
                Victory();
            }

            if (gameTimer <= 0f)
            {
                Lose();
            }
        }
        else if(actualState == GameState.OutGame && !_hasWon)
        {
            startGameTimer -= Time.deltaTime;
            startGameTxt.text = Mathf.RoundToInt(startGameTimer).ToString();
            
            if(startGameTimer <= 0f)
                LaunchGame();
        }
    }

    public void NewKey()
    {
        int randomInt = Random.Range(0, keyCodeList.Count - 1);
        KeyToMash k = keyCodeList[randomInt];
        keyCodeList.RemoveAt(randomInt);
        keyCodeList.Add(keyToMash);
        
        keyToMash = k;
    }

    void OnGUI()
    {
        Event e = Event.current;

        if (actualState == GameState.InGame)
        {
            if (e.type == EventType.KeyDown && e.keyCode == keyToMash.keyCode)
                spriteKeyToMash.GetComponent<SpriteRenderer>().sprite = keyToMash.spriteKeyMashed;
            if (e.type == EventType.KeyUp)
            {
                if (e.keyCode == keyToMash.keyCode)
                {
                    Debug.Log("Nice touch");
                    jauge.value += valueToAdd;

                    _indexFrame++;
                    if (_indexFrame >= nightFrames.Count)
                        _indexFrame = 0;

                    objMashingAnimated.GetComponent<SpriteRenderer>().sprite =
                        _levelManager.actualLevel.mashingFrames[_indexFrame].sprite;
                    objMashingAnimated.GetComponent<SpriteRenderer>().sortingOrder =
                        _levelManager.actualLevel.mashingFrames[_indexFrame].sortingLayerValue;
                    objMashingAnimated.GetComponent<SpriteRenderer>().sortingLayerName = _levelManager.actualLevel
                        .mashingFrames[_indexFrame].sortingLayer.ToString();
                    objMashingAnimated.transform.position = _levelManager.actualLevel.mashingFrames[_indexFrame].pos;
                    objMashingAnimated.transform.rotation = _levelManager.actualLevel.mashingFrames[_indexFrame].rot;
                    spriteKeyToMash.GetComponent<SpriteRenderer>().sprite = keyToMash.spriteKeyNotMash;
                }
                else
                {
                    if(e.keyCode != KeyCode.Escape)
                    {
                        Debug.Log("Wrong Touch");
                        jauge.value -= valueToWithdraw;
                    }
                }
            }
        }
    }

    public void HideSlider(bool value)
    {
        if (!jauge.gameObject.activeSelf)
        {
            jauge.gameObject.SetActive(true);
        }
        if (value)
        {
            foreach (var image in jauge.GetComponentsInChildren<Image>())
            {
                image.DOFade(0, 0.5f);
            }
        }
        else
        {
            jauge.gameObject.SetActive(!value);
            foreach (var image in jauge.GetComponentsInChildren<Image>())
            {
                image.DOFade(1, 0.5f);
            }
        }
    }

    public void Victory()
    {
        Debug.Log("Victory");
        _hasWon = true;
        actualState = GameState.PauseGame;
        _levelManager.PlayVictoryAnim();
        
        Destroy(spriteKeyToMash);
    }

    public void Lose()
    {
        Debug.Log("Lose");
        actualState = GameState.PauseGame;
        _levelManager.PlayLoseAnim();
        
        Destroy(spriteKeyToMash);
    }
}

[Serializable]
public class KeyToMash
{
    public KeyCode keyCode;
    public Sprite spriteKeyNotMash;
    public Sprite spriteKeyMashed;
}