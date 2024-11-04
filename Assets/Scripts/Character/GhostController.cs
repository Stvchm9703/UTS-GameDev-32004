using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Object = System.Object;
using Random = UnityEngine.Random;

public class SearchWaypoint
{
    public Waypoint Waypoint;
    public SearchWaypoint Parent;
    public int X => Waypoint.x;
    public int Y => Waypoint.y;
    public int G; // Cost from start to this node
    public int H; // Heuristic cost to goal
    public int F => G + H; // Total cost

    public bool IsWalkable
    {
        get { return Waypoint.gridType == 5 || Waypoint.gridType == 6 || Waypoint.gridType == 0; }
    }

    public SearchWaypoint(Waypoint waypoint, int cost = 1, int heuristic = 0)
    {
        this.Waypoint = waypoint;
        this.Parent = null;
        this.G = cost;
        this.H = heuristic;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is SearchWaypoint waypoint &&
               Waypoint == waypoint.Waypoint &&
               Parent == waypoint.Parent;
    }

    public static bool operator ==(SearchWaypoint a, SearchWaypoint b)
    {
        if (Object.ReferenceEquals(a, null) && Object.ReferenceEquals(b, null))
        {
            return true;
        }

        if (Object.ReferenceEquals(a, null) || Object.ReferenceEquals(b, null))
        {
            return false;
        }

        return a.Equals(b);
        // return a.Equals(b);
    }

    public static bool operator !=(SearchWaypoint a, SearchWaypoint b)
    {
        if (Object.ReferenceEquals(a, null) && Object.ReferenceEquals(b, null))
        {
            return false;
        }

        if (Object.ReferenceEquals(a, null) || Object.ReferenceEquals(b, null))
        {
            return true;
        }

        return !a.Equals(b);
    }
}

public class GhostController : MonoBehaviour
{
    InGameController inGameController;
    LevelGenerator levelGenerator;
    GameObject GhostBlueInst, GhostGreenInst, GhostRedInst, GhostYellowInst;
    EnemyMovement emBlue, emGreen, emRed, emYellow;
    PacStudentMovement pacStudentMovement;

    List<Waypoint> outerRoute = new List<Waypoint>();
    Waypoint topLeft, topRight, bottomLeft, bottomRight;

    Waypoint playerPositionInstant;

    bool isPaused = true;


    public void Init()
    {
        inGameController = GetComponent<InGameController>();
        if (inGameController == null) return;
        // Instantiate the Player
        GhostBlueInst = inGameController.GhostBlueInst;
        GhostGreenInst = inGameController.GhostGreenInst;
        GhostRedInst = inGameController.GhostRedInst;
        GhostYellowInst = inGameController.GhostYellowInst;

        emBlue = inGameController.emBlue;
        emGreen = inGameController.emGreen;
        emRed = inGameController.emRed;
        emYellow = inGameController.emYellow;

        levelGenerator = inGameController.levelGenerator;
        inGameController.EmmitOnReadyStateChange.AddListener(OnReadyStateChange);
        inGameController.EmmitOnScareStateChange.AddListener(OnScareStateChange);
        StartCoroutine(InitialRouteSetting());
    }

    void OnScareStateChange(bool prev, bool current)
    {
        if (current)
        {
            emBlue.ChangeState(EnemyState.Scared);
            emGreen.ChangeState(EnemyState.Scared);
            emRed.ChangeState(EnemyState.Scared);
            emYellow.ChangeState(EnemyState.Scared);
        }
        else
        {
            emBlue.ChangeState(EnemyState.Normal);
            emGreen.ChangeState(EnemyState.Normal);
            emRed.ChangeState(EnemyState.Normal);
            emYellow.ChangeState(EnemyState.Normal);
        }
    }

    void OnReadyStateChange(bool prev, bool current)
    {
        emBlue.SetPause(current);
        emGreen.SetPause(current);
        emRed.SetPause(current);
        emYellow.SetPause(current);
        isPaused = current;
    }

    IEnumerator InitialRouteSetting()
    {
        GenerateGhost4Route();
        yield return new WaitForSeconds(1f);
        pacStudentMovement = inGameController.pacStudentMovement;
        yield return new WaitForSeconds(2f);
        SetupDefaultRoute();
    }

    void SetupDefaultRoute()
    {
        emBlue.SetRoute(GenerateGhost1Route());
        emBlue.emmitOnChangeState.AddListener(OnGhost1ChangeState);
        emBlue.emmitOnAllRouteCompleted.AddListener(() => OnGhostAllRouteCompleted(emBlue, GenerateGhost1Route));
        emGreen.emmitOnBackToRespawn.AddListener(() => OnGhostBackRespawn(emBlue));

        emGreen.SetRoute(GenerateGhost2Route());
        emGreen.emmitOnChangeState.AddListener(OnGhost2ChangeState);
        emGreen.emmitOnAllRouteCompleted.AddListener(() => OnGhostAllRouteCompleted(emGreen, GenerateGhost2Route));
        emGreen.emmitOnBackToRespawn.AddListener(() => OnGhostBackRespawn(emGreen));


        emRed.SetRoute(GenerateGhost3Route());
        emRed.emmitOnChangeState.AddListener(OnGhost3ChangeState);
        emRed.emmitOnAllRouteCompleted.AddListener(() => OnGhostAllRouteCompleted(emRed, GenerateGhost3Route));
        emRed.emmitOnBackToRespawn.AddListener(() => OnGhostBackRespawn(emRed));

        var initPath = FindPath(emYellow.TargetPoint, bottomRight);
        emYellow.SetRoute(initPath);
        emYellow.emmitOnChangeState.AddListener(OnGhost4ChangeState);
        emYellow.emmitOnAllRouteCompleted.AddListener(() => OnGhostAllRouteCompleted(emYellow, () => outerRoute));
        emYellow.emmitOnBackToRespawn.AddListener(() => OnGhostBackRespawn(emYellow));
    }

    private void OnGhost1ChangeState(EnemyState prev, EnemyState current)
    {
        if (current == EnemyState.Normal)
        {
            emBlue.SetRoute(GenerateGhost1Route(), true);
        }
        else if (current == EnemyState.Scared)
        {
            emBlue.SetRoute(GenerateGhost1Route(), true);
        }
        else if (current == EnemyState.Dead)
        {
            emBlue.SetRoute(FindPath(emBlue.TargetPoint, emBlue.RespwanPoint), true);
        }
    }

    private void OnGhost2ChangeState(EnemyState prev, EnemyState current)
    {
        if (current == EnemyState.Normal)
        {
            emGreen.SetRoute(GenerateGhost2Route(), true);
        }
        else if (current == EnemyState.Scared)
        {
            emGreen.SetRoute(GenerateGhost1Route(), true);
        }
        else if (current == EnemyState.Dead)
        {
            emGreen.SetRoute(FindPath(emGreen.TargetPoint, emGreen.RespwanPoint), true);
        }
    }

    private void OnGhost3ChangeState(EnemyState prev, EnemyState current)
    {
        if (current == EnemyState.Normal)
        {
            emRed.SetRoute(GenerateGhost2Route(), true);
        }
        else if (current == EnemyState.Scared)
        {
            emRed.SetRoute(GenerateGhost1Route(), true);
        }
        else if (current == EnemyState.Dead)
        {
            emRed.SetRoute(FindPath(emRed.TargetPoint, emRed.RespwanPoint), true);
        }
    }


    private void OnGhost4ChangeState(EnemyState prev, EnemyState current)
    {
        if (current == EnemyState.Normal)
        {
            var target = new List<Waypoint>
            {
                topLeft,
                topRight,
                bottomRight,
                bottomLeft
            };

            var initPath = FindPath(emYellow.TargetPoint, target[Random.Range(0, target.Count - 1)]);
            ;
            emYellow.SetRoute(initPath, true);
        }
        else if (current == EnemyState.Scared)
        {
            emYellow.SetRoute(GenerateGhost1Route(), true);
        }
        else if (current == EnemyState.Dead)
        {
            emYellow.SetRoute(FindPath(emYellow.TargetPoint, emYellow.RespwanPoint), true);
        }
    }

    private void OnGhostAllRouteCompleted(EnemyMovement em, Func<List<Waypoint>> route = null)
    {
        if (em.state == EnemyState.Normal && route != null)
            em.SetRoute(route());
        else if (em.state == EnemyState.Scared)
            em.SetRoute(GenerateGhost1Route());
        else if (em.state == EnemyState.Dead)
            em.SetRoute(FindPath(em.TargetPoint, em.RespwanPoint));
    }

    void OnGhostBackRespawn(EnemyMovement em)
    {
        em.ChangeState(EnemyState.Normal);
        if (inGameController.InScareState)
        {
            em.ChangeState(EnemyState.Scared);
        }
        else
        {
            em.ChangeState(EnemyState.Normal);
        }
    }


    /// <summary>
    ///    Generate route for ghost 1
    ///     on every time cycle, ghost 1 will move to the snapshot of the player position, move more far from the player
    /// </summary>
    /// <returns></returns>
    List<Waypoint> GenerateGhost1Route()
    {
        playerPositionInstant = pacStudentMovement.targetWaypoint;
        if (playerPositionInstant == null || playerPositionInstant is { x: 0, y: 0 })
            return GenerateGhost3Route();
        
        int randomX = playerPositionInstant.x > levelGenerator.GridCols / 2
            ? Random.Range(1, playerPositionInstant.x)
            : Random.Range(playerPositionInstant.x, levelGenerator.GridCols - 2);
        // int modY = playerPositionInstant.y / 9;
        int randomY = playerPositionInstant.y > levelGenerator.GridRows / 2
            ? Random.Range(1, playerPositionInstant.y)
            : Random.Range(playerPositionInstant.y, levelGenerator.GridRows - 2);

        var randomWp = levelGenerator.Waypoints.Find(wp =>
            wp.x != playerPositionInstant.x && randomX - 3 <= wp.x && wp.x <= randomX &&
            wp.y != playerPositionInstant.y && randomY - 3 <= wp.y && wp.y <= randomY &&
            wp.IsWalkable()
        );

        return FindPath(emBlue.TargetPoint, playerPositionInstant);
    }


    /// <summary>
    ///  Generate route for ghost 2
    /// on every time cycle, ghost 2 will move to the snapshot of the player position, move more near to the player
    /// </summary>
    /// <returns></returns>
    List<Waypoint> GenerateGhost2Route()
    {
        playerPositionInstant = pacStudentMovement.targetWaypoint;
        if (playerPositionInstant == null) return GenerateGhost3Route();
        return FindPath(emGreen.TargetPoint, playerPositionInstant);
    }

    /// <summary>
    /// Generate route for ghost 3
    /// on every time cycle, ghost 3 will move to random waypoint
    /// </summary>
    /// <returns></returns>
    List<Waypoint> GenerateGhost3Route()
    {
        var randomWp =
            levelGenerator.AvailableWaypoints[Random.Range(0, levelGenerator.AvailableWaypoints.Count - 1)];
        return FindPath(emRed.TargetPoint, randomWp);
    }

    List<Waypoint> GenerateGhost4Route()
    {
        // get 4 corner
        topLeft = levelGenerator.Waypoints.Find(wp => wp.x == 1 && wp.y == 1);
        topRight = levelGenerator.Waypoints.Find(wp => wp.y == 1 && wp.x == levelGenerator.GridCols - 2);
        bottomLeft = levelGenerator.Waypoints.Find(wp => wp.x == 1 && wp.y == levelGenerator.GridRows - 2);
        bottomRight = levelGenerator.Waypoints.Find(wp =>
            wp.x == levelGenerator.GridCols - 2 && wp.y == levelGenerator.GridRows - 2);

        var routeBottom = FindPath(bottomRight, bottomLeft);
        var routeLeft = FindPath(bottomLeft, topLeft);
        var routeTop = FindPath(topLeft, topRight);
        var routeRight = FindPath(topRight, bottomRight);

        outerRoute.AddRange(routeBottom);
        outerRoute.AddRange(routeLeft);
        outerRoute.AddRange(routeTop);
        outerRoute.AddRange(routeRight);

        return outerRoute;
    }


    public List<Waypoint> FindPath(Waypoint Start, Waypoint End)
    {
        SearchWaypoint start = new SearchWaypoint(Start);
        SearchWaypoint end = new SearchWaypoint(End);
        SearchWaypoint current = null;

        // Stack<SearchWaypoint> Path = new Stack<SearchWaypoint>();
        List<SearchWaypoint> openList = new List<SearchWaypoint>();
        List<SearchWaypoint> closedList = new List<SearchWaypoint>();
        int g = 0;

        openList.Add(start);

        while (openList.Count > 0)
        {
            // get the square with the lowest F score
            var lowest = openList.Min(l => l.F);
            current = openList.First(l => l.F == lowest);

            // add the current square to the closed list
            closedList.Add(current);

            // remove it from the open list
            openList.Remove(current);

            // if we added the destination to the closed list, we've found a path
            if (closedList.FirstOrDefault(l => l.X == end.X && l.Y == end.Y) != null)
                break;

            var adjacentSquares = GetNeighbors(current);
            g++;


            foreach (var adjacentSquare in adjacentSquares)
            {
                // if this adjacent square is already in the closed list, ignore it
                if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                                                   && l.Y == adjacentSquare.Y) != null)
                    continue;

                // if it's not in the open list...
                if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                                                 && l.Y == adjacentSquare.Y) == null)
                {
                    // compute its score, set the parent
                    adjacentSquare.G = g;
                    adjacentSquare.H = GetHeuristic(adjacentSquare, end);
                    adjacentSquare.Parent = current;

                    // and add it to the open list
                    openList.Insert(0, adjacentSquare);
                }
                else
                {
                    // test if using the current G score makes the adjacent square's F score
                    // lower, if yes update the parent because it means it's a better path
                    if (g + adjacentSquare.H < adjacentSquare.F)
                    {
                        adjacentSquare.G = g;
                        adjacentSquare.Parent = current;
                    }
                }
            }
        }

        List<Waypoint> path = new List<Waypoint>();
        while (current != null)
        {
            path.Add(current.Waypoint);
            current = current.Parent;
        }

        return path;
    }


    private int GetHeuristic(SearchWaypoint a, SearchWaypoint b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }


    private List<SearchWaypoint> GetNeighbors(SearchWaypoint node)
    {
        return levelGenerator.Waypoints
            .FindAll(wp =>
                ((wp.x == node.X + 1 || wp.x == node.X - 1) && (wp.y == node.Y)) ||
                (wp.x == node.X && (wp.y == node.Y + 1 || wp.y == node.Y - 1))
            )
            .FindAll(wp => (wp.gridType == 5 || wp.gridType == 6 || wp.gridType == 0))
            .ConvertAll(wp => new SearchWaypoint(wp));
    }

    [SerializeField] private Sprite debugSprite;

    void debugPath(List<Waypoint> path)
    {
        var goPath = new GameObject("debug-Path");
        goPath.transform.SetParent(this.transform);
        foreach (var wp in path)
        {
            var wpObj = new GameObject("debug-wp");
            wpObj.transform.SetParent(goPath.transform);
            wpObj.transform.localPosition = wp.position;
            wpObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            var sr = wpObj.AddComponent<SpriteRenderer>();
            sr.sprite = debugSprite;
        }
    }
}