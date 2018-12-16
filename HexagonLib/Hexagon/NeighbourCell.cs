namespace Hexagon
{
    public class NeighbourCell
    {
        public string Id { get; }
        public int Resources { get; }
        public CellOwner Owner { get; }

        public NeighbourCell(HexagonCell hexagonCell, string ownerName)
        {
            Id = hexagonCell.Id;
            Resources = hexagonCell.Resources;
            Owner = hexagonCell.OwnerName == ownerName ? CellOwner.Own : (hexagonCell.OwnerName == null ? CellOwner.None : CellOwner.Other);
        }
    }
}