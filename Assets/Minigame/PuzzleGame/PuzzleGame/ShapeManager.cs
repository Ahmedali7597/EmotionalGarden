using UnityEngine;
using System.Collections.Generic;

public class ShapeManager : MonoBehaviour
{
    public Socket[] sockets;
    public GameObject[] shapePrefabs;
    public int shapesPerSocket = 3;

    private Color[] colors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan, Color.gray, Color.white, Color.black };
    private string[] colorNames = new string[] { "Red", "Blue", "Green", "Yellow", "Magenta", "Cyan", "Gray", "White", "Black" };

    void Start()
    {
        if (sockets == null || sockets.Length != 3) return;

        List<int> allColorIndices = new List<int>();
        for (int i = 0; i < colorNames.Length; i++) { allColorIndices.Add(i); }

        int[] correctColorIndicesForSockets = new int[3];
        List<int> tempColorPool = new List<int>(allColorIndices);

        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] == null) continue;

            int pickedIdx = GetUniqueRandomIndex(tempColorPool);
            correctColorIndicesForSockets[i] = pickedIdx;

            // Get the 'real shape' written on the socket.
            string shapeName = sockets[i].myShapeName; 
            
            // Set the correct color for the socket.
            sockets[i].SetSolution(colorNames[pickedIdx], colors[pickedIdx]);
        }
        
        List<int> fakeColorPool = new List<int>(allColorIndices);
        List<int> correctIndicesList = new List<int>(correctColorIndicesForSockets);
        correctIndicesList.Sort(); 
        for (int i = correctIndicesList.Count - 1; i >= 0; i--)
        {
            fakeColorPool.Remove(correctIndicesList[i]);
        }

        for (int i = 0; i < sockets.Length; i++)
        {
            // Create shapes based on the shape assigned to the socket.
            string currentShapeName = sockets[i].myShapeName;

            // 1. Always create exactly 1 'correct shape' that perfectly matches the socket.
            int correctColIdx = correctColorIndicesForSockets[i];
            CreateShape(currentShapeName, colorNames[correctColIdx], colors[correctColIdx]);

            // 2. Create the remaining fake shapes.
            for (int j = 0; j < shapesPerSocket - 1; j++)
            {
                if (fakeColorPool.Count == 0) break;
                int randomFakePos = Random.Range(0, fakeColorPool.Count);
                int fakeColorIdx = fakeColorPool[randomFakePos];

                CreateShape(currentShapeName, colorNames[fakeColorIdx], colors[fakeColorIdx]);
            }
        }
    }

    int GetUniqueRandomIndex(List<int> pool)
    {
        if (pool.Count == 0) return 0;
        int randomPos = Random.Range(0, pool.Count);
        int pickedIndex = pool[randomPos];
        pool.RemoveAt(randomPos); 
        return pickedIndex;
    }

    void CreateShape(string shape, string colorName, Color colorValue)
    {
        GameObject prefab = GetPrefabByShape(shape);
        if (prefab == null) return; 

        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        float padding = 1.0f; 

        Vector2 pos = new Vector2(
            Random.Range(-cameraWidth + padding, cameraWidth - padding), 
            Random.Range(-cameraHeight * 0.5f, cameraHeight - padding * 2)
        );

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        obj.name = $"{colorName}_{shape}"; 

        DragObject drag = obj.GetComponent<DragObject>();
        if (drag != null)
        {
            drag.SetShapeAndColor(shape, colorName, colorValue);
        }
    }

    GameObject GetPrefabByShape(string shape)
    {
        if (shape == "Rune1") return shapePrefabs[0]; // First shape prefab
        if (shape == "Rune2") return shapePrefabs[1]; // Second shape prefab
        if (shape == "Rune3") return shapePrefabs[2]; // Third shape prefab
        
        // Warning message in case of a typo in the name
        Debug.LogError($"Error: Shape name is invalid: '{shape}'. Please make sure it is Rune1, Rune2, or Rune3.");
        return shapePrefabs[0];
    }
}