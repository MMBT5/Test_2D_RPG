using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test_2D_RPG
{
    public class Map
    {
        public const int TileSize = 32;
        public const int Columns = 16;
        public const int Rows = 16;

        private int[,] tiles;

        // Merken sich welches Tile und jedem Block liegt 0 = Gras als Standard
        private int[,] underlayer;

        private Texture2D grass, path, tree, water;
        private Texture2D woodBlock, sunkenBlock;
        private Texture2D goalTile, switchTile, doorTile, usedSwitch;

        public Map North, South, East, West;

        public Map(int[,] tiles,
                   Texture2D grass, Texture2D path, Texture2D tree, Texture2D water, Texture2D woodBlock, Texture2D sunkenBlock, Texture2D goalTile, Texture2D switchTile, Texture2D doorTile, Texture2D usedSwitch)
        {
            this.tiles = tiles;
            this.grass = grass;
            this.path = path;
            this.tree = tree;
            this.water = water;
            this.woodBlock = woodBlock;
            this.sunkenBlock = sunkenBlock;
            this.goalTile = goalTile;
            this.switchTile = switchTile;
            this.doorTile = doorTile;
            this.usedSwitch = usedSwitch;

            underlayer = new int[Rows, Columns];
        }

        // Boden zeichnen
        public void DrawGround(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Rectangle dest = new Rectangle(col * TileSize, row * TileSize, TileSize, TileSize);

                    switch (tiles[row, col])
                    {
                        case 2:  spriteBatch.Draw(path, dest, Color.White); break;
                        case 4:  spriteBatch.Draw(water, dest, Color.White); break;
                        case 5:  spriteBatch.Draw(grass, dest, Color.White); spriteBatch.Draw(woodBlock, dest, Color.White); break;
                        case 6:  spriteBatch.Draw(sunkenBlock, dest, Color.White); break;
                        case 7:  spriteBatch.Draw(grass, dest, Color.White); spriteBatch.Draw(goalTile, dest, Color.White); break;
                        case 8:  spriteBatch.Draw(grass, dest, Color.White); spriteBatch.Draw(switchTile, dest, Color.White); break;
                        case 9:  spriteBatch.Draw(doorTile, dest, Color.White); break;
                        case 10: spriteBatch.Draw(grass, dest, Color.White); spriteBatch.Draw(usedSwitch, dest, Color.White); break;
                        default: spriteBatch.Draw(grass, dest, Color.White); break;
                    }
                }
            }
        }

        // Bäume 1 hinter dem Spieler zeichnen 
        public void DrawTrees(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (tiles[row, col] == 1)
                    {
                        int x = col * TileSize + TileSize / 2 - tree.Width / 2;
                        int y = row * TileSize + TileSize - tree.Height;
                        spriteBatch.Draw(tree, new Rectangle(x, y, tree.Width, tree.Height), Color.White);
                    }
                }
            }
        }

        // Baumstümpfe 3 vor dem Spieler zeichnen 
        // nur die unteren 30px damit man den Spieler noch sieht
        public void DrawTreesUnten(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (tiles[row, col] == 3)
                    {
                        int x = col * TileSize + TileSize / 2 - tree.Width / 2;
                        int stumpY = row * TileSize + TileSize - 30;

                        Rectangle src  = new Rectangle(0, tree.Height - 30, tree.Width, 30);
                        Rectangle dest = new Rectangle(x, stumpY, tree.Width, 30);

                        spriteBatch.Draw(tree, dest, src, Color.White);
                    }
                }
            }
        }

        public bool IsSolid(int col, int row)
        {
            if (col < 0 || col >= Columns || row < 0 || row >= Rows)
                return false;

            int t = tiles[row, col];
            return t == 1 || t == 3 || t == 4 || t == 5 || t == 9;
        }

        public int GetTile(int col, int row)
        {
            if (col < 0 || col >= Columns || row < 0 || row >= Rows) return -1;
            return tiles[row, col];
        }

        public void SetTile(int col, int row, int value)
        {
            if (col < 0 || col >= Columns || row < 0 || row >= Rows) return;
            tiles[row, col] = value;
        }

        // Block schieben
        public bool TryPushBlock(int col, int row, int dirCol, int dirRow)
        {
            if (GetTile(col, row) != 5) return false;

            int zielSpalte = col + dirCol;
            int zielZeile  = row + dirRow;
            int zielTyp    = GetTile(zielSpalte, zielZeile);

            if (zielTyp == 4)
            {
                // Block faellt ins Wasser und versinkt
                SetTile(col, row, underlayer[row, col]);
                underlayer[row, col] = 0;
                SetTile(zielSpalte, zielZeile, 6);
                return true;
            }

            if (zielTyp == 0 || zielTyp == 2 || zielTyp == 6)
            {
                // Block gleitet auf Gras, Weg oder versunkenen Block
                int darunter = underlayer[row, col];
                underlayer[row, col] = 0;
                SetTile(col, row, darunter);

                underlayer[zielZeile, zielSpalte] = zielTyp;
                SetTile(zielSpalte, zielZeile, 5);
                return true;
            }

            return false;
        }

        // Block ziehen (fuer die E-Taste in Player.cs)
        public void MoveBlock(int vonSpalte, int vonZeile, int nachSpalte, int nachZeile)
        {
            int darunter = underlayer[vonZeile, vonSpalte];
            underlayer[vonZeile, vonSpalte] = 0;
            SetTile(vonSpalte, vonZeile, darunter);

            underlayer[nachZeile, nachSpalte] = GetTile(nachSpalte, nachZeile);
            SetTile(nachSpalte, nachZeile, 5);
        }
    }
}
