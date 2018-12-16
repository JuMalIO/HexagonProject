using System;
using System.Linq;
using Hexagon;

namespace DemoStrategy
{
    public class RandomStrategy : IStrategy
    {
        private Random _rnd = new Random(DateTime.Now.Millisecond);

        public RandomStrategy()
        {
        }

        public Transaction Turn(MyCell[] myBlocks)
        {
            var rndIndexTarget = _rnd.Next(0, myBlocks.Count());
            var rndIndexSource = _rnd.Next(0, myBlocks.Count());
            var source = myBlocks[rndIndexSource];
            var rndResource = _rnd.Next(0, source.Resources);
            var targetBlock = myBlocks[rndIndexTarget];

            var neighbourRnd = _rnd.Next(1, targetBlock.Neighbours.Length);
            var target = targetBlock.Neighbours[neighbourRnd];

            return new Transaction(source.Id, target.Id, rndResource);
        }
    }
}
