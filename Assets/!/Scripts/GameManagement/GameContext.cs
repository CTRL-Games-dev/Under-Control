using UnityEngine;

public enum GameDifficulty
{
    EASY,
    NORMAL,
    HARD
}

public enum Dimension
{
    HUB,
    FOREST,
    VEKTHAR_BOSS,
    ENT_BOSS,
    SLIME_BOSS,
    CARD_CHOOSE,
    TUTORIAL,
}

// [CreateAssetMenu(fileName = "SO_GameContext", menuName = "GameContext")]
// public class GameContext : ScriptableObject
// {
//     public GameDifficulty Difficulty = GameDifficulty.NORMAL;
//     public Dimension CurrentDimension = Dimension.HUB;
//     [Range(0, 1)]
//     public float Influence = 0;

//     public void SetDefault(GameContext context) {
//         context.Difficulty = GameDifficulty.NORMAL;
//         context.CurrentDimension = Dimension.HUB;
//     }
// }


