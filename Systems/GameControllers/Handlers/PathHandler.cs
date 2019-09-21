using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PathHandler : MonoCached
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Vector3 offsetFromRoad;

    public Node[,] grid { get; private set; }
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    public Transform[] pathsEndPoints;

    private EnemyHandler enemyHandler;

    private Vector3 pathsEnd;

    private List<Node>[] retracedPaths;
    private int freeRetracedPathsIndex = 0;


    public int MaxSize { get { return gridSizeX * gridSizeY; } }

    public void InjectEnemyHandler(EnemyHandler enemyHandler)
    {
        this.enemyHandler = enemyHandler;
    }

    public void CreateGrid(Vector3 roadPos)
    {

        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = roadPos /* - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2*/;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool walkable = !(Physics.CheckSphere(worldPoint, nodeDiameter - 0.2f, unwalkableMask));

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
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int X = -1; X <= 1; X++)
                {
                    for (int Y = -1; Y <= 1; Y++)
                    {
                        if (X == 0 && Y == 0)
                            continue;

                        int checkX = grid[x, y].gridX + X;
                        int checkY = grid[x, y].gridY + Y;

                        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
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
        Vector3 worldBottomLeft = newRoadPos - offsetFromRoad;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                grid[x, y].worldPosition = worldPoint;
            }
        }

        StartCoroutine(UpdateGrid(newRoadPos));

        yield return null;
    }

    private IEnumerator UpdateGrid(Vector3 roadPos)
    {
        yield return new WaitForFixedUpdate();

        Vector3 worldBottomLeft = roadPos - offsetFromRoad;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                bool walkable = !(Physics.CheckSphere(grid[x,y].worldPosition, nodeDiameter - 0.2f, unwalkableMask));
                grid[x, y].walkable = walkable;
            }
        }

        for (int i = 0; i < pathsEndPoints.Length; i++)
        {
            pathsEnd.x = pathsEndPoints[i].position.x;
            pathsEnd.y = roadPos.y;
            pathsEnd.z = roadPos.z + gridWorldSize.y;

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

        Heap<Node> openSet = new Heap<Node>(gridSizeX * gridSizeY);
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
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
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
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;

                Gizmos.DrawWireSphere(n.worldPosition, /*Vector3.one * (nodeDiameter - .1f)*/ nodeRadius-.4f);
            }
        }
    }
}
