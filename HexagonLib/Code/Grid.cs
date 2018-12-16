using System;
using System.Linq;
using Hexagon;
using System.Collections.Generic;

namespace HexagonLib.Code
{
    public class Grid
    {
        public static int MAX_VALUE = 100;

        public HexagonCell[][] HexagonCellGrid { get; private set; }
        public int TurnCount { get; private set; }

        private Player[] players;

        public Grid(int size, Player[] players)
        {
            this.players = players;

            HexagonCellGrid = new HexagonCell[size][];

            for (int i = 0; i < HexagonCellGrid.Length; i++)
            {
                HexagonCellGrid[i] = new HexagonCell[size];
            }

            FillGrid();
        }

        private void FillGrid()
        {
            int size = (int)Math.Ceiling(HexagonCellGrid.Length / 2.0);
            int topFirstLineHexagonEnabledCount = size % 2 == 1 ? 3 : 1;
            int bottomFirstLineHexagonEnabledCount = size % 2 == 1 ? 1 : 3;

            for (int row = 0; row < HexagonCellGrid.Length; row++)
            {
                for (int col = 0; col < HexagonCellGrid[row].Length; col++)
                {
                    int topDisabled = (HexagonCellGrid.Length - (topFirstLineHexagonEnabledCount + row * 4)) / 2;
                    int bottomDisabled = (HexagonCellGrid.Length - (bottomFirstLineHexagonEnabledCount + (HexagonCellGrid.Length - (row + 1)) * 4)) / 2;
                    bool enabled = (col < topDisabled || col >= HexagonCellGrid.Length - topDisabled) ||
                        (col < bottomDisabled || col >= HexagonCellGrid.Length - bottomDisabled) ? false : true;

                    if (enabled)
                    {
                        HexagonCellGrid[row][col] = new HexagonCell()
                        {
                            Id = row + "_" + col,
                            OwnerName = null,
                            Resources = 0,
                            Neighbours = null
                        };
                    }
                }
            }

            for (int row = 0; row < HexagonCellGrid.Length; row++)
            {
                for (int col = 0; col < HexagonCellGrid[row].Length; col++)
                {
                    if (HexagonCellGrid[row][col] != null)
                    {
                        HexagonCellGrid[row][col].Neighbours = GetHexagonCellNeighbours(row, col);
                    }
                }
            }

            var random = new Random();
            foreach (int i in Enumerable.Range(0, players.Length).OrderBy(x => random.Next()))
            {
                const int INC = 4;
                int row = INC;
                int col = 0;
                while (true)
                {
                    if (row >= HexagonCellGrid.Length)
                    {
                        throw new Exception("Grid too small for players");
                    }
                    else if (col >= HexagonCellGrid.Length)
                    {
                        col = INC;
                        row += INC;
                    }
                    else
                    {
                        col += INC;
                    }

                    HexagonCell hexagonCell = null;
                    if ((hexagonCell = GetHexagonCellByRowCol(row, col)) != null && hexagonCell.OwnerName == null)
                    {
                        hexagonCell.Resources = MAX_VALUE;
                        hexagonCell.OwnerName = players[i].OwnerName;
                        break;
                    }
                }
            }
        }

        private HexagonCell[] GetHexagonCellNeighbours(int row, int col)
        {
            int down = 0, up = 0;
            if (col % 2 == 0)
                up = 1;
            else
                down = 1;

            var neighbours = new List<HexagonCell>();
            HexagonCell hexagonCell = null;
            if ((hexagonCell = GetHexagonCellByRowCol(row - 1, col)) != null)
                neighbours.Add(hexagonCell);
            if ((hexagonCell = GetHexagonCellByRowCol(row - up, col - 1)) != null)
                neighbours.Add(hexagonCell);
            if ((hexagonCell = GetHexagonCellByRowCol(row - up, col + 1)) != null)
                neighbours.Add(hexagonCell);
            if ((hexagonCell = GetHexagonCellByRowCol(row + down, col - 1)) != null)
                neighbours.Add(hexagonCell);
            if ((hexagonCell = GetHexagonCellByRowCol(row + down, col + 1)) != null)
                neighbours.Add(hexagonCell);
            if ((hexagonCell = GetHexagonCellByRowCol(row + 1, col)) != null)
                neighbours.Add(hexagonCell);

            return neighbours.ToArray();
        }

        private HexagonCell GetHexagonCellByRowCol(int row, int col)
        {
            if (row >= 0 && col >= 0 && HexagonCellGrid.Length > row && HexagonCellGrid[row].Length > col)
                return HexagonCellGrid[row][col];
            return null;
        }


        private MyCell[] GetMyCells(string ownerName)
        {
            List<MyCell> myCells = new List<MyCell>();
            for (int row = 0; row < HexagonCellGrid.Length; row++)
            {
                for (int col = 0; col < HexagonCellGrid[row].Length; col++)
                {
                    if (HexagonCellGrid[row][col] != null && HexagonCellGrid[row][col].OwnerName == ownerName)
                    {
                        myCells.Add(new MyCell(HexagonCellGrid[row][col], ownerName));
                    }
                }
            }
            return myCells.ToArray();
        }

        private HexagonCell GetHexagonCellById(string id)
        {
            for (int row = 0; row < HexagonCellGrid.Length; row++)
            {
                for (int col = 0; col < HexagonCellGrid[row].Length; col++)
                {
                    if (HexagonCellGrid[row][col] != null && HexagonCellGrid[row][col].Id == id)
                    {
                        return HexagonCellGrid[row][col];
                    }
                }
            }
            return null;
        }

        public string Play(int turns)
        {
            for (int t = 0; t < turns; ++t)
            {
                var winner = Play();
                if (winner != null)
                    return winner;
            }
            return null;
        }

        public string Play()
        {
            TurnCount++;

            for (int i = 0; i < players.Length; ++i)
            {
                var player = players[i];
                var myCells = GetMyCells(player.OwnerName);
                if (myCells.Length > 0)
                {
                    var transaction = player.Strategy.Turn(myCells);
                    ValidateTransaction(transaction, player.OwnerName);
                }
            }

            FinishTurn();

            var winner = IsWinner();
            if (winner != null)
            {
                return winner;
            }

            return null;
        }

        private bool ValidateTransaction(Transaction transaction, string owner)
        {
            var source = GetHexagonCellById(transaction.SourceId);
            var target = GetHexagonCellById(transaction.TargetId);
            if (source != null && target != null)
            {
                for (int i = 0; i < target.Neighbours.Length; ++i)
                {
                    if (target.OwnerName == owner || target.Neighbours[i].OwnerName == owner)
                    {
                        if (source.Resources >= transaction.Resource)
                        {
                            if (source.OwnerName != target.OwnerName)
                            {
                                if (source.Resources > target.Resources)
                                {
                                    source.Resources -= transaction.Resource;
                                    target.Resources = transaction.Resource;
                                    target.OwnerName = source.OwnerName;
                                    return true;
                                }
                            }
                            else
                            {
                                source.Resources -= transaction.Resource;
                                target.Resources += transaction.Resource;
                                return true;
                            }
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        private void FinishTurn()
        {
            for (int row = 0; row < HexagonCellGrid.Length; row++)
            {
                for (int col = 0; col < HexagonCellGrid[row].Length; col++)
                {
                    if (HexagonCellGrid[row][col] != null)
                    {
                        if (HexagonCellGrid[row][col].OwnerName != null && HexagonCellGrid[row][col].Resources < MAX_VALUE)
                        {
                            HexagonCellGrid[row][col].Resources++;
                        }
                    }
                }
            }
        }

        public string IsWinner()
        {
            string winning = null;

            for (int row = 0; row < HexagonCellGrid.Length; row++)
            {
                for (int col = 0; col < HexagonCellGrid[row].Length; col++)
                {
                    if (HexagonCellGrid[row][col] != null)
                    {
                        if (winning == null)
                        {
                            winning = HexagonCellGrid[row][col].OwnerName;
                        }

                        if (winning != HexagonCellGrid[row][col].OwnerName)
                        {
                            return null;
                        }
                    }
                }
            }

            return winning;
        }

        public int GetScore(string ownerName)
        {
            int score = 0;
            for (int row = 0; row < HexagonCellGrid.Length; row++)
            {
                for (int col = 0; col < HexagonCellGrid[row].Length; col++)
                {
                    if (HexagonCellGrid[row][col] != null && HexagonCellGrid[row][col].OwnerName == ownerName)
                    {
                        score++;
                    }
                }
            }
            return score;
        }

        public string GetStringScore(string ownerName)
        {
            int score = 0;
            int total = 0;
            for (int row = 0; row < HexagonCellGrid.Length; row++)
            {
                for (int col = 0; col < HexagonCellGrid[row].Length; col++)
                {
                    if (HexagonCellGrid[row][col] != null && HexagonCellGrid[row][col].OwnerName == ownerName)
                    {
                        score++;
                        total += HexagonCellGrid[row][col].Resources;
                    }
                }
            }
            return score + "/" + total;
        }
    }
}
