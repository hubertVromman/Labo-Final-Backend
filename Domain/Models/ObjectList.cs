using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models {
    public class ObjectList<T> {

        public int Count { get; set; }

        public IEnumerable<T> Objects { get; set; }
    }
}
