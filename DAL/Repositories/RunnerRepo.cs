using DAL.Models.DTO;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories {
    public class RunnerRepo(SqlConnection conn) {

        public Runner? GetRunnerByName(string firstname, string lastname) {
            string sql = "SELECT * FROM runner WHERE Firstname = @firstname AND Lastname = @lastname";
            return conn.QuerySingleOrDefault<Runner>(sql, new { firstname, lastname });
        }

        public Runner AddRunner(string firstname, string lastname, string? gender = null) {
            string sql = "INSERT INTO runner (Firstname, Lastname, Gender) " +
                "VALUES (@firstname, @lastname, @gender)";

            conn.Execute(sql, new { firstname, lastname, gender });

            return GetRunnerByName(firstname, lastname)!;
        }
    }
}
