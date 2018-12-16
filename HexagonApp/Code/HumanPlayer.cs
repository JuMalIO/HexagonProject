using Hexagon;

namespace HexagonApp.Code
{
    public class HumanPlayer : IStrategy
    {
        public static string NAME = "Human";

        public bool MakingMove { get; private set; }

        private Transaction transaction;

        public HumanPlayer()
        {
            MakingMove = true;
        }

        public Transaction Turn(MyCell[] myBlocks)
        {
            return transaction;
        }

        public void SetTransaction(Transaction transaction)
        {
            this.transaction = transaction;
        }

        public void EndTurn()
        {
            MakingMove = false;
        }
    }
}
