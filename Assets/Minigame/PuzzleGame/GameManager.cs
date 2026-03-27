using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene switching (Restart/Next Level)

public class GameManager : MonoBehaviour
{
    // Singleton Pattern so DragObject can easily find this.
    public static GameManager instance; 

    // IMPORTANT! Assign your Socket GameObjects here in the Inspector!
    public Socket[] sockets; 
    public GameObject winUI; // Assign your Win UI Panel (e.g., "Well Done!")

    // Initialize the static instance.
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            // In case a duplicate GameManager exists in a new scene.
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        // Hide Win UI at the start
        if (winUI != null) winUI.SetActive(false); 
    }

    // This method is called by DragObject when a shape is dropped.
    public void CheckWin() 
    {
        // Safety check in case sockets are not assigned.
        if (sockets == null || sockets.Length == 0)
        {
            Debug.LogError("Error: No sockets assigned to the GameManager in the Inspector!");
            return; 
        }

        bool allCorrect = true;

        // Loop through all assigned sockets
        foreach (Socket socket in sockets)
        {
            // Extra safety: Check if a slot in the array is null.
            if (socket == null)
            {
                Debug.LogError("Error: Found a null entry in the GameManager Sockets array. Please correct.");
                allCorrect = false; 
                break;
            }

            // Check if THIS specific socket has been solved.
            if (!socket.isCorrect) 
            {
                allCorrect = false;
                break; // If even ONE is incorrect, we stop checking.
            }
        }

        // If all sockets are correct, the player wins!
        if (allCorrect)
        {
            Debug.Log("Puzzle Solved: Well Done!");
            if (winUI != null) winUI.SetActive(true); // Show the Win UI
        }
    }

    // UI Button Function: Restart Current Level
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // UI Button Function: Next Level (by index)
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}