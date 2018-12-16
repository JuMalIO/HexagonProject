namespace Hexagon
{
    public interface IStrategy
    {
        Transaction Turn(MyCell[] myBlocks);
    }
}