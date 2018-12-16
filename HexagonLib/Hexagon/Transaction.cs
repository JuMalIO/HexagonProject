namespace Hexagon
{
    public class Transaction
    {
        public string SourceId { get; }
        public string TargetId { get; }
        public int Resource { get; }

        public Transaction(string sourceId, string targetId, int resource)
        {
            this.SourceId = sourceId;
            this.TargetId = targetId;
            this.Resource = resource;
        }
    }
}