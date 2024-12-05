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
            string sql = "INSERT INTO result (RunnerId, RaceId, GeneralRank, GeneralRankShown, GenderRank, Time) " +
                "VALUES (@runnerId, @raceId, @generalRank, @generalRankShown, @genderRank, @time)";

            conn.Execute(sql, ri);
        }

        public IEnumerable<Result> GetByRunnerId(int runnerId) {
            string sql = "SELECT result.*, race.* FROM result JOIN race ON race.RaceId = result.RaceId WHERE RunnerId = @runnerId";
            return conn.Query<Result, Race, Result>(sql, (result, race) => {
                result.Race = race;
                return result;
            }, new { runnerId },
            splitOn: "RaceId");
        }
    }
}
