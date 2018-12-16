using DemoStrategy;
using Hexagon;
using HexagonLib.Code;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System.Drawing;

namespace HexagonEvolution
{
    public class HexagonEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private static int PLAY_MAX_TURNS = 40;
        private static int GAME_COUNT = 20;

        private ulong _evalCount;
        private bool _stopConditionSatisfied;
        private int _gridSize;

        public HexagonEvaluator(int gridSize)
        {
            _gridSize = gridSize;
        }

        /// <summary>
        /// Gets the total number of evaluations that have been performed.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox against the random player and return its fitness score.
        /// Each network plays 10 games against the random player and two games against the expert player.
        /// 
        /// A win is worth 10 points, a draw is worth 1 point, and a loss is worth 0 points.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            double fitness = 0;

            var randomPlayer = new Player(typeof(RandomStrategy).Name, new SolidBrush(Color.Green), new RandomStrategy());
            var neatPlayer = new Player(typeof(NeatStrategy).Name, new SolidBrush(Color.Red), new NeatStrategy(box));

            var players = new Player[] { randomPlayer, neatPlayer };

            // Play GAME_COUNT games
            for (int i = 0; i < GAME_COUNT; i++)
            {
                Grid grid = new Grid(_gridSize, players);

                grid.Play(PLAY_MAX_TURNS);

                fitness += grid.GetScore(neatPlayer.OwnerName);
            }

            players = new Player[] { neatPlayer, randomPlayer };
            // Play GAME_COUNT games
            for (int i = 0; i < GAME_COUNT; i++)
            {
                Grid grid = new Grid(_gridSize, players);

                grid.Play(PLAY_MAX_TURNS);

                fitness += grid.GetScore(neatPlayer.OwnerName);
            }

            // Update the evaluation counter.
            _evalCount++;

            // If the network plays perfectly, it will beat the random player
            // and draw the optimal player.
            if (fitness >= GAME_COUNT * 2 * 19)
                _stopConditionSatisfied = true;

            // Return the fitness score
            return new FitnessInfo(fitness, fitness);
        }

        /// <summary>
        /// Returns the score for a game. Scoring is 10 for a win, 1 for a draw
        /// and 0 for a loss. Note that scores cannot be smaller than 0 because
        /// NEAT requires the fitness score to be positive.
        /// </summary>
        /*private int getScore(SquareTypes winner, Owner owner)
        {
            if (winner == neatSquareType)
                return 10;
            if (winner == SquareTypes.N)
                return 1;
            return 0;
        }*/

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// Note. The TicTacToe problem domain has no internal state. This method does nothing.
        /// </summary>
        public void Reset()
        {
        }
    }
}
