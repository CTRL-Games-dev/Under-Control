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
    public float Influence;
    // {
    //     get;
    //     set
    //     {
    //         if(value > 1) 
    //         {
    //             Influence = 1;
    //             Debug.LogWarning("Influence cannot be set to something bigger than 1");
    //         } 
    //         else if(value < 0)
    //         {
    //             Influence = 0;
    //             Debug.LogWarning("Influence cannot be set to something smaller than 0");
    //         } 
    //         else 
    //         {
    //             Influence = value;
    //         }
    //     }
    // }

    public void SetDefault(GameContext context) {
        context.difficulty = GameDifficulty.NORMAL;
        context.currentDimension = Dimension.HUB;
    }
}


