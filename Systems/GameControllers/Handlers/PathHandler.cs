using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PathHandler : MonoCached
{
    public LayerMask obstacleMask;
    [SerializeField]
    private Vector3 gridOffset;

    public int pathGridLength;
    public int nodeRadius;
    public int pathGridWidth { get; set; }
    public float roadLength { get; set; }
    public Node[,] grid { get; private set; }

    public Transform[] pathsEndPoints;

    private EnemyHandler enemyHandler;

    private Vector3 pathsEnd;

    private List<Node>[] retracedPaths;
    private int freeRetracedPathsIndex = 0;


    public int MaxSize { get { return pathGridLength * pathGridWidth; } }

    public void InjectEnemyHandler(EnemyHandler enemyHandler)
    {
        this.enemyHandler = enemyHandler;
    }

    public void CreateGrid(Vector3 roadPos)
    {
        grid = new Node[pathGridWidth, pathGridLength];
        Vector3 worldBottomLeft = roadPos ;

        for (int x = 0; x < pathGridWidth; x++)
        {
            for (int y = 0; y < pathGridLength; y++)
            {
                Vector3 worldPoint = worldBottomLeft - Vector3.right * ((roadLength / pathGridWidth) / 2 + x * (roadLength / pathGridWidth) + gridOffset.x) - Vector3.forward * ((roadLength / pathGridWidth) / 2 + y * (roadLength / pathGridWidth) + gridOffset.z);

                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask));

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
        pathsEnd = new Vector3();

        retracedPaths = new List<Node>[40];

        for (int i = 0; i < retracedPaths.Length; i++)
        {
            retracedPaths[i] = new List<Node>();
        }

        AddNeighboursToNodes();

        StartCoroutine(UpdateGrid(roadPos));
    }

    private void AddNeighboursToNodes()
    {
        for (int x = 0; x < pathGridWidth; x++)
        {
            for (int y = 0; y < pathGridLength; y++)
            {
                for (int X = -1; X <= 1; X++)
                {
                    for (int Y = -1; Y <= 1; Y++)
                    {
                        if (X == 0 && Y == 0)
                            continue;

                        int checkX = grid[x, y].gridX + X;
                        int checkY = grid[x, y].gridY + Y;

                        if (checkX >= 0 && checkX < pathGridWidth && checkY >= 0 && checkY < pathGridLength)
                        {
                            grid[x, y].neighbours.Add(grid[checkX, checkY]);
                        }
                    }
                }
            }
        }
    }

    public void StartTranslateGrid(Vector3 newRoadPos)
    {
        StartCoroutine(TranslateGrid(newRoadPos));
    }

    private IEnumerator TranslateGrid(Vector3 newRoadPos)
    {
        for (int x = 0; x < pathGridWidth; x++)
        {
            for (int y = 0; y < pathGridLength; y++)
            {
                Vector3 worldPoint = newRoadPos - Vector3.right * ((roadLength / pathGridWidth) / 2 + x * (roadLength / pathGridWidth) + gridOffset.x) - Vector3.forward * ((roadLength / pathGridWidth) / 2 + y * (roadLength / pathGridWidth) + gridOffset.z);
                grid[x, y].worldPosition = worldPoint;
            }
        }

        StartCoroutine(UpdateGrid(newRoadPos));

        yield return null;
    }

    private IEnumerator UpdateGrid(Vector3 roadPos)
    {
        yield return new WaitForFixedUpdate();

        Vector3 worldBottomLeft = roadPos;

        for (int x = 0; x < pathGridWidth; x++)
        {
            for (int y = 0; y < pathGridLength; y++)
            {
                bool walkable = !(Physics.CheckSphere(grid[x,y].worldPosition, nodeRadius, obstacleMask));
                grid[x, y].walkable = walkable;
                grid[x, y].isUsed = false;
            }
        }

        for (int i = 0; i < pathsEndPoints.Length; i++)
        {
            pathsEnd.x = pathsEndPoints[i].position.x;
            pathsEnd.y = roadPos.y;
            pathsEnd.z = roadPos.z + pathGridLength;

            pathsEndPoints[i].position = pathsEnd;
        }

        UpdateEnemiesPaths();

        yield return null;
    }

    private void UpdateEnemiesPaths()
    {
        enemyHandler.StartUpdateEnemiesPaths(this);
    }

    public Node FindClosestNodeTo(Vector3 targetPosition)
    {
        Node closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var node in grid)
        {
            var dist = (node.worldPosition - targetPosition).magnitude;
            if (dist < closestDistance && node.walkable==true)
            {
                closest = node;
                closestDistance = dist;
            }
        }
        if (closest != null)
            return closest;
        return null;
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = FindClosestNodeTo(startPos);
        Node targetNode = FindClosestNodeTo(targetPos);

        Heap<Node> openSet = new Heap<Node>(pathGridWidth * pathGridLength);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();

            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in currentNode.neighbours)
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.weight;
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    //if(currentNode.isUsed==false)
                   // {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                   // }
                }
            }
        }

       
        return null;
    }
   

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        if(freeRetracedPathsIndex >= retracedPaths.Length-1)
        {
            for (int i = 0; i < retracedPaths.Length; i++)
            {
                retracedPaths[i].Clear();
            }
            freeRetracedPathsIndex = 0;
        }
        freeRetracedPathsIndex++;

        //List<Node> retracedPath = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            //currentNode.isUsed = true;
            retracedPaths[freeRetracedPathsIndex].Add(currentNode);
            currentNode = currentNode.parent;
        }
        retracedPaths[freeRetracedPathsIndex].Reverse();

        return retracedPaths[freeRetracedPathsIndex];
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 10 * (dstX - dstY) + 14 * dstY ;
        return 10 * (dstY - dstX) + 14 * dstX ;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(pathGridWidth, 1, pathGridLength));

        if (grid != null && grid.Length>5)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;

                Gizmos.DrawWireSphere(n.worldPosition, /*Vector3.one * (nodeDiameter - .1f)*/ nodeRadius);
            }
        }
    }
}
