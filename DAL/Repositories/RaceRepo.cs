﻿using Dapper;
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
        public int AddRace(Race r)
        {
            string sql = "INSERT INTO race (RaceName, Place, StartDate, RaceType, Distance, RealDistance) " +
                "VALUES (@raceName, @place, @startDate, @raceType, @distance, @realDistance)";

            conn.Execute(sql, r);

            return conn.QuerySingle<int>("SELECT TOP 1 RaceId FROM race WHERE RaceName = @raceName AND Place = @place", r);
        }
    }
}