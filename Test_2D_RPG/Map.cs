using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test_2D_RPG
{
    public class Map
    {
        public const int TileSize = 32;
        public const int Columns  = 16;
        public const int Rows     = 16;

        private int[,] tiles;

        private Texture2D grass;
        private Texture2D path;
        private Texture2D tree;
        private Texture2D water;
        private Texture2D woodBlock;
        private Texture2D sunkenBlock;
        private Texture2D goalTile;   // Typ 7 = Ziel (Gold)

        public Map North, South, East, West;

        public Map(int[,] tiles, Texture2D grass, Texture2D path, Texture2D tree,
                   Texture2D water, Texture2D woodBlock, Texture2D sunkenBlock,
                   Texture2D goalTile)
        {
            this.tiles       = tiles;
            this.grass       = grass;
            this.path        = path;
            this.tree        = tree;
            this.water       = water;
            this.woodBlock   = woodBlock;
            this.sunkenBlock = sunkenBlock;
            this.goalTile    = goalTile;
        }

        // Schicht 1: Boden (Gras, Weg, Wasser, Bloecke, Ziel)
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
                        case 2: spriteBatch.Draw(path,        dest, Color.White); break;
                        case 4: spriteBatch.Draw(water,       dest, Color.White); break;
                        case 5: // Holzblock: Gras im Hintergrund, Block drueber
                            spriteBatch.Draw(grass,     dest, Color.White);
                            spriteBatch.Draw(woodBlock, dest, Color.White);
                            break;
                        case 6: spriteBatch.Draw(sunkenBlock, dest, Color.White); break;
                        case 7: // Ziel: Gras im Hintergrund, Ziel-Tile drueber
                            spriteBatch.Draw(grass,    dest, Color.White);
                            spriteBatch.Draw(goalTile, dest, Color.White);
                            break;
                        default: spriteBatch.Draw(grass, dest, Color.White); break;
                    }
                }
            }
        }

        // Schicht 2: Baeume HINTER dem Spieler (Typ 1)
        public void DrawTrees(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
            {
                if (tiles[row, col] != 1) continue;
                int x = col * TileSize + (TileSize / 2) - (tree.Width  / 2);
                int y = row * TileSize +  TileSize       - (tree.Height);
                spriteBatch.Draw(tree, new Rectangle(x, y, tree.Width, tree.Height), Color.White);
            }
        }

        // Schicht 4: Baeume VOR dem Spieler (Typ 3)
        public void DrawTreesUnten(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
            {
                if (tiles[row, col] != 3) continue;
                int x = col * TileSize + (TileSize / 2) - (tree.Width  / 2);
                int y = row * TileSize +  TileSize       - (tree.Height);
                spriteBatch.Draw(tree, new Rectangle(x, y, tree.Width, tree.Height), Color.White);
            }
        }

        // Kollision: welche Typen blockieren den Spieler?
        public bool IsSolid(int col, int row)
        {
            if (col < 0 || col >= Columns || row < 0 || row >= Rows)
                return false; // Ausserhalb = Raumwechsel, nicht blockieren

            int t = tiles[row, col];
            // Blockiert: Baum (1), Baum-vorne (3), Wasser (4), Holzblock (5)
            // Begehbar:  Gras (0), Weg (2), Versunken (6), Ziel (7)
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
        // dirCol/dirRow: Schieberichtung (-1, 0 oder 1)
        // Gibt true zurueck wenn der Push erfolgreich war.
        public bool TryPushBlock(int col, int row, int dirCol, int dirRow)
        {
            if (GetTile(col, row) != 5) return false;

            int targetCol  = col + dirCol;
            int targetRow  = row + dirRow;
            int targetTile = GetTile(targetCol, targetRow);

            if (targetTile == 4) // Wasser -> Block versinkt, wird begehbar
            {
                SetTile(col,       row,       0);
                SetTile(targetCol, targetRow, 6);
                return true;
            }
            if (targetTile == 0 || targetTile == 2) // Gras oder Weg -> Block gleitet
            {
                SetTile(col,       row,       0);
                SetTile(targetCol, targetRow, 5);
                return true;
            }

            return false; // Wand, weiterer Block oder Rand -> kein Push moeglich
        }
    }
}