using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hexagon;

namespace DemoStrategy
{
    public class JamalVAlgo1Strategy : IStrategy
    {
        private Random _rnd = new Random(DateTime.Now.Millisecond);

        public JamalVAlgo1Strategy()
        {
        }

        public Transaction Turn(MyCell[] myBlocks)
        {
            var source = GetSource(myBlocks);
            var target = GetTarget(myBlocks);

            var resource = 0;
            if (target.Resources + 1 < source.Resources)
                resource = _rnd.Next(target.Resources + 1, source.Resources);
            else
            {
                target = GetTargetNeighbourOwn(source);
                resource = source.Resources;
            }

            return new Transaction(source.Id, target.Id, resource);
        }

        private MyCell GetSource(MyCell[] myBlocks)
        {
            int neighboursCount = 0;
            MyCell myCell = null;
            for (int i = 0; i < myBlocks.Length; i++)
            {
                int count = myBlocks[i].Neighbours.Count(item => item.Owner == CellOwner.Own);
                if (myCell == null || myBlocks[i].Resources > myCell.Resources || (count > neighboursCount && myBlocks[i].Resources >= myCell.Resources))
                {
                    neighboursCount = count;
                    myCell = myBlocks[i];
                }
            }
            return myCell;
        }

        private NeighbourCell GetTarget(MyCell[] myBlocks)
        {
            NeighbourCell neighbourCellNone = null;
            NeighbourCell neighbourCellOther = null;
            for (int i = 0; i < myBlocks.Length; i++)
            {
                for (int ii = 0; ii < myBlocks[i].Neighbours.Length; ii++)
                {
                    if (myBlocks[i].Neighbours[ii].Owner == CellOwner.None)
                    {
                        if (neighbourCellNone == null || myBlocks[i].Neighbours[ii].Resources < neighbourCellNone.Resources)
                            neighbourCellNone = myBlocks[i].Neighbours[ii];
                    }
                    else if (myBlocks[i].Neighbours[ii].Owner == CellOwner.Other)
                    {
                        if (neighbourCellOther == null || myBlocks[i].Neighbours[ii].Resources < neighbourCellOther.Resources)
                            neighbourCellOther = myBlocks[i].Neighbours[ii];
                    }
                }
            }
            if (neighbourCellNone == null)
                return neighbourCellOther;
            return neighbourCellNone;
        }

        private NeighbourCell GetTargetNeighbourOwn(MyCell source)
        {
            int neighboursCount = 0;
            NeighbourCell neighbourCell = null;
            for (int i = 0; i < source.Neighbours.Length; i++)
            {
                int count = source.Neighbours.Count(item => item.Owner == CellOwner.Own);
                if (neighbourCell == null || count > neighboursCount || (count >= neighboursCount && source.Neighbours[i].Resources >= neighbourCell.Resources))
                {
                    neighboursCount = count;
                    neighbourCell = source.Neighbours[i];
                }
            }
            return neighbourCell;
        }
    }
}
