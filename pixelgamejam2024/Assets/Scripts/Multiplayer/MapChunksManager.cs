using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using InsaneScatterbrain.RandomNumberGeneration;
using InsaneScatterbrain.ScriptGraph;
using Multiplayer;
using Playroom;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapChunksManager : MonoBehaviour
{
    [SerializeField] private ScriptGraphRunner _runner;
    [SerializeField] private Camera _cam;
    [SerializeField] private int _chunkSize = 25;
    [SerializeField] private int _spawnRange = 25;
    [SerializeField] private Transform _mapObjectsContainer;
    [SerializeField] private Tilemap _tilemap = null;

    private Queue<Vector2Int> _queuedChunks;
    private HashSet<Vector2Int> _existingChunkCoords;
    private List<Vector2Int> _chunkCoordsInRange;
    private readonly Dictionary<Vector2Int, Transform> _existingChunkParents = new();

    private bool _setupComplete;
    private bool _graphRunnerActive;
    private Rect _chunkSpawningArea;

    private void Awake()
    {
        Debug.Log("[MapChunksManager] Awake");
        Pools.Clear();    // Make sure the map is clear before we start using it.
        _tilemap.ClearAllTiles();    // Make sure the tilemap is clear before we start using it.
        
        _queuedChunks = new Queue<Vector2Int>();
        _existingChunkCoords = new HashSet<Vector2Int>();
        _chunkCoordsInRange = new List<Vector2Int>();

        if (!PlayroomKit.IsRunningInBrowser())
        {
            Debug.Log("[MapChunksManager] Awake - Skipping playroom initialization.");
            InitializeMapChunks();
        }
    }

    public void InitializeMapChunks()
    {
        if (!PlayroomKit.IsRunningInBrowser())
        {
            Debug.Log("[MapChunksManager] InitializeMapChunks - skipping playroom");
            string dateSeed = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            SetupFromServer(GetRandomSeed(dateSeed));
            return;
        }
        if (PlayroomKit.IsHost())
        {
            Debug.Log("[MapChunksManager] InitializeMapChunks - We are host, setting random seed");
            string dateSeed = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            PlayroomKit.SetState(GameConstants.GameStateData.MapSeed.ToString(), dateSeed, true);
            SetupFromServer(GetRandomSeed(dateSeed));
            Debug.Log($"[MapChunksManager] InitializeMapChunks - Host seed is set as {dateSeed}");
        }
        else
        {
            Debug.Log("[MapChunksManager] InitializeMapChunks - Client waiting on map seed");
            _runner.SetIn("NoiseRNGState", GetRandomSeed(DateTime.Now.ToString(CultureInfo.CurrentCulture))); // doesn't do anything but maybe stop a failure
            PlayroomKit.WaitForState(GameConstants.GameStateData.MapSeed.ToString(), WaitForSeedStateCallback);
        }
    }

    private void WaitForSeedStateCallback(string callbackReason)
    {
        if (callbackReason != GameConstants.GameStateData.MapSeed.ToString())
        {
            PlayroomKit.WaitForState(GameConstants.GameStateData.MapSeed.ToString(), WaitForSeedStateCallback);
            return;
        }

        OnSeedReceived();
    }

    private void OnSeedReceived()
    {
        Debug.Log("[MapChunksManager] OnSeedReceived - Received seed, trying to retrieve state...");
        string seed = PlayroomKit.GetState<string>(GameConstants.GameStateData.MapSeed.ToString());
        Debug.Log($"[MapChunksManager] OnSeedReceived - Seed is {seed}");
        SetupFromServer(GetRandomSeed(seed));
    }

    private RngState GetRandomSeed(string seed)
    {
        return RngState.FromBytes(Encoding.ASCII.GetBytes(seed));
    }

    private void SetupFromServer(RngState noiseRngState)
    {
        Debug.Log($"[MapChunksManager] SetupFromServer - Setting up");
        // Set the state for the Perlin Noise Fill Texture nodes.
        _runner.SetIn("NoiseRNGState", noiseRngState);

        _runner.OnProcessed += objects =>
        {
            // If the runner's done processing, it can run for another chunk.
            _graphRunnerActive = false;
            RunNextChunk();
        };
        _setupComplete = true;
        // Run the first chunk.
        RunNextChunk();
    }

    private void RunNextChunk()
    {
        if (!_setupComplete) return;
        Debug.Log($"[MapChunksManager] RunNextChunk - Checking if we generate a chunk");
        if (_queuedChunks.Count < 1) return; // No new chunks to generate.

        if (_graphRunnerActive) return;      // The runner's busy with another chunk, so we wait for it to finish.
        
        _graphRunnerActive = true;
        
        var chunkCoords = _queuedChunks.Dequeue();

        _runner.SetIn("ChunkSize", _chunkSize);
        _runner.SetIn("Coordinates", chunkCoords);
        var startPosition = _mapObjectsContainer.transform.position;
        var chunkParent = Instantiate(_mapObjectsContainer, _mapObjectsContainer.parent);
        chunkParent.position =
            new Vector3(startPosition.x + chunkCoords.x, startPosition.y, startPosition.z + chunkCoords.y);
        _runner.SetIn("Container", chunkParent.gameObject);
        _runner.SetIn("SafeZoneFlagSize", chunkCoords == Vector2Int.zero ? new Vector2Int(_chunkSize, _chunkSize) : Vector2Int.zero);
        _existingChunkParents.Add(chunkCoords, chunkParent);
        //Debug.Log($"[MapChunksManager] RunNextChunk - Chunk Generation beginning");
        _runner.Run();
    }

    private void LateUpdate()
    {
        if (!_setupComplete) return;
        if (_cam == null)
            try
            {
                _cam = Camera.main;
            }
            catch
            {
                Debug.LogWarning("Camera not yet ready");
            }
        
        var camPos = _cam.transform.position;
        
        // Calculate the area in which chunks are generated.
        var halfSize = _chunkSize * .5f;
        
        var xMin = camPos.x - _spawnRange;
        var xMax = camPos.x + _spawnRange;
        var zMin = camPos.z - _spawnRange;
        var zMax = camPos.z + _spawnRange;
        _chunkSpawningArea = new Rect(
            xMin, zMin, xMax - xMin, zMax - zMin
        );

        // Find the min. chunk coordinates in the current area.
        var minChunkCoordsX = 
            Mathf.FloorToInt(RoundToClosestMultiple(xMin - halfSize, _chunkSize));
        var minChunkCoordsZ =
            Mathf.FloorToInt(RoundToClosestMultiple(zMin - halfSize, _chunkSize));

        // Find all the chunk coords that are instead of the current area.
        _chunkCoordsInRange.Clear();
        for (var x = minChunkCoordsX; x < xMax - halfSize; x += _chunkSize)
            for (var z = minChunkCoordsZ; z < zMax - halfSize; z += _chunkSize)
            {
                var chunkCoords = new Vector2Int(x, z);
                _chunkCoordsInRange.Add(chunkCoords);

                if (_existingChunkCoords.Contains(chunkCoords)) continue;    // A chunk has already been generated for these coords.

                _existingChunkCoords.Add(chunkCoords);
                _queuedChunks.Enqueue(chunkCoords);
                RunNextChunk();
            }
        HideChunksOutOfRange();
    }

    private void HideChunksOutOfRange()
    {
        foreach (var kvp in _existingChunkParents)
        {
            if (_chunkCoordsInRange.Contains(kvp.Key))
            {
                if (!kvp.Value.gameObject.activeSelf)
                    kvp.Value.gameObject.SetActive(true);
            }
            else
            {
                if (kvp.Value.gameObject.activeSelf)
                    kvp.Value.gameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        // Draw lines that display the area in which new chunks get generated.
        Gizmos.DrawLine(new Vector3(_chunkSpawningArea.xMin, 0f, _chunkSpawningArea.yMin), new Vector3(_chunkSpawningArea.xMin, 0f, _chunkSpawningArea.yMax));
        Gizmos.DrawLine(new Vector3(_chunkSpawningArea.xMin, 0f, _chunkSpawningArea.yMin), new Vector3(_chunkSpawningArea.xMax, 0f, _chunkSpawningArea.yMin));
        Gizmos.DrawLine(new Vector3(_chunkSpawningArea.xMin, 0f, _chunkSpawningArea.yMax), new Vector3(_chunkSpawningArea.xMax, 0f, _chunkSpawningArea.yMax));
        Gizmos.DrawLine(new Vector3(_chunkSpawningArea.xMax, 0f, _chunkSpawningArea.yMin), new Vector3(_chunkSpawningArea.xMax, 0f, _chunkSpawningArea.yMax));
        
        if (_chunkCoordsInRange == null) return;

        // Draw the center point of chunks within visible range.
        foreach (var coord in _chunkCoordsInRange)
        {
            var halfSize = _chunkSize * .5f;
            var chunkCenterX = coord.x + halfSize;
            var chunkCenterZ = coord.y + halfSize;
            Gizmos.DrawWireSphere(new Vector3(chunkCenterX, 2f, chunkCenterZ), .2f);
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
