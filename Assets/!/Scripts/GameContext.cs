using UnityEngine;

public enum GameDifficulty
{
    EASY,
    NORMAL,
    HARD
}

public enum Dimension
{
    MAIN_MENU,
    HUB,
    FOREST,
}

[CreateAssetMenu(fileName = "GameContext", menuName = "ScriptableObject/GameContext", order = 1)]
public class GameContext : ScriptableObject
{
    public GameDifficulty Difficulty = GameDifficulty.NORMAL;
    public Dimension CurrentDimension = Dimension.HUB;
    [Range(0, 1)]
    public float Influence = 0;

    public void SetDefault(GameContext context) {
        context.Difficulty = GameDifficulty.NORMAL;
        context.CurrentDimension = Dimension.HUB;
    }
}


