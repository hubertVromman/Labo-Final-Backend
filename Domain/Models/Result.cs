namespace Domain.Models {
  public class Result {
    public Race Race { get; set; }
    public Runner Runner { get; set; }

    public int RaceId { get; set; }
    public int RunnerId { get; set; }
    public int GeneralRank { get; set; }
    public string GeneralRankShown { get; set; }
    public int GenderRank { get; set; }
    public TimeOnly Time { get; set; }
    public Decimal Speed { get; set; }
    public string Pace { get; set; }
  }
}
