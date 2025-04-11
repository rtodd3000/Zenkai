using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;  // For legacy UI, use: public Text messageText;
    [SerializeField] private float messageDuration = 2f;  // Duration the message stays on screen

    // Call this method to display a message.
    public void ShowMessage(string message)
    {
        if (messageText == null)
        {
            Debug.LogWarning("UIManager: Message Text is not assigned!");
            return;
        }

        // Update and activate the text.
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        // Stop any previous message coroutine and start a new one.
        StopAllCoroutines();
        StartCoroutine(HideMessageCoroutine());
    }

    // Coroutine to hide the message after a delay.
    private IEnumerator HideMessageCoroutine()
    {
        yield return new WaitForSeconds(messageDuration);
        messageText.gameObject.SetActive(false);
    }
}
