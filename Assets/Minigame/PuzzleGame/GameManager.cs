using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Socket[] sockets;
    public Transform door; // assign door object here

    void Awake()
    {
        instance = this;
    }

    public void CheckWin()
    {
        foreach (Socket s in sockets)
        {
            if (!s.isFilled)
                return;
        }

        OpenDoor();
    }

    void OpenDoor()
    {
        Debug.Log("Door opened!");
    }
}