using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hexagon;

namespace HexagonApp.Code
{
    public class IO
    {
        private static float SQRT_3 = (float)Math.Sqrt(3);
        private static Pen PEN = Pens.Black;
        private static Pen PEN_CLICKED = new Pen(Color.Red, 2);
        private static Brush BRUSHES = Brushes.Black;

        private HexagonCell clicked;
        private HexagonCell[][] hexagonCellGrid;
        private Player[] players;

        public IO(HexagonCell[][] hexagonCellGrid, Player[] players)
        {
            this.players = players;
            this.hexagonCellGrid = hexagonCellGrid;
        }

        public void DrawGrid(Graphics gr, Font font, float xmax, float ymax)
        {
            gr.SmoothingMode = SmoothingMode.AntiAlias;

            float s1 = (xmax - 1) / ((hexagonCellGrid.Length + 1) / 2 + hexagonCellGrid.Length);
            float s2 = (ymax - 1) / (hexagonCellGrid.Length * SQRT_3 + SQRT_3 / 2);

            var s = Math.Min(s1, s2);
            PointF[] clickedPoints = null;

            for (int row = 0; row < hexagonCellGrid.Length; row++)
            {
                for (int col = 0; col < hexagonCellGrid[row].Length; col++)
                {
                    if (hexagonCellGrid[row][col] != null)
                    {
                        var points = HexToPoints(s, row, col);

                        if (hexagonCellGrid[row][col] == clicked)
                            clickedPoints = points;
                        else
                            gr.DrawPolygon(PEN, points);

                        gr.FillPolygon(GetBrushByOwner(hexagonCellGrid[row][col].OwnerName, players), points);

                        using (StringFormat sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            float x = (points[0].X + points[3].X) / 2;
                            float y = (points[1].Y + points[4].Y) / 2;
                            string label = hexagonCellGrid[row][col].Resources.ToString();
                            gr.DrawString(label, font, BRUSHES, x, y, sf);
                        }
                    }
                }
            }

            if (clickedPoints != null)
                gr.DrawPolygon(PEN_CLICKED, clickedPoints);
        }

        private Brush GetBrushByOwner(string ownerName, Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].OwnerName == ownerName)
                    return players[i].Color;
            }
            return Brushes.White;
        }

        public static Brush GetColor(int count)
        {
            var random = new Random();
            Color[] colors = new Color[] {
                Color.AntiqueWhite,
                Color.Aqua,
                Color.Aquamarine,
                Color.Azure,
                Color.Beige,
                Color.Bisque,
                Color.Black,
                Color.BlanchedAlmond,
                Color.Blue,
                Color.BlueViolet,
                Color.Brown,
                Color.BurlyWood,
                Color.CadetBlue,
                Color.Chartreuse,
                Color.Chocolate,
                Color.Coral,
                Color.CornflowerBlue,
                Color.Cornsilk,
                Color.Crimson,
                Color.Cyan,
                Color.DarkBlue,
                Color.DarkCyan,
                Color.DarkGoldenrod,
                Color.DarkGray,
                Color.DarkGreen,
                Color.DarkKhaki,
                Color.DarkMagenta,
                Color.DarkOliveGreen,
                Color.DarkOrange,
                Color.DarkOrchid,
                Color.DarkRed,
                Color.DarkSalmon,
                Color.DarkSeaGreen,
                Color.DarkSlateBlue,
                Color.DarkSlateGray,
                Color.DarkTurquoise,
                Color.DarkViolet,
                Color.DeepPink,
                Color.DeepSkyBlue,
                Color.DimGray,
                Color.DodgerBlue,
                Color.Firebrick,
                Color.FloralWhite,
                Color.ForestGreen,
                Color.Fuchsia,
                Color.Gainsboro,
                Color.GhostWhite,
                Color.Gold,
                Color.Goldenrod,
                Color.Gray,
                Color.Green,
                Color.GreenYellow,
                Color.Honeydew,
                Color.HotPink,
                Color.IndianRed,
                Color.Indigo,
                Color.Ivory,
                Color.Khaki,
                Color.Lavender,
                Color.LavenderBlush,
                Color.LawnGreen,
                Color.LemonChiffon,
                Color.LightBlue,
                Color.LightCoral,
                Color.LightCyan,
                Color.LightGoldenrodYellow,
                Color.LightGray,
                Color.LightGreen,
                Color.LightPink,
                Color.LightSalmon,
                Color.LightSeaGreen,
                Color.LightSkyBlue,
                Color.LightSlateGray,
                Color.LightSteelBlue,
                Color.LightYellow,
                Color.Lime,
                Color.LimeGreen,
                Color.Linen,
                Color.Magenta,
                Color.Maroon,
                Color.MediumAquamarine,
                Color.MediumBlue,
                Color.MediumOrchid,
                Color.MediumPurple,
                Color.MediumSeaGreen,
                Color.MediumSlateBlue,
                Color.MediumSpringGreen,
                Color.MediumTurquoise,
                Color.MediumVioletRed,
                Color.MidnightBlue,
                Color.MintCream,
                Color.MistyRose,
                Color.Moccasin,
                Color.NavajoWhite,
                Color.Navy,
                Color.OldLace,
                Color.Olive,
                Color.OliveDrab,
                Color.Orange,
                Color.OrangeRed,
                Color.Orchid,
                Color.PaleGoldenrod,
                Color.PaleGreen,
                Color.PaleTurquoise,
                Color.PaleVioletRed,
                Color.PapayaWhip,
                Color.PeachPuff,
                Color.Peru,
                Color.Pink,
                Color.Plum,
                Color.PowderBlue,
                Color.Purple,
                Color.Red,
                Color.RosyBrown,
                Color.RoyalBlue,
                Color.SaddleBrown,
                Color.Salmon,
                Color.SandyBrown,
                Color.SeaGreen,
                Color.SeaShell,
                Color.Sienna,
                Color.Silver,
                Color.SkyBlue,
                Color.SlateBlue,
                Color.SlateGray,
                Color.Snow,
                Color.SpringGreen,
                Color.SteelBlue,
                Color.Tan,
                Color.Teal,
                Color.Thistle,
                Color.Tomato,
                Color.Transparent,
                Color.Turquoise,
                Color.Violet,
                Color.Wheat,
                Color.White,
                Color.WhiteSmoke,
                Color.Yellow,
                Color.YellowGreen
            };
            return new SolidBrush(colors[count % 140]);
        }

        private PointF[] HexToPoints(float s, float row, float col)
        {
            float height = SQRT_3 * s;
            float width = 2 * s;

            float y = height / 2;
            float x = col * (width * 0.75f);

            y += row * height;

            if (col % 2 == 1)
                y += height / 2;

            return new PointF[]
            {
                new PointF(x, y),
                new PointF(x + width * 0.25f, y - height / 2),
                new PointF(x + width * 0.75f, y - height / 2),
                new PointF(x + width, y),
                new PointF(x + width * 0.75f, y + height / 2),
                new PointF(x + width * 0.25f, y + height / 2),
            };
        }

        public bool ClickOnGrid(float xmax, float ymax, float x, float y)
        {
            PointF p = new PointF(x, y);

            float s1 = (xmax - 1) / ((hexagonCellGrid.Length + 1) / 2 + hexagonCellGrid.Length);
            float s2 = (ymax - 1) / (hexagonCellGrid.Length * SQRT_3 + SQRT_3 / 2);

            var s = Math.Min(s1, s2);

            for (int row = 0; row < hexagonCellGrid.Length; row++)
            {
                for (int col = 0; col < hexagonCellGrid[row].Length; col++)
                {
                    if (hexagonCellGrid[row][col] != null)
                    {
                        var points = HexToPoints(s, row, col);

                        if (IsPointInPolygon(points, p))
                        {
                            clicked = hexagonCellGrid[row][col];
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsPointInPolygon(PointF[] polygon, PointF testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
    }
}
