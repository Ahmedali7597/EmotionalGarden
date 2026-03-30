using UnityEngine;
using System.Collections;

public class RuneGameManager : MonoBehaviour
{
    public int totalRunes = 2;   
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

            // Start delay
            StartCoroutine(ShowEndGameWithDelay(2f));
        }
    }

    IEnumerator ShowEndGameWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndGameUI.Show(true);
    }
}