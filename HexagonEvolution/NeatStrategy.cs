using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using HexagonLib.Code;
using Hexagon;
using System.Linq;
using System.Xml;
using SharpNeat.Genomes.Neat;

namespace HexagonEvolution
{
    public class NeatStrategy : IStrategy
    {
        public IBlackBox Brain { get; private set; }

        public NeatStrategy(IBlackBox brain)
        {
            Brain = brain;
        }

        public NeatStrategy(string file, string name)
        {
            NeatGenome genome = null;
            using (XmlReader xr = XmlReader.Create(file))
                genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false)[0];

            var experiment = new HexagonExperiment();
            experiment.Initialize(name);
            // Get a genome decoder that can convert genomes to phenomes.
            var genomeDecoder = experiment.CreateGenomeDecoder();
            // Decode the genome into a phenome (neural network).
            Brain = genomeDecoder.Decode(genome);
        }

        public Transaction Turn(MyCell[] myBlocks)
        {
            Brain.ResetState();

            var allBlockIds = SetInputSignalArray(Brain.InputSignalArray, myBlocks);

            Brain.Activate();

            var sourceId = GetId(myBlocks.Select(i => i.Id).ToList(), Brain.OutputSignalArray[0]);
            var targetId = GetId(allBlockIds, Brain.OutputSignalArray[1]);
            var resource = GetResource(Brain.OutputSignalArray[2]);

            return new Transaction(sourceId, targetId, resource);
        }

        private List<string> SetInputSignalArray(ISignalArray inputArr, MyCell[] myBlocks)
        {
            var allBlockIds = new List<string>();

            inputArr.Reset();

            for (int row = 0; row < myBlocks.Length; row++)
            {
                for (int col = 0; col < myBlocks[row].Neighbours.Length; col++)
                {
                    if (!allBlockIds.Contains(myBlocks[row].Neighbours[col].Id))
                    {
                        allBlockIds.Add(myBlocks[row].Neighbours[col].Id);
                    }

                    var id = myBlocks[row].Neighbours[col].Id.Split('_');
                    var r = int.Parse(id[0]);
                    var c = int.Parse(id[1]);
                    inputArr[r * myBlocks.Length + c] = myBlocks[row].Neighbours[col].Owner == CellOwner.Own || myBlocks[row].Neighbours[col].Resources < 0
                        ? myBlocks[row].Neighbours[col].Resources
                        : -myBlocks[row].Neighbours[col].Resources;
                }
            }

            allBlockIds.Sort();

            return allBlockIds;
        }

        private string GetId(List<string> blockIds, double d)
        {
            return blockIds[(int)Math.Round(d * (blockIds.Count - 1))];
        }

        private int GetResource(double d)
        {
            return (int)Math.Round(d * Grid.MAX_VALUE);
        }
    }
}
