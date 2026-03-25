using UnityEngine;

public class RuneGameManager : MonoBehaviour
{
    public int totalRunes = 2;   // How many runes must finish
    private int completedRunes = 0;

    public BlackScreenReveal blackScreenReveal;
    public void RuneCompleted()
    {
        completedRunes++;

        if (completedRunes >= totalRunes)
        {
            // All runes finished → remove black screen
            blackScreenReveal.StartReveal();
            Debug.Log("Game Completed!");
        }
    }

}
