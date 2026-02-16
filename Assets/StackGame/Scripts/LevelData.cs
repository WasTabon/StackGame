using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "STACK/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public GoalType goalType;
    public int goalValue;
    public float spawnInterval = 8f;
    public int maxLayers = 10;
    public int startingLayers = 6;
    public int colorCount = 5;

    public enum GoalType
    {
        RemoveLayers,
        ReachScore,
        SurviveTime,
        ChainReaction
    }

    public string GetGoalDescription()
    {
        switch (goalType)
        {
            case GoalType.RemoveLayers:
                return "Remove " + goalValue + " layers";
            case GoalType.ReachScore:
                return "Score " + goalValue + " points";
            case GoalType.SurviveTime:
                return "Survive " + goalValue + " seconds";
            case GoalType.ChainReaction:
                return "Get a x" + goalValue + " chain";
            default:
                return "";
        }
    }
}
