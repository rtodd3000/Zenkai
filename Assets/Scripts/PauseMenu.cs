// by GarrettDeveloper on YT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;
    public GameObject PauseMenuCanvas;

    // Drag your Cinemachine Virtual Camera GameObject here
    public GameObject followCameraGO;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Cursor.visible = false;
                Play();
            }
            else
            {
                Cursor.visible = true;
                Stop();
            }
        }
    }

    void Stop()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
        Cursor.visible = true;
        // DISABLE the virtual camera
        if (followCameraGO != null)
            followCameraGO.SetActive(false);
    }

    public void Play()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
        Cursor.visible = false;
        // RE-ENABLE the virtual camera
        if (followCameraGO != null)
            followCameraGO.SetActive(true);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}