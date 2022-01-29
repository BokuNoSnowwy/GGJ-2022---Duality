using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private bool _isPaused;
    public GameObject pauseMenu;
    public GameObject optionMenu;

    public static GameplayManager Instance;

    private AudioManager _audioManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _audioManager = AudioManager.Instance;
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
        optionMenu.SetActive(false);
        Time.timeScale = 1f;
        
        //TODO Deactivate Pause Menu
    }

    public void OpenOption()
    {
        optionMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }
    
    public void CloseOption()
    {
        optionMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
