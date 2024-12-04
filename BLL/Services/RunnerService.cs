using DAL.Repositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services {
    public class RunnerService(RunnerRepo rr) {
        public Runner? GetById(int id) {
            return rr.GetById(id);
        }

        public Runner? GetByName(string firstname, string lastname) {
            return rr.GetRunnerByName(firstname, lastname);
        }
    }
}
