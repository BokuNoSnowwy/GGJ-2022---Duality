using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    OutGame,
    InGame
}

public class MashingGame : MonoBehaviour
{
    [Header("Game Rules")]
    [Tooltip("Le temps que dure la nuit")] public float gameTimer;
    private bool _hasWon;
    [Tooltip("État du jeu")] private GameState actualState;
    [Tooltip("Décompte avant le commencement du jeu")] public float startGameTimer;
    [Tooltip("Texte du décompte (temporaire)")] public TMP_Text startGameTxt;
    
    [Header("Mashing")]
    [Tooltip("Liste des touches qui peuvent être à masher")] public List<KeyCode> keyCodeList = new List<KeyCode>();
    [Tooltip("La touche à masher")] public KeyCode keyToMash;
    [Tooltip("Le temps avant chaque changement de lettre")] public float timerBeforeKeyChange;
    private float _chrono;

    [Header("Jauge")]
    [Tooltip("La jauge")] public Slider jauge;
    [Tooltip("La valeur à ajouter si on appuie sur la bonne touche")] public float valueToAdd;
    [Tooltip("La valeur à retirer si on appuie sur la mauvaise touche")] public float valueToWithdraw;
    [Tooltip("A quel point la jauge descend si on ne fait rien")] public float ratioOverTime;
    
    void Start()
    {
        actualState = GameState.OutGame;
        
        if (keyCodeList.Count > 0)
        {
            int randomInt = Random.Range(0, keyCodeList.Count);
            keyToMash = keyCodeList[randomInt];
            keyCodeList.RemoveAt(randomInt);
        }
    }

    public void StartNight()
    {
        actualState = GameState.OutGame;
        _hasWon = false;

        _chrono = 0f;
        jauge.value = 0f;
        startGameTimer = 3f;
        startGameTxt.text = null;
        //TODO Récup valeurs du LevelManager
        //timerBeforeKeyChange = Le temps avant chaque changement de lettre
        //jauge.maxValue = la valeur à atteindre
        //ratioOverTime = à quel point la jauge descend si on ne fait rien
        //valueToAdd = la valeur à ajouter si on appuie sur la bonne touche
        //valueToWithdraw = la valeur à retirer si on appuie sur la mauvaise touche
        
        int randomInt = Random.Range(0, keyCodeList.Count);
        keyToMash = keyCodeList[randomInt];
        keyCodeList.RemoveAt(randomInt);
        
        
    }

    void LaunchGame()
    {
        startGameTxt.text = null;
        startGameTimer = 3f;
        actualState = GameState.InGame;
    }

    
    void Update()
    {
        if (actualState == GameState.InGame)
        {
            jauge.value -= ratioOverTime * Time.deltaTime;

            _chrono += Time.deltaTime;
            if (_chrono >= timerBeforeKeyChange)
            {
                keyToMash = NewKey();
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

    KeyCode NewKey()
    {
        int randomInt = Random.Range(0, keyCodeList.Count);
        KeyCode k = keyCodeList[randomInt];
        keyCodeList.RemoveAt(randomInt);
        keyCodeList.Add(keyToMash);

        return k;
    }

    void OnGUI()
    {
        Event e = Event.current;
    
        if (e.type == EventType.KeyUp)
        {
            if (e.keyCode == keyToMash)
            {
                Debug.Log("Nice touch");
                jauge.value += valueToAdd;
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

    public void Victory()
    {
        Debug.Log("Victory");
        _hasWon = true;
        actualState = GameState.OutGame;
    }

    public void Lose()
    {
        Debug.Log("Lose");
        actualState = GameState.OutGame;
    }
}
