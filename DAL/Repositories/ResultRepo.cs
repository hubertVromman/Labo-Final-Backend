using DAL.Models.DTO;
using Dapper;
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

        public void AddResult(ResultInfo ri)
        {
            string sql = "INSERT INTO result (RunnerId, RaceId, GeneralRank, GeneralRankShown, GenderRank, Time) " +
                "VALUES (@runnerId, @raceId, @generalRank, @generalRankShown, @genderRank, @time)";

            conn.Execute(sql, ri);
        }
    }
}
