using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private bool _isPaused;
    public GameObject pauseMenu;

    public static GameplayManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    
    
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if(!_isPaused)
                Pause();
            else
            {
                Resume();
            }
        }
    }

    public void Pause()
    {
        _isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        
        //TODO Activate Pause Menu
    }

    public void Resume()
    {
        _isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        
        //TODO Deactivate Pause Menu
    }

    public void Quit()
    {
        Application.Quit();
    }
}
