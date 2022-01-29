using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MashingGame : MonoBehaviour
{
    [Header("Mashing")]
    public List<KeyCode> keyCodeList = new List<KeyCode>();
    public KeyCode keyToMash;
    public float timerBeforeKeyChange;
    private float _chrono;

    [Header("Jauge")]
    public Slider jauge;
    public float valueToAdd;
    public float valueToWithdraw;
    public float ratioOverTime;
    
    void Start()
    {
        if (keyCodeList.Count > 0)
        {
            int randomInt = Random.Range(0, keyCodeList.Count);
            keyToMash = keyCodeList[randomInt];
            keyCodeList.RemoveAt(randomInt);
        }
    }

    
    void Update()
    {
        jauge.value -= ratioOverTime * Time.deltaTime;

        _chrono += Time.deltaTime;
        if (_chrono >= timerBeforeKeyChange)
        {
            keyToMash = NewKey();
            _chrono = 0f;
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
                Debug.Log("Wrong Touch");
                jauge.value -= valueToWithdraw;
            }
        }
        
        
    }
}
