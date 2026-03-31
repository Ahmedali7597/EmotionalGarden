using UnityEngine;

public class Plant : MonoBehaviour
{
    public string plantType; //the type of plant it is 
    public int currentGrowth = 0; //current stage of the plants growth 
    public int maxGrowth = 3; //max stage possible for the plant 

    public SpriteRenderer plantSprite; //sprite for the plant 
    public Sprite[] growthSprites; //array of sprites for the different growth levels 

    /// <summary>
    /// On start, ensure the SpriteRenderer exists and show the initial growth sprite
    /// </summary>
    void Start()
    {
        // Auto-wire SpriteRenderer if not assigned
        if (plantSprite == null)
            plantSprite = GetComponent<SpriteRenderer>();

        // Show the initial growth stage sprite
        if (plantSprite != null && growthSprites != null && growthSprites.Length > currentGrowth && growthSprites[currentGrowth] != null)
        {
            plantSprite.sprite = growthSprites[currentGrowth];
        }
    }

    /// <summary>
    /// Increase the growth of the plant if it is less than the max possible 
    /// </summary>
    public void Grow()
    {
        if (currentGrowth < maxGrowth && currentGrowth < growthSprites.Length - 1)
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
        if (plantSprite != null && growthSprites.Length > currentGrowth && growthSprites[currentGrowth] != null)
        {
            plantSprite.sprite = growthSprites[currentGrowth];
        }
    }

}
