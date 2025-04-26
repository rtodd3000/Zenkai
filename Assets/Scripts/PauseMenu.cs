using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;
    public GameObject PauseMenuCanvas;
    public GameObject followCameraGO;

    void Start()
    {
        Time.timeScale = 1f;
        paused = false;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) Play();
            else         Stop();
        }
    }

    // Fully enter pause state
    private void Stop()
    {
        paused = true;
        Time.timeScale = 0f;
        Cursor.visible = true;

        PauseMenuCanvas.SetActive(true);
        if (followCameraGO != null) followCameraGO.SetActive(false);

        // If inventory is open, hide it (UI-only)
        var inv = FindObjectOfType<InventoryManager>();
        if (inv != null) inv.HideUIOnly();
    }

    // Fully exit pause state
    public void Play()
    {
        paused = false;
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;

        if (followCameraGO != null) followCameraGO.SetActive(true);
    }

    // Just hide the pause UI without changing timeScale or paused flag
    public void HideUIOnly()
    {
        PauseMenuCanvas.SetActive(false);
    }

    // Just show the pause UI without changing timeScale or paused flag
    public void ShowUIOnly()
    {
        PauseMenuCanvas.SetActive(true);
    }

    public void MainMenuButton()
    {
        // Cache the current index for clarity
        int idx = SceneManager.GetActiveScene().buildIndex;

        if (idx == 2)  // ← use '==' to compare, not '='
        {
            // From tutorial (2) back to main menu (0)
            SceneManager.LoadScene(idx - 2);
        }
        else
        {
            // From any other level back one scene
            SceneManager.LoadScene(idx - 1);
        }
    }
}
