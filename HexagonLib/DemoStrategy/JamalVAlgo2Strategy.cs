using System;
using System.Linq;
using Hexagon;

namespace DemoStrategy
{
    public class JamalVAlgo2Strategy : IStrategy
    {
        private Random _rnd = new Random(DateTime.Now.Millisecond);

        public JamalVAlgo2Strategy()
        {
        }

        public Transaction Turn(MyCell[] myBlocks)
        {
            var source = GetSource(myBlocks);

            var target = GetTargetNone(myBlocks);

            if (target == null)
                target = GetTargetOther(myBlocks, source.Resources);

            var resource = 0;
            if (target != null)
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

        private NeighbourCell GetTargetNone(MyCell[] myBlocks)
        {
            NeighbourCell neighbourCell = null;
            for (int i = 0; i < myBlocks.Length; i++)
            {
                for (int ii = 0; ii < myBlocks[i].Neighbours.Length; ii++)
                {
                    if (myBlocks[i].Neighbours[ii].Owner == CellOwner.None)
                    {
                        if (neighbourCell == null || myBlocks[i].Neighbours[ii].Resources < neighbourCell.Resources)
                            neighbourCell = myBlocks[i].Neighbours[ii];
                    }
                }
            }
            return neighbourCell;
        }

        private NeighbourCell GetTargetOther(MyCell[] myBlocks, int sourceResources)
        {
            const int RESOURCES = 1;

            NeighbourCell minNeighbourCell = null;
            NeighbourCell[] countNeighbourCell = new NeighbourCell[7];
            for (int i = 0; i < myBlocks.Length; i++)
            {
                for (int ii = 0; ii < myBlocks[i].Neighbours.Length; ii++)
                {
                    if (myBlocks[i].Neighbours[ii].Owner == CellOwner.Other)
                    {
                        if (minNeighbourCell == null || myBlocks[i].Neighbours[ii].Resources < minNeighbourCell.Resources)
                            minNeighbourCell = myBlocks[i].Neighbours[ii];

                        int count = myBlocks[i].Neighbours.Count(item => item.Owner == CellOwner.Own);

                        if (countNeighbourCell[count] == null || myBlocks[i].Neighbours[ii].Resources <= countNeighbourCell[count].Resources)
                            countNeighbourCell[count] = myBlocks[i].Neighbours[ii];
                    }
                }
            }

            for (int i = countNeighbourCell.Length - 1; i >= 0; i--)
            {
                if (countNeighbourCell[i] != null)
                {
                    if (countNeighbourCell[i].Resources - i * RESOURCES < minNeighbourCell.Resources && countNeighbourCell[i].Resources < sourceResources)
                        return countNeighbourCell[i];
                }
            }

            if (minNeighbourCell.Resources < sourceResources)
                return minNeighbourCell;

            return null;
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
