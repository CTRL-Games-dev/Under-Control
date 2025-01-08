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
    public GameDifficulty difficulty = GameDifficulty.NORMAL;
    public Dimension currentDimension = Dimension.HUB;
    [Range(0, 1)]
    public float influence = 0;

    public void SetDefault(GameContext context) {
        context.difficulty = GameDifficulty.NORMAL;
        context.currentDimension = Dimension.HUB;
    }
}


