using Dapper;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL.Repositories {
  public class RaceRepo(SqlConnection conn) {
    public Race AddRaceIfNotExist(Race r) {
            Console.WriteLine(r.RealDistance);
      Race? race = conn.QuerySingleOrDefault<Race>("SELECT TOP 1 * FROM race WHERE RaceName = @raceName AND StartDate = @startDate AND Distance = @distance", r);
      if (race is not null)
        throw new Exception("Race already exists");

      string sql = "INSERT INTO race (RaceName, Place, StartDate, RaceType, Distance, RealDistance) " +
          "VALUES (@raceName, @place, @startDate, @raceType, @distance, @realDistance)";

      conn.Execute(sql, r);

      return conn.QuerySingle<Race>("SELECT TOP 1 * FROM race WHERE RaceName = @raceName AND StartDate = @startDate AND Distance = @distance", r);
    }

    public void UpdateResultNumber(int raceId, int resultNumber) {
      string sql = "UPDATE race SET resultNumber = @resultNumber WHERE RaceId = @raceId";
      conn.Execute(sql, new { raceId, resultNumber });
    }

    public ObjectList<Race> GetByDate(int offset, int limit) {
      string sql = "SELECT * FROM race ORDER BY StartDate DESC OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";
      IEnumerable<Race> raceList = conn.Query<Race>(sql, new { offset, limit });
      sql = "SELECT COUNT(1) FROM race";
      int count = conn.QuerySingle<int>(sql);
      return new ObjectList<Race> { Count = count, Objects = raceList };
    }

    public Race GetById(int id) {
      string sql = "SELECT * FROM race WHERE RaceId = @id";
      return conn.QuerySingle<Race>(sql, new { id });
    }

    public IEnumerable<Race> Search(string query) {
      string[] fragments = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

      string sql = "SELECT * FROM race WHERE";

      var parameters = new DynamicParameters();
      for (int i = 0; i < fragments.Length; i++) {
        parameters.Add("fragment" + i, "%" + fragments[i] + "%", DbType.String, ParameterDirection.Input, fragments[i].Length + 2);
        if (i > 0)
          sql += " AND ";
        sql += " RaceName LIKE @fragment" + i;
      }
      return conn.Query<Race>(sql, parameters);
    }
  }
}
