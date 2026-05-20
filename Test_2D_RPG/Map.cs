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

        private Texture2D grass;
        private Texture2D path;
        private Texture2D tree;
        private Texture2D water;
        private Texture2D woodBlock;
        private Texture2D sunkenBlock;

        public Map North, South, East, West;

        public Map(int[,] tiles, Texture2D grass, Texture2D path, Texture2D tree,
                   Texture2D water, Texture2D woodBlock, Texture2D sunkenBlock)
        {
            this.tiles = tiles;
            this.grass = grass;
            this.path = path;
            this.tree = tree;
            this.water = water;
            this.woodBlock = woodBlock;
            this.sunkenBlock = sunkenBlock;
        }

        public void DrawGround(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Rectangle dest = new Rectangle(
                        col * TileSize, row * TileSize, TileSize, TileSize);

                    switch (tiles[row, col])
                    {
                        case 2: spriteBatch.Draw(path, dest, Color.White); break;
                        case 4: spriteBatch.Draw(water, dest, Color.White); break;
                        case 5:
                            spriteBatch.Draw(grass, dest, Color.White);
                            spriteBatch.Draw(woodBlock, dest, Color.White);
                            break;
                        case 6: spriteBatch.Draw(sunkenBlock, dest, Color.White); break;
                        default: spriteBatch.Draw(grass, dest, Color.White); break;
                    }
                }
            }
        }

        // Bäume hinter dem Spieler (Typ 1)
        public void DrawTrees(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (tiles[row, col] != 1) continue;
                    int x = col * TileSize + (TileSize / 2) - (tree.Width / 2);
                    int y = row * TileSize + TileSize - tree.Height;
                    spriteBatch.Draw(tree, new Rectangle(x, y, tree.Width, tree.Height), Color.White);
                }
            }
        }

        // Bäume vor dem Spieler (Typ 3)
        public void DrawTreesUnten(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (tiles[row, col] != 3) continue;
                    int x = col * TileSize + (TileSize / 2) - (tree.Width / 2);
                    int y = row * TileSize + TileSize - tree.Height;
                    spriteBatch.Draw(tree, new Rectangle(x, y, tree.Width, tree.Height), Color.White);
                }
            }
        }

        // Kollision: 1=Baum, 3=Baum, 4=Wasser, 5=Holzblock blockieren
        // 0=Gras, 2=Weg, 6=Versunkener Block sind betretbar
        public bool IsSolid(int col, int row)
        {
            if (col < 0 || col >= Columns || row < 0 || row >= Rows)
                return false;

            int t = tiles[row, col];
            return t == 1 || t == 3 || t == 4 || t == 5;
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

        // Versucht einen Holzblock (Typ 5) zu verschieben.
        // Gibt true zurück wenn der Block erfolgreich verschoben wurde.
        public bool TryPushBlock(int col, int row, int dirCol, int dirRow)
        {
            if (GetTile(col, row) != 5) return false;

            int targetCol = col + dirCol;
            int targetRow = row + dirRow;
            int targetTile = GetTile(targetCol, targetRow);

            if (targetTile == 4) // Wasser → Block versinkt, wird betretbar
            {
                SetTile(col, row, 0);
                SetTile(targetCol, targetRow, 6);
                return true;
            }
            if (targetTile == 0 || targetTile == 2) // Gras oder Weg → Block gleitet
            {
                SetTile(col, row, 0);
                SetTile(targetCol, targetRow, 5);
                return true;
            }

            return false; // Wand, weiterer Block oder Kartenrand → kein Push möglich
        }
    }
}