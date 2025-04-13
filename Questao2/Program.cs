using System.Net.Http.Json;

public class Program
{
    public static async Task Main()
    {
        using var client = new HttpClient();

        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await GetTotalScoredGoals(client, teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await GetTotalScoredGoals(client, teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> GetTotalScoredGoals(HttpClient httpClient, string team, int year)
    {
        var team1GoalsTask = GetGoalsFromMatches(httpClient, team, year, "team1");

        var team2GoalsTask = GetGoalsFromMatches(httpClient, team, year, "team2");
        await Task.WhenAll(team1GoalsTask, team2GoalsTask);

        return await team1GoalsTask + await team2GoalsTask;
    }

    private static async Task<int> GetGoalsFromMatches(HttpClient httpClient, string team, int year, string role)
    {
        int goals = 0;
        int page = 1;
        int totalPages;

        do
        {
            string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{role}={team}&page={page}";
            var response = await httpClient.GetFromJsonAsync<ApiResponse>(url);

            foreach (var match in response.data)
            {
                if (role == "team1")
                    goals += int.Parse(match.team1goals);
                else
                    goals += int.Parse(match.team2goals);
            }

            totalPages = response.total_pages;
            page++;
        }
        while (page <= totalPages);

        return goals;
    }

    public class Match
    {
        public string team1 { get; set; }
        public string team2 { get; set; }
        public string team1goals { get; set; }
        public string team2goals { get; set; }
    }

    public class ApiResponse
    {
        public int page { get; set; }
        public int total_pages { get; set; }
        public List<Match> data { get; set; }
    }
}