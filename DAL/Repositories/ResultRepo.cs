using Dapper;
using Domain.Forms;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ResultRepo(SqlConnection conn)
    {

        public void AddResult(ResultForm ri)
        {
            string sql = "INSERT INTO result (RunnerId, RaceId, GeneralRank, GeneralRankShown, GenderRank, Time, Speed, Pace) " +
                "VALUES (@runnerId, @raceId, @generalRank, @generalRankShown, @genderRank, @time, @speed, @pace)";

            conn.Execute(sql, ri);
        }

        public IEnumerable<Result> GetByRaceId(int raceId) {
            string sql = "SELECT result.*, runner.* FROM result JOIN runner ON runner.RunnerId = result.RunnerId WHERE RaceId = @raceId ORDER BY GeneralRank";
            return conn.Query<Result, Runner, Result>(sql, (result, runner) => {
                result.Runner = runner;
                return result;
            }, new { raceId },
            splitOn: "RunnerId");
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
