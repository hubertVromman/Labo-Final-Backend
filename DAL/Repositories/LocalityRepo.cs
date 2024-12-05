using Dapper;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories {
    public class LocalityRepo(SqlConnection conn) {

        public IEnumerable<Locality> GetAll() {
            string sql = "SELECT * FROM [locality]";
            return conn.Query<Locality>(sql);
        }
    }
}
