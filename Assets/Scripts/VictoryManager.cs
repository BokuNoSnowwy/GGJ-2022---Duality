using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    
    public GameObject creditPanel;
    
    
    
    public void DisplayCredit(bool value)
    {
        creditPanel.SetActive(value);
    }

    public void Restart()
    {
        SceneManager.LoadScene("ValentinScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
