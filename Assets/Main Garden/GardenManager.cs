using UnityEngine;

public class GardenManager : MonoBehaviour
{
    public string currentEmotion = "calm"; //Current emotion selected  
    public Plant[] plants; //array of plants 
    public Camera mainCamera;


    /// <summary>
    /// Debugging function 
    /// </summary>
    void Start()
    {
        Debug.Log("Garden started with emotion: " + currentEmotion);
    }

    /// <summary>
    /// Update the displayed sprite for each plant in the garden 
    /// </summary>
    public void UpdatePlants()
    {
        for (int i = 0; i < plants.Length; i++)
        {
            if (plants[i] != null)
            {
                plants[i].Grow();
            }
        }
    }

    /// <summary>
    /// Function called when changing emotions 
    /// </summary>
    /// <param name="newEmotion"> emotion to change to </param>
    public void ChangeEmotion(string newEmotion)
    {
        currentEmotion = newEmotion;
        Debug.Log("Emotion changed to: " + currentEmotion);
    }

    /// <summary>
    /// Change garden based on selected mood 
    /// </summary>
    public void UpdateEvironment()
    {
        if (currentEmotion == "Calm")
        {
            //Logic goes here 
        }

        else if (currentEmotion == "Energetic")
        {
            //Logic goes here
        }

        else if (currentEmotion == "Anxious")
        {
            //Logic goes here
        }

        else if (currentEmotion == "Sad")
        {
            //Logic goes here 
        }

        else
        {
            //Logic goes here 
        }
    }

}
