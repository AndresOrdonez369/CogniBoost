[System.Serializable]
public class LeaderboardEntry
{
    public string playerName; // Nombre simple del jugador
    public int score;
    public int total_clicks; // Opcional, pero puede ser interesante mostrar
    public int total_time;   // Opcional

    // Constructor para facilitar la creación
    public LeaderboardEntry(string name, int scr, int clicks, int time)
    {
        playerName = name;
        score = scr;
        total_clicks = clicks;
        total_time = time;
    }
}