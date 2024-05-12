using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using InsaneScatterbrain.RandomNumberGeneration;
using InsaneScatterbrain.ScriptGraph;
using Multiplayer;
using Playroom;
using UnityEngine;

public class MapChunksManager : MonoBehaviour
{
    [SerializeField] private ScriptGraphRunner _runner;
    [SerializeField] private Camera _cam;
    [SerializeField] private int _chunkSize = 25;
    [SerializeField] private int _spawnRange = 25;
    //[SerializeField] private Transform _mapObjectsContainer;

    private Queue<Vector2Int> queuedChunks;
    private HashSet<Vector2Int> existingChunkCoords;
    private List<Vector2Int> chunkCoordsInRange;

    private bool graphRunnerActive;
    private Rect chunkSpawningArea;

    private void Start()
    {
        Pools.Clear();    // Make sure the map is clear before we start using it.
        
        queuedChunks = new Queue<Vector2Int>();
        existingChunkCoords = new HashSet<Vector2Int>();
        chunkCoordsInRange = new List<Vector2Int>();

        if (!PlayroomKit.IsRunningInBrowser())
        {
            InitializeMapChunks();
        }
    }

    public void InitializeMapChunks()
    {
        if (!PlayroomKit.IsRunningInBrowser())
        {
            string dateSeed = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            SetupFromServer(GetRandomSeed(dateSeed));
            return;
        }
        if (PlayroomKit.IsHost())
        {
            string dateSeed = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            PlayroomKit.SetState(GameConstants.GameStateData.MapSeed.ToString(), dateSeed, true);
            SetupFromServer(GetRandomSeed(dateSeed));
        }
        else
        {
            PlayroomKit.WaitForState(GameConstants.GameStateData.MapSeed.ToString(), OnSeedReceived);
        }
    }

    private void OnSeedReceived()
    {
        string seed = PlayroomKit.GetState<string>(GameConstants.GameStateData.MapSeed.ToString());
        SetupFromServer(GetRandomSeed(seed));
    }

    private RngState GetRandomSeed(string seed)
    {
        return RngState.FromBytes(Encoding.ASCII.GetBytes(seed));
    }

    private void SetupFromServer(RngState noiseRngState)
    {
        /*
        if (_runner.GraphProcessor.IsSeedRandom)
        {   
            // If the graph's being run with a random seed, a single random seed is generated to pass to
            // the Perlin Noise Fill Texture node.
            // If we don't do this a new random seed is used for each chunk and they won't match-up with each other.
            noiseRngState = RngState.New();
        }
        else
        {
        */
        /*
        // If a static seed is used, we generate a state from that and pass that instead.
        switch (_runner.GraphProcessor.SeedType)
        {
            case SeedType.Guid:
                if (!Guid.TryParse(_runner.GraphProcessor.SeedGuid, out var guid))
                {
                    throw new ArgumentException("Seed is not a valid GUID.");
                }

                noiseRngState = RngState.FromBytes(guid.ToByteArray());
                break;
            case SeedType.Int:
                noiseRngState = RngState.FromInt(_runner.GraphProcessor.Seed);
                break;
            default:
                throw new ArgumentException("Invalid seed type.");
        }
        }*/

        // Set the state for the Perlin Noise Fill Texture nodes.
        _runner.SetIn("NoiseRNGState", noiseRngState);

        _runner.OnProcessed += objects =>
        {
            // If the runner's done processing, it can run for another chunk.
            graphRunnerActive = false;
            RunNextChunk();
        };

        // Run the first chunk.
        RunNextChunk();
    }

    private void RunNextChunk()
    {
        if (queuedChunks.Count < 1) return; // No new chunks to generate.

        if (graphRunnerActive) return;      // The runner's busy with another chunk, so we wait for it to finish.
        
        graphRunnerActive = true;
        
        var chunkCoords = queuedChunks.Dequeue();

        _runner.SetIn("ChunkSize", _chunkSize);
        _runner.SetIn("Coordinates", chunkCoords);
        _runner.Run();
    }

    private void LateUpdate()
    {
        if (_cam == null)
            _cam = Camera.main;
        if (_cam == null) return;
        
        var camPos = _cam.transform.position;
        
        // Calculate the area in which chunks are generated.
        var halfSize = _chunkSize * .5f;
        
        var xMin = camPos.x - _spawnRange;
        var xMax = camPos.x + _spawnRange;
        var yMin = camPos.y - _spawnRange;
        var yMax = camPos.y + _spawnRange;
        chunkSpawningArea = new Rect(
            xMin, yMin, xMax - xMin, yMax - yMin
        );

        // Find the min. chunk coordinates in the current area.
        var minChunkCoords = new Vector2Int(
            Mathf.FloorToInt(RoundToClosestMultiple(xMin - halfSize, _chunkSize)),
            Mathf.FloorToInt(RoundToClosestMultiple(yMin - halfSize, _chunkSize))
        );

        // Find all the chunk coords that are instead of the current area.
        chunkCoordsInRange.Clear();
        for (var x = minChunkCoords.x; x < xMax - halfSize; x += _chunkSize)
        for (var y = minChunkCoords.y; y < yMax - halfSize; y += _chunkSize)
        {
            var chunkCoords = new Vector2Int(x, y);
            chunkCoordsInRange.Add(chunkCoords);

            if (existingChunkCoords.Contains(chunkCoords)) continue;    // A chunk has already been generated for these coords.

            existingChunkCoords.Add(chunkCoords);
            queuedChunks.Enqueue(chunkCoords);
            RunNextChunk();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        // Draw lines that display the area in which new chunks get generated.
        Gizmos.DrawLine(chunkSpawningArea.min, new Vector2(chunkSpawningArea.xMin, chunkSpawningArea.yMax));
        Gizmos.DrawLine(chunkSpawningArea.min, new Vector2(chunkSpawningArea.xMax, chunkSpawningArea.yMin));
        Gizmos.DrawLine(new Vector2(chunkSpawningArea.xMin, chunkSpawningArea.yMax), chunkSpawningArea.max);
        Gizmos.DrawLine(new Vector2(chunkSpawningArea.xMax, chunkSpawningArea.yMin), chunkSpawningArea.max);
        
        if (chunkCoordsInRange == null) return;

        // Draw the center point of chunks within visible range.
        foreach (var coord in chunkCoordsInRange)
        {
            var chunkOrigin = new Vector2(coord.x, coord.y);
            var halfSize = _chunkSize * .5f;
            var chunkCenter = new Vector2(chunkOrigin.x + halfSize, chunkOrigin.y + halfSize);
            Gizmos.DrawWireSphere(new Vector3(chunkCenter.x, chunkCenter.y, 0), .1f);
        }
    }
    
    private float RoundToClosestMultiple(float numToRound, float multiple)
    {
        if (multiple == 0) return numToRound;

        var remainder = Mathf.Abs(numToRound) % multiple;
        if (remainder == 0)
        {
            return numToRound;
        }

        if (numToRound < 0)
        {
            return -(Mathf.Abs(numToRound) - remainder);
        }

        return numToRound + multiple - remainder;
    }
}
