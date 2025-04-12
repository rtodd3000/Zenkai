// by GarrettDeveloper on YT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Load Play Scene
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    //Load Tutorial Scene
    public void Tutorial()
    {
        SceneManager.LoadScene(2);
    }

    // Figure out Options

    //Quit Game
    public void Quit()
    {
        Application.Quit();
        Debug.Log("The Player has Quit the game");
    }
}