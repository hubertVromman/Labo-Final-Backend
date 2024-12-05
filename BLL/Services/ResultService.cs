using DAL.Repositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services {
    public class ResultService(ResultRepo rr) {
        public IEnumerable<Result> GetByRunnerId(int runnerId) {
            return rr.GetByRunnerId(runnerId);
        }
    }
}
