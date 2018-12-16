using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace HexagonEvolution
{
    public class HexagonExperiment : SimpleNeatExperiment
    {
        private static int GRID_SIZE = 17;
        private static int INPUTS = GRID_SIZE * GRID_SIZE;
        private static int OUTPUTS = 3;

        public override IPhenomeEvaluator<IBlackBox> PhenomeEvaluator
        {
            get { return new HexagonEvaluator(GRID_SIZE); }
        }

        public override int InputCount
        {
            get { return INPUTS; }
        }

        public override int OutputCount
        {
            get { return OUTPUTS; }
        }

        public override bool EvaluateParents
        {
            get { return true; }
        }
    }
}
