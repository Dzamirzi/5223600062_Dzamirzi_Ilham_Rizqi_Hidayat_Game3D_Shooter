using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public bool displayGridGizmos = true; // Nyalakan untuk melihat kotak di Editor
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    public TerrainType[] walkableRegions;
    LayerMask walkableMask;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        Instance = this;
        CreateGrid();
    }

    // Fungsi helper untuk menghitung ukuran grid (dipakai di Editor & Play)
    void UpdateGridParams()
    {
        nodeDiameter = nodeRadius * 2;
        // Mencegah error pembagian 0
        if (nodeDiameter == 0) nodeDiameter = 1;

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        // Setup Layer Bobot
        walkableRegionsDictionary.Clear();
        walkableMask = 0;
        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value;
            // Menggunakan Logaritma untuk mendapatkan ID Layer dari Bitmask
            int layerID = (int)Mathf.Log(region.terrainMask.value, 2);
            if (!walkableRegionsDictionary.ContainsKey(layerID))
            {
                walkableRegionsDictionary.Add(layerID, region.terrainPenalty);
            }
        }
    }

    void CreateGrid()
    {
        UpdateGridParams();

        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = true;
                int movementPenalty = 0;

                // 1. Cek Tabrakan (Obstacle)
                if (Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask))
                {
                    walkable = false;
                }

                // 2. Cek Bobot Tanah (Hanya jika walkable)
                if (walkable)
                {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    // Raycast sejauh 100 unit ke bawah
                    if (Physics.Raycast(ray, out hit, 100, walkableMask))
                    {
                        walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        UpdateGridParams(); // Pastikan parameter update jika dipanggil mendadak

        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    // VISUALISASI DI EDITOR
    void OnDrawGizmos()
    {
        // Gambar kotak batas luar (Wireframe) agar tahu seberapa besar gridnya
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (displayGridGizmos)
        {
            // Jika grid belum dibuat (karena belum Play), paksa buat di Editor
            // AGAR ANDA BISA MELIHATNYA TANPA KLIK PLAY
            if (grid == null)
            {
                CreateGrid();
            }

            if (grid != null)
            {
                foreach (Node n in grid)
                {
                    // Tentukan Warna
                    // Merah = Tembok (Obstacle/Unwalkable)
                    // Putih = Tanah Biasa
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;

                    // Jika Tanah dan punya Penalty (Air/Lava)
                    if (n.walkable && n.movementPenalty > 0)
                    {
                        // Gradasi dari Putih ke Biru Gelap
                        // Semakin tinggi penalty, semakin gelap
                        Gizmos.color = Color.Lerp(Color.cyan, Color.blue, Mathf.InverseLerp(0, 20, n.movementPenalty));

                        // Khusus Lava (Penalty tinggi misal > 40), warnai Orange/Merah
                        if (n.movementPenalty > 40) Gizmos.color = new Color(1, 0.4f, 0);
                    }

                    // Gambar Kotak
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }
}

[System.Serializable]
public class TerrainType
{
    public LayerMask terrainMask;
    public int terrainPenalty;
}