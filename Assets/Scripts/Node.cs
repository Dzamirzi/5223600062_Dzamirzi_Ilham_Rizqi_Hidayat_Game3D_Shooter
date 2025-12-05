using UnityEngine;

[System.Serializable]
public class Node
{
    public bool walkable;       // Apakah bisa dilewati?
    public Vector3 worldPosition; // Posisi asli di dunia 3D
    public int gridX;           // Koordinat X di grid
    public int gridY;           // Koordinat Y di grid
    public int movementPenalty; // Bobot (Air = Mahal)

    // Variabel untuk Pathfinding nanti
    public int gCost;
    public int hCost;
    public Node parent;

    // Menghitung fCost (Total biaya)
    public int fCost
    {
        get { return gCost + hCost; }
    }

    // Constructor
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
    }
}