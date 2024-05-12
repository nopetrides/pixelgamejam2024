using Playroom;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    void Awake()
    {
        Debug.Log($"Loaded into the game with {PlayroomKit.GetPlayers().Count} connected players");
    }
}
