using UnityEngine;

public class Plant : MonoBehaviour
{
    public string plantType; //the type of plant it is 
    public int currentGrowth = 0; //current stage of the plants growth 
    public int maxGrowth = 3; //max stage possible for the plant 

    public SpriteRenderer plantSprite; //sprite for the plant 
    public Sprite[] growthSprites; //array of sprites for the different growth levels 

    /// <summary>
    /// Increase the growth of the plant if it is less than the max possible 
    /// </summary>
    public void Grow()
    {
        if (currentGrowth < maxGrowth)
        {
            currentGrowth++;
            UpdateAppearance();
        }
    }
    /// <summary>
    /// display the new sprite of the plant 
    /// </summary>
    void UpdateAppearance()
    {
        if (plantSprite != null && growthSprites.Length > currentGrowth)
        {
            plantSprite.sprite = growthSprites[currentGrowth];
        }
    }

}
