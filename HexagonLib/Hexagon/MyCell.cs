namespace Hexagon
{
    public class MyCell
    {
        public string Id { get; }
        public int Resources { get; }
        public NeighbourCell[] Neighbours { get; }

        public MyCell(HexagonCell hexagonCell, string ownerName)
        {
            Id = hexagonCell.Id;
            Resources = hexagonCell.Resources;
            Neighbours = new NeighbourCell[hexagonCell.Neighbours.Length];
            for (int i = 0; i < hexagonCell.Neighbours.Length; i++)
            {
                Neighbours[i] = new NeighbourCell(hexagonCell.Neighbours[i], ownerName);
            }
        }
    }
}