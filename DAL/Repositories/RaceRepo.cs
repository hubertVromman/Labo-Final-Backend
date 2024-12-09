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
    public class RaceRepo(SqlConnection conn)
    {
        public Race AddRace(Race r)
        {
            string sql = "INSERT INTO race (RaceName, Place, StartDate, RaceType, Distance, RealDistance) " +
                "VALUES (@raceName, @place, @startDate, @raceType, @distance, @realDistance)";

            conn.Execute(sql, r);

            return conn.QuerySingle<Race>("SELECT TOP 1 * FROM race WHERE RaceName = @raceName AND Place = @place", r);
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
    }
}
