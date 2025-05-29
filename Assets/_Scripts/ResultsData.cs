
[System.Serializable]
public class ResultsData
{
    public int total_clicks;
    public int total_time;
    public int pairs;
    public int score;
}

[System.Serializable]
public class ResultsWrapper 
{
    public ResultsData results; //El nombre de este campo debe ser "results" para coincidir con el JSON
}