using Dapper;
using Domain.Forms;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL.Repositories {
  public class ResultRepo(SqlConnection conn) {

    public void AddResult(ResultForm ri) {
      string sql = "INSERT INTO result (RunnerId, RaceId, GeneralRank, GeneralRankShown, GenderRank, Time, Speed, Pace) " +
          "VALUES (@runnerId, @raceId, @generalRank, @generalRankShown, @genderRank, @time, @speed, @pace)";

      conn.Execute(sql, ri);
    }

    public ObjectList<Result> GetByRaceId(int raceId, int offset, int limit) {
      string sql = "SELECT result.*, runner.* FROM result JOIN runner ON runner.RunnerId = result.RunnerId WHERE RaceId = @raceId ORDER BY GeneralRank OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";
      IEnumerable<Result> resultList = conn.Query<Result, Runner, Result>(sql, (result, runner) => {
        result.Runner = runner;
        return result;
      }, new { raceId, offset, limit },
      splitOn: "RunnerId");
      sql = "SELECT COUNT(1) FROM result WHERE RaceId = @raceId";
      int count = conn.QuerySingle<int>(sql, new { raceId });
      return new ObjectList<Result> { Count = count, Objects = resultList };
    }

    public ObjectList<Result> GetByRaceIdWithFilter(int raceId, int offset, int limit, string name) {
      string[] fragments = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

      string nameFilter = "";

      var parameters = new DynamicParameters(new { raceId, offset, limit });
      for (int i = 0; i < fragments.Length; i++) {
        parameters.Add("fragment" + i, fragments[i] + "%", DbType.String, ParameterDirection.Input, fragments[i].Length + 1);
        nameFilter += " AND ";
        nameFilter += " (Lastname LIKE @fragment" + i;
        nameFilter += " OR Firstname LIKE @fragment" + i + ")";
      }
      //parameters.Add("raceId", raceId);

      string sql = "SELECT result.*, runner.* FROM result JOIN runner ON runner.RunnerId = result.RunnerId WHERE RaceId = @raceId " + nameFilter + " ORDER BY GeneralRank OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";
      IEnumerable<Result> resultList = conn.Query<Result, Runner, Result>(sql, (result, runner) => {
        result.Runner = runner;
        return result;
      }, parameters,
      splitOn: "RunnerId");
      sql = "SELECT COUNT(1) FROM result WHERE RaceId = @raceId";
      int count = conn.QuerySingle<int>(sql, new { raceId });
      return new ObjectList<Result> { Count = count, Objects = resultList };
    }

    public IEnumerable<Result> GetByRunnerId(int runnerId) {
      string sql = "SELECT result.*, race.* FROM result JOIN race ON race.RaceId = result.RaceId WHERE RunnerId = @runnerId ORDER BY StartDate DESC";
      return conn.Query<Result, Race, Result>(sql, (result, race) => {
        result.Race = race;
        return result;
      }, new { runnerId },
      splitOn: "RaceId");
    }
  }
}
