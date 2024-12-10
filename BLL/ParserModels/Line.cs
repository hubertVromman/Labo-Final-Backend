namespace BLL.ParserModels {
  internal class Line {
    public static double Precision { get; set; } = 1;
    public double Position { get; set; }
    public List<Item> Items { get; set; } = new();

    internal Item? FindItemByPosition(double position) {
      foreach (var item in Items) {
        if (Math.Abs(item.Position - position) < Precision)
          return item;
      }
      return null;
    }
  }
}
