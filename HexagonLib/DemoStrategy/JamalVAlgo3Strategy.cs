using System;
using System.Collections.Generic;
using System.Linq;
using Hexagon;

namespace DemoStrategy
{
    public class JamalVAlgo3Strategy : IStrategy
    {
        private Random _rnd = new Random(DateTime.Now.Millisecond);

        private List<string> ids = new List<string>();

        public JamalVAlgo3Strategy()
        {
        }

        public Transaction Turn(MyCell[] myBlocks)
        {
            var source = GetSource(myBlocks);

            NeighbourCell target = null;

            if (myBlocks.Length >= ids.Count)
            {
                AddId(myBlocks);
                target = GetTarget(myBlocks, source.Resources);

                if (target == null)
                    target = GetTargetOther(myBlocks, source.Resources);
            }
            else
            {
                target = GetTargetOtherAggressively(myBlocks, source.Resources);
            }

            var resource = 0;
            if (target != null)
                resource = _rnd.Next(target.Resources + 1, source.Resources);
            else
            {
                target = GetTargetNeighbourOwn(myBlocks, source);
                resource = source.Resources;
            }

            return new Transaction(source.Id, target.Id, resource);
        }

        private void AddId(MyCell[] myBlocks)
        {
            for (int i = 0; i < myBlocks.Length; i++)
            {
                if (!ids.Contains(myBlocks[i].Id))
                    ids.Add(myBlocks[i].Id);
            }
        }

        private NeighbourCell GetTarget(MyCell[] myBlocks, int sourceResources)
        {
            var myCell = GetMyCellById(myBlocks, ids.LastOrDefault());
            string previousId = null;
            if (ids.Count > 2)
                previousId = ids[ids.Count - 2];

            List<NeighbourCell> others = new List<NeighbourCell>();
            List<MyCell> own = new List<MyCell>();

            if (myCell == null)
                return null;

            for (int i = 0; i < myCell.Neighbours.Length; i++)
            {
                if (myCell.Neighbours[i].Owner != CellOwner.Own)
                {
                    others.Add(myCell.Neighbours[i]);
                }
                else if (myCell.Neighbours[i].Owner == CellOwner.Own && previousId != myCell.Neighbours[i].Id)
                {
                    own.Add(GetMyCellById(myBlocks, myCell.Neighbours[i].Id));
                }
            }

            for (int i = 0; i < own.Count; i++)
            {
                for (int ii = 0; ii < own[i].Neighbours.Length; ii++)
                {
                    for (int iii = 0; iii < others.Count; iii++)
                    {
                        if (own[i].Neighbours[ii].Id == others[iii].Id)
                        {
                            if (others[iii].Resources < sourceResources)
                            {
                                return others[iii];
                            }
                            break;
                        }
                    }
                }
            }

            return GetTarget(others, sourceResources);
        }

        private NeighbourCell GetTarget(List<NeighbourCell> neighbourCells, int sourceResources)
        {
            NeighbourCell neighbourCell = null;
            for (int i = 0; i < neighbourCells.Count; i++)
            {
                if (neighbourCell == null || neighbourCells[i].Resources < neighbourCell.Resources)
                    neighbourCell = neighbourCells[i];
            }

            if (neighbourCell != null && neighbourCell.Resources < sourceResources)
                return neighbourCell;

            return null;
        }

        private MyCell GetMyCellById(MyCell[] myBlocks, string id)
        {
            for (int i = 0; i < myBlocks.Length; i++)
            {
                if (myBlocks[i].Id == id)
                {
                    return myBlocks[i];
                }
            }
            return null;
        }

        private MyCell GetSource(MyCell[] myBlocks)
        {
            int sumResources = 0;
            int neighboursCount = 0;
            MyCell myCell = null;
            for (int i = 0; i < myBlocks.Length; i++)
            {
                int sum = myBlocks[i].Neighbours.Sum(item => item.Owner == CellOwner.Own ? item.Resources : 0);
                int count = myBlocks[i].Neighbours.Count(item => item.Owner == CellOwner.Own);
                if (myCell == null || myBlocks[i].Resources > myCell.Resources || (sum > sumResources && count > neighboursCount && myBlocks[i].Resources >= myCell.Resources))
                {
                    sumResources = sum;
                    neighboursCount = count;
                    myCell = myBlocks[i];
                }
            }
            return myCell;
        }

        private NeighbourCell GetTargetOtherAggressively(MyCell[] myBlocks, int sourceResources)
        {
            int c = 0;
            NeighbourCell countNeighbourCell = null;
            for (int i = 0; i < myBlocks.Length; i++)
            {
                for (int ii = 0; ii < myBlocks[i].Neighbours.Length; ii++)
                {
                    if (myBlocks[i].Neighbours[ii].Owner != CellOwner.Own)
                    {
                        int count = myBlocks[i].Neighbours.Count(item => item.Owner == CellOwner.Own);
                        if (countNeighbourCell == null || myBlocks[i].Neighbours[ii].Resources <= countNeighbourCell.Resources && count > c)
                        {
                            c = count;
                            countNeighbourCell = myBlocks[i].Neighbours[ii];
                        }
                    }
                }
            }

            if (countNeighbourCell != null && countNeighbourCell.Resources < sourceResources)
                return countNeighbourCell;

            return null;
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
                    if (myBlocks[i].Neighbours[ii].Owner != CellOwner.Own)
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

        private NeighbourCell GetTargetNeighbourOwn(MyCell[] myBlocks, MyCell source)
        {
            int neighboursCount = 0;
            NeighbourCell neighbourCell = null;
            for (int i = 0; i < source.Neighbours.Length; i++)
            {
                if (source.Neighbours[i].Owner == CellOwner.Own)
                {
                    int count = source.Neighbours.Count(item => item.Owner == CellOwner.Own);
                    if (neighbourCell == null || count > neighboursCount || (count >= neighboursCount && source.Neighbours[i].Resources >= neighbourCell.Resources))
                    {
                        neighboursCount = count;
                        neighbourCell = source.Neighbours[i];
                    }
                }
            }
            if (neighbourCell != null)
                return neighbourCell;

            for (int i = 0; i < myBlocks.Length; i++)
            {
                for (int ii = 0; ii < myBlocks[i].Neighbours.Length; ii++)
                {
                    if (myBlocks[i].Neighbours[ii].Owner == CellOwner.Own)
                    {
                        return myBlocks[i].Neighbours[ii];
                    }
                }
            }

            for (int i = 0; i < myBlocks.Length; i++)
            {
                for (int ii = 0; ii < myBlocks[i].Neighbours.Length; ii++)
                {
                    if (neighbourCell == null || source.Resources >= neighbourCell.Resources)
                    {
                        neighbourCell = myBlocks[i].Neighbours[ii];
                    }
                }
            }

            return neighbourCell;
        }
    }
}
