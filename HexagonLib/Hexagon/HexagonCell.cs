namespace Hexagon
{
    public class HexagonCell
    {
        public string Id { get; set; }
        public int Resources { get; set; }
        public string OwnerName { get; set; }
        public HexagonCell[] Neighbours { get; set; }
    }
}
