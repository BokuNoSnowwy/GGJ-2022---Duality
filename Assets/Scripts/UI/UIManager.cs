using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI startText;
    public TextMeshProUGUI quitText;
    public TextMeshProUGUI creditText;

    public GameObject creditPanel;

    
    public void Play()
    {
        SceneManager.LoadScene("ValentinScene");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void DisplayCredit(bool value)
    {
        creditPanel.SetActive(value);
    }

    public void DisplayStartText(bool value)
    {
        startText.gameObject.SetActive(value);
    }
    
    public void DisplayQuitText(bool value)
    {
        quitText.gameObject.SetActive(value);
    }
    
    public void DisplayCreditText(bool value)
    {
        creditText.gameObject.SetActive(value);
    }
}
