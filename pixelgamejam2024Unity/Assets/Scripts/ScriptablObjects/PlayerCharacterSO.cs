using Multiplayer;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Entities/Player")]
public class PlayerCharacterSO : EntitySO
{
    [SerializeField]
    private float _carryCapacity;

    [SerializeField] private GameConstants.CharacterTypes _characterType;

    [SerializeField] private Color _characterColor;

    public float CarryCapacity
    {
        get => _carryCapacity;
        private set => _carryCapacity = value;
    }

    public GameConstants.CharacterTypes CharacterType
    {
        get => _characterType;
        private set => _characterType = value;
    }

    public Color CharacterColor
    {
        get => _characterColor;
        private set => _characterColor = value;
    }
}
