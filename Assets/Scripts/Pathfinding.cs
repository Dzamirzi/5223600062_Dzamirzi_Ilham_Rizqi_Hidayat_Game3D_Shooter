using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance; // Singleton agar mudah dipanggil musuh
    GridManager grid;

    void Awake()
    {
        Instance = this;
        grid = GetComponent<GridManager>();
    }

    // Fungsi Utama: Mencari Jalan
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Konversi posisi dunia 3D menjadi Node Grid
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();    // Node yang akan dicek
        HashSet<Node> closedSet = new HashSet<Node>(); // Node yang sudah dicek

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // 1. Cari node di openSet dengan biaya terendah (fCost)
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // 2. Jika sudah sampai target, selesai!
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            // 3. Cek semua tetangga
            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                // Jika tembok atau sudah dicek, lewati
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                // Hitung biaya gerak ke tetangga ini
                // (movementPenalty adalah bobot Air/Lava)
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;

                // Jika jalur baru ini lebih pendek, atau tetangga belum ada di openSet
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // Update data tetangga
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode); // Heuristic (Jarak garis lurus ke target)
                    neighbour.parent = currentNode; // Simpan dari mana kita datang

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        // Jika loop selesai tapi target tidak ketemu (misal terkurung tembok)
        return null;
    }

    // Fungsi untuk membalik urutan jalur (dari Target -> Start menjadi Start -> Target)
    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse(); // Balik urutannya
        return path;
    }

    // Menghitung jarak antar 2 node (Diagonal vs Lurus)
    // Lurus = 10, Diagonal = 14 (Akar 2 dari 10^2 + 10^2)
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}