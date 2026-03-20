using UnityEngine;

public class ShapeManager : MonoBehaviour
{
    public Socket[] sockets;
    public DragObjectMobile[] shapes;

    void Start()
    {
        // Randomize all sockets
        foreach (Socket s in sockets)
        {
            s.RandomizeSocket();
        }

        // Make sure each socket has at least one matching shape
        for (int i = 0; i < sockets.Length; i++)
        {
            if (i < shapes.Length)
            {
                DragObjectMobile shape = shapes[i];
                shape.shapeType = sockets[i].correctShape;
                shape.colorType = sockets[i].correctColor;
                shape.GetComponent<SpriteRenderer>().color = sockets[i].sr.color;
            }
        }

        // Assign remaining shapes completely random
        for (int i = sockets.Length; i < shapes.Length; i++)
        {
            shapes[i].RandomizeShapeAndColor(); // call public method instead of Start()
        }
    }
}