using DAL.Repositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services {
    public class LocalityService(LocalityRepo lr) {

        public IEnumerable<Locality> GetAll() {
            return lr.GetAll();
        }
    }
}
