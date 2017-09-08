using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public int chunkSize = 32;

    public bool displayGridGizmos = true;
    public bool autoUpdate;
    [SerializeField]
    bool gridGenerated;

    int oldChunkSize;
    bool chunkSizeChanged;
    public Node[,] chunk;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateGrid();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DestroyGrid();
        }
    }

    public void GenerateGrid()
    {
        if (gridGenerated == true)
        {
            DestroyGrid();
            Debug.LogWarning("Grid allready generated");
            return;
        }

        chunk = new Node[chunkSize, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Node newNode = new Node(x, y);
                chunk[x, y] = newNode;
            }
        }

        gridGenerated = true;
    }

    private void DestroyGrid()
    {
        if (gridGenerated)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    chunk[x, y] = null;
                }
            }
            chunk = null;
            gridGenerated = false;
        }
    }
    private void OnEnable()
    {
        if (!gridGenerated & autoUpdate)
        {
            GenerateGrid();
        }
    }
    private void OnDisable()
    {
        if (gridGenerated)
            DestroyGrid();
    }

    private void OnDrawGizmos()
    {
        if (!displayGridGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(chunkSize, chunkSize, 0f));

        if (!gridGenerated)
            return;

        Gizmos.color = Color.green;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Node node = chunk[x, y];
                Vector3 nodePos = new Vector3(node.posX, node.posY, 0f);
                Gizmos.DrawWireCube(transform.position, nodePos);
            }
        }
    }

}

[System.Serializable]
public class Node
{
    public int posX;
    public int posY;

    public Node (int newPosX, int newPosY)
    {
        posX = newPosX;
        posY = newPosY;
    }
}
