using System.Collections.Generic;

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries;

    public LeaderboardData()
    {
        entries = new List<LeaderboardEntry>();
    }
}