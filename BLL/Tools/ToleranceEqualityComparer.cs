namespace BLL.Tools {
    public class ToleranceEqualityComparer : IEqualityComparer<double> {
        public double Tolerance { get; set; } = 1;
        public bool Equals(double x, double y) {
            return x - Tolerance <= y && x + Tolerance > y;
        }

        //This is to force the use of Equals methods.
        public int GetHashCode(double obj) => 1;
    }
}
