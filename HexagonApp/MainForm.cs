using HexagonApp.Code;
using HexagonLib.Code;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hexagon;
using DemoStrategy;
using HexagonEvolution;

namespace HexagonApp
{
    public partial class MainForm : Form
    {
        private static readonly int INITIAL_SIZE = 17;

        private int size = INITIAL_SIZE;
        private List<Player> players = new List<Player>();
        private HumanPlayer humanPlayer;
        private IO io;
        private Grid grid;

        public MainForm()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            timer1.Stop();

            Player.Reset(players);

            grid = new Grid(size, players.ToArray());
            io = new IO(grid.HexagonCellGrid, players.ToArray());

            UpdateStatus();
        }

        private void picGrid_Paint(object sender, PaintEventArgs e)
        {
            io.DrawGrid(e.Graphics, this.Font, picGrid.ClientSize.Width, picGrid.ClientSize.Height);
        }

        private void picGrid_Resize(object sender, EventArgs e)
        {
            picGrid.Refresh();
        }

        private void picGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (humanPlayer != null && humanPlayer.MakingMove)
            {
                io.ClickOnGrid(picGrid.ClientSize.Width, picGrid.ClientSize.Height, e.X, e.Y);

                picGrid.Refresh();
            }
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("HexagonApp v0.02", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            humanPlayer = null;
            players = new List<Player>();

            size = Convert.ToInt32(e.ClickedItem.Text);

            Initialize();

            picGrid.Refresh();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (humanPlayer != null && humanPlayer.MakingMove)
            {
                timer1.Stop();
            }
            else
            {
                var winner = grid.Play();

                picGrid.Refresh();

                UpdateStatus();

                if (winner != null)
                {
                    timer1.Stop();
                    MessageBox.Show("Winner is " + winner + "\nTurns made: " + grid.TurnCount, "Win!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void UpdateStatus()
        {
            string text = "";
            for (int i = 0; i < players.Count; i++)
            {
                var score = grid.GetStringScore(players[i].OwnerName);
                if (score != "0/0")
                    text += players[i].OwnerName + ": " + score + "; ";
            }
            toolStripStatusLabel1.Text = text;
            toolStripStatusLabel2.Text = "Timer: " + timer1.Interval;
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Initialize();

            picGrid.Refresh();

            timer1.Start();
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (humanPlayer != null && humanPlayer.MakingMove)
                {
                    humanPlayer.EndTurn();

                    timer1.Start();
                }
            }
            else if (e.KeyChar == 'r')
            {
                randomToolStripMenuItem_Click(null, null);
            }
            else if (e.KeyChar == 'j')
            {
                jamal4ToolStripMenuItem_Click(null, null);
            }
            else if (e.KeyChar == 'h')
            {
                humanToolStripMenuItem_Click(null, null);
            }
            else if (e.KeyChar == 'n')
            {
                neatToolStripMenuItem_Click(null, null);
            }
        }

        private void randomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            players.Add(new Player(typeof(RandomStrategy).Name.Replace("Strategy", "") + "_" + players.Count, IO.GetColor(players.Count), new RandomStrategy()));

            Initialize();

            picGrid.Refresh();
        }

        private void humanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (humanPlayer == null)
            {
                humanPlayer = new HumanPlayer();

                players.Add(new Player(HumanPlayer.NAME, IO.GetColor(players.Count), humanPlayer));

                Initialize();

                picGrid.Refresh();
            }
        }

        private void jamalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            players.Add(new Player(typeof(JamalVAlgo1Strategy).Name.Replace("Strategy", "") + "_" + players.Count, IO.GetColor(players.Count), new JamalVAlgo1Strategy()));

            Initialize();

            picGrid.Refresh();
        }

        private void jamal2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            players.Add(new Player(typeof(JamalVAlgo2Strategy).Name.Replace("Strategy", "") + "_" + players.Count, IO.GetColor(players.Count), new JamalVAlgo2Strategy()));

            Initialize();

            picGrid.Refresh();
        }

        private void jamal3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            players.Add(new Player(typeof(JamalVAlgo3Strategy).Name.Replace("Strategy", "") + "_" + players.Count, IO.GetColor(players.Count), new JamalVAlgo3Strategy()));

            Initialize();

            picGrid.Refresh();
        }

        private void jamal4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            players.Add(new Player(typeof(JamalVAlgo4Strategy).Name.Replace("Strategy", "") + "_" + players.Count, IO.GetColor(players.Count), new JamalVAlgo4Strategy()));

            Initialize();

            picGrid.Refresh();
        }

        private void neatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var result = openFileDialog1.ShowDialog();
                if (result != DialogResult.OK)
                    return;

                var neatStrategy = new NeatStrategy(openFileDialog1.FileName, "NeatStrategy");
                players.Add(new Player(typeof(NeatStrategy).Name.Replace("Strategy", "") + "_" + players.Count, IO.GetColor(players.Count), neatStrategy));
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error creating NeatStrategy from file!\nLoading aborted.\n" + exception.Message);
                return;
            }

            Initialize();

            picGrid.Refresh();
        }
    }
}
