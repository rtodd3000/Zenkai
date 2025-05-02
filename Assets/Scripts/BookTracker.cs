using UnityEngine;

public class BookTracker : MonoBehaviour
{
    public static BookTracker Instance;

    [SerializeField] private int totalBooks = 3;
    private int booksCollected = 0;

    [SerializeField] private GameObject objectToReveal;
    [TextArea(2, 5)]
    [SerializeField] private string allBooksCollectedMessage = "You've collected all 3 books! But wait, is that...... a pyramid?";

    [SerializeField] private UIManager uiManager; // Optional: assign manually or auto-find

    [SerializeField] private GameObject crownPrefab;       // Crown asset to attach
    [SerializeField] private Transform headTransform;      // Character's head transform

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        if (objectToReveal != null)
            objectToReveal.SetActive(false);

        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();
    }

    public void BookCollected()
    {
        booksCollected++;

        if (booksCollected >= totalBooks)
        {
            if (objectToReveal != null)
                objectToReveal.SetActive(true);

            if (uiManager != null && !string.IsNullOrWhiteSpace(allBooksCollectedMessage))
                uiManager.ShowMessage(allBooksCollectedMessage);
        }
    }
}
