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

        // Merkt sich welches Tile UNTER jedem Block liegt (0 = Gras als Standard).
        // Ohne dieses Array würde beim Verschieben eines Blocks der Weg- oder
        // Versunken-Tile darunter mit Gras überschrieben.
        private int[,] underlayer;

        private Texture2D grass, path, tree, water;
        private Texture2D woodBlock, sunkenBlock;
        private Texture2D goalTile, switchTile, doorTile, usedSwitch;

        public Map North, South, East, West;

        public Map(int[,] tiles,
                   Texture2D grass, Texture2D path, Texture2D tree,
                   Texture2D water, Texture2D woodBlock, Texture2D sunkenBlock,
                   Texture2D goalTile, Texture2D switchTile,
                   Texture2D doorTile, Texture2D usedSwitch)
        {
            this.tiles       = tiles;
            this.grass       = grass;
            this.path        = path;
            this.tree        = tree;
            this.water       = water;
            this.woodBlock   = woodBlock;
            this.sunkenBlock = sunkenBlock;
            this.goalTile    = goalTile;
            this.switchTile  = switchTile;
            this.doorTile    = doorTile;
            this.usedSwitch  = usedSwitch;

            // Mit 0 (Gras) initialisiert – korrekt fuer alle Startbloecke,
            // die immer auf Gras platziert werden.
            underlayer = new int[Rows, Columns];
        }

        // Schicht 1: Boden
        public void DrawGround(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
            {
                Rectangle dest = new Rectangle(col * TileSize, row * TileSize, TileSize, TileSize);
                switch (tiles[row, col])
                {
                    case 2:  spriteBatch.Draw(path,        dest, Color.White); break;
                    case 4:  spriteBatch.Draw(water,       dest, Color.White); break;
                    case 5:
                        spriteBatch.Draw(grass,     dest, Color.White);
                        spriteBatch.Draw(woodBlock, dest, Color.White); break;
                    case 6:  spriteBatch.Draw(sunkenBlock, dest, Color.White); break;
                    case 7:
                        spriteBatch.Draw(grass,    dest, Color.White);
                        spriteBatch.Draw(goalTile, dest, Color.White); break;
                    case 8:
                        spriteBatch.Draw(grass,      dest, Color.White);
                        spriteBatch.Draw(switchTile, dest, Color.White); break;
                    case 9:  spriteBatch.Draw(doorTile,  dest, Color.White); break;
                    case 10:
                        spriteBatch.Draw(grass,      dest, Color.White);
                        spriteBatch.Draw(usedSwitch, dest, Color.White); break;
                    default: spriteBatch.Draw(grass, dest, Color.White); break;
                }
            }
        }

        // Schicht 2: Baeume HINTER dem Spieler – voller Baum (Typ 1)
        public void DrawTrees(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
            {
                if (tiles[row, col] != 1) continue;
                int x = col * TileSize + TileSize / 2 - tree.Width / 2;
                int y = row * TileSize + TileSize - tree.Height;
                spriteBatch.Draw(tree, new Rectangle(x, y, tree.Width, tree.Height), Color.White);
            }
        }

        // Schicht 4: Baumstumpf VOR dem Spieler (Typ 3)
        public void DrawTreesUnten(SpriteBatch spriteBatch)
        {
            const int stumpH = 30;

            for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
            {
                if (tiles[row, col] != 3) continue;

                int x      = col * TileSize + TileSize / 2 - tree.Width / 2;
                int stumpY = row * TileSize + TileSize - stumpH;

                Rectangle src  = new Rectangle(0, tree.Height - stumpH, tree.Width, stumpH);
                Rectangle dest = new Rectangle(x, stumpY, tree.Width, stumpH);

                spriteBatch.Draw(tree, dest, src, Color.White);
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

        // Verschiebt einen Block per SCHIEBEN (wird aus Player.cs aufgerufen).
        // Stellt den Tile-Typ unter dem Block korrekt wieder her –
        // z.B. bleibt ein Weg-Tile erhalten wenn der Block darueber geschoben wird.
        public bool TryPushBlock(int col, int row, int dirCol, int dirRow)
        {
            if (GetTile(col, row) != 5) return false;

            int tc = col + dirCol;
            int tr = row + dirRow;
            int tt = GetTile(tc, tr);

            if (tt == 4) // Wasser → Block versinkt
            {
                // Ursprungsposition: was unter dem Block lag, wiederherstellen
                SetTile(col, row, underlayer[row, col]);
                underlayer[row, col] = 0;

                // Zielposition: wird versunken (kein underlayer-Eintrag noetig)
                SetTile(tc, tr, 6);
                return true;
            }

            if (tt == 0 || tt == 2 || tt == 6) // Gras, Weg oder versunkener Block
            {
                // Ursprungsposition wiederherstellen
                int savedUnder = underlayer[row, col];
                underlayer[row, col] = 0;
                SetTile(col, row, savedUnder);

                // Zielposition: was dort lag merken, Block platzieren
                underlayer[tr, tc] = tt;
                SetTile(tc, tr, 5);
                return true;
            }

            return false;
        }

        // Verschiebt einen Block per ZIEHEN (wird aus Player.cs aufgerufen).
        // Identisch zur Push-Logik, aber Richtung und Pruefung kommen vom Aufrufer.
        // Gibt true zurueck wenn der Block erfolgreich bewegt wurde.
        public bool MoveBlock(int fromCol, int fromRow, int toCol, int toRow)
        {
            if (GetTile(fromCol, fromRow) != 5) return false;

            int toTile = GetTile(toCol, toRow);
            if (toTile != 0 && toTile != 2 && toTile != 6) return false;

            // Ursprungsposition wiederherstellen
            int savedUnder = underlayer[fromRow, fromCol];
            underlayer[fromRow, fromCol] = 0;
            SetTile(fromCol, fromRow, savedUnder);

            // Zielposition: was dort lag merken, Block platzieren
            underlayer[toRow, toCol] = toTile;
            SetTile(toCol, toRow, 5);

            return true;
        }
    }
}
