namespace Domain.Models {
  public class ObjectList<T> {

    public int Count { get; set; }

    public IEnumerable<T> Objects { get; set; }
  }
}
