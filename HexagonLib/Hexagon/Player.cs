using System;
using System.Collections.Generic;
using System.Drawing;

namespace Hexagon
{
    public class Player
    {
        public Brush Color { get; }

        public string OwnerName { get; }

        public IStrategy Strategy { get; private set; }

        public Player(string ownerName, Brush color, IStrategy strategy)
        {
            OwnerName = ownerName;
            Color = color;
            Strategy = strategy;
        }

        public static void Reset(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                try
                {
                    var type = players[i].Strategy.GetType();
                    players[i].Strategy = (IStrategy)Activator.CreateInstance(type, true);
                }
                catch
                {
                }
            }
        }
    }
}