namespace Project.WebApi.Models;

public class LeaderBoard
{
    public int TotalTrips { get; set; }
    public double TotalDistance { get; set; }
    public double AverageTripDistance { get; set; }
    public IList<UserLeadStats> UserLeadStats { get; set; }
}

public class UserLeadStats
{
    public string Name { get; set; }
    public double Distance { get; set; }
    public int NumberOfTrips { get; set; }
    public double AverageTripUser { get; set; }
    public int TotalTripsUser { get; set; }
}