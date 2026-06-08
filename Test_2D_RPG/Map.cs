using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test_2D_RPG
{
    public class Map
    {
        public const int TileSize = 32;
        public const int Columns  = 16;
        public const int Rows     = 16;

        private int[,] kacheln;

        // speichert was unter einem Block liegt
        // sonst wird beim verschieben immer Gras gesetzt 
        private int[,] unterKachel;

        private Texture2D gras, weg, baum, wasser;
        private Texture2D holzBlock, versunkenBlock;
        private Texture2D zielKachel, schalterKachel, torKachel, benutzterSchalter;

        // Nachbarkarten
        public Map North, South, East, West;

        public Map(int[,] kacheln,
                   Texture2D gras, Texture2D weg, Texture2D baum,
                   Texture2D wasser, Texture2D holzBlock, Texture2D versunkenBlock,
                   Texture2D zielKachel, Texture2D schalterKachel,
                   Texture2D torKachel, Texture2D benutzterSchalter)
        {
            this.kacheln          = kacheln;
            this.gras             = gras;
            this.weg              = weg;
            this.baum             = baum;
            this.wasser           = wasser;
            this.holzBlock        = holzBlock;
            this.versunkenBlock   = versunkenBlock;
            this.zielKachel       = zielKachel;
            this.schalterKachel   = schalterKachel;
            this.torKachel        = torKachel;
            this.benutzterSchalter = benutzterSchalter;

            unterKachel = new int[Rows, Columns]; // standartmäßig 0 (Gras)
        }

        // Boden zeichnen (Schicht 1)
        public void DrawGround(SpriteBatch spriteBatch)
        {
            for (int zeile = 0; zeile < Rows; zeile++)
            for (int spalte = 0; spalte < Columns; spalte++)
            {
                Rectangle bereich = new Rectangle(spalte * TileSize, zeile * TileSize, TileSize, TileSize);
                switch (kacheln[zeile, spalte])
                {
                    case 2: spriteBatch.Draw(weg,             bereich, Color.White); break;
                    case 4: spriteBatch.Draw(wasser,          bereich, Color.White); break;
                    case 5:
                        spriteBatch.Draw(gras,      bereich, Color.White);
                        spriteBatch.Draw(holzBlock, bereich, Color.White); break;
                    case 6: spriteBatch.Draw(versunkenBlock,  bereich, Color.White); break;
                    case 7:
                        spriteBatch.Draw(gras,       bereich, Color.White);
                        spriteBatch.Draw(zielKachel, bereich, Color.White); break;
                    case 8:
                        spriteBatch.Draw(gras,           bereich, Color.White);
                        spriteBatch.Draw(schalterKachel, bereich, Color.White); break;
                    case 9: spriteBatch.Draw(torKachel,       bereich, Color.White); break;
                    case 10:
                        spriteBatch.Draw(gras,              bereich, Color.White);
                        spriteBatch.Draw(benutzterSchalter, bereich, Color.White); break;
                    default: spriteBatch.Draw(gras, bereich, Color.White); break;
                }
            }
        }

        // Bäume hinter dem Spieler zeichnen (Typ 1)
        public void DrawTrees(SpriteBatch spriteBatch)
        {
            for (int zeile = 0; zeile < Rows; zeile++)
            for (int spalte = 0; spalte < Columns; spalte++)
            {
                if (kacheln[zeile, spalte] != 1) continue;
                int x = spalte * TileSize + TileSize / 2 - baum.Width / 2;
                int y = zeile  * TileSize + TileSize     - baum.Height;
                spriteBatch.Draw(baum, new Rectangle(x, y, baum.Width, baum.Height), Color.White);
            }
        }

        // Baumstümpfe vor dem Spieler zeichnen (Typ 3)
        // nur die unteren 30px damit man den Spieler noch sieht
        public void DrawTreesUnten(SpriteBatch spriteBatch)
        {
            const int stumpfHoehe = 30;

            for (int zeile = 0; zeile < Rows; zeile++)
            for (int spalte = 0; spalte < Columns; spalte++)
            {
                if (kacheln[zeile, spalte] != 3) continue;

                int x      = spalte * TileSize + TileSize / 2 - baum.Width / 2;
                int stumpY = zeile  * TileSize + TileSize - stumpfHoehe;

                Rectangle quelle = new Rectangle(0, baum.Height - stumpfHoehe, baum.Width, stumpfHoehe);
                Rectangle ziel   = new Rectangle(x, stumpY, baum.Width, stumpfHoehe);

                spriteBatch.Draw(baum, ziel, quelle, Color.White);
            }
        }

        // gibt zurück ob eine Kachel den Spieler blockiert
        public bool IsSolid(int spalte, int zeile)
        {
            if (spalte < 0 || spalte >= Columns || zeile < 0 || zeile >= Rows)
                return false;
            int t = kacheln[zeile, spalte];
            return t == 1 || t == 3 || t == 4 || t == 5 || t == 9;
        }

        public int GetTile(int spalte, int zeile)
        {
            if (spalte < 0 || spalte >= Columns || zeile < 0 || zeile >= Rows) return -1;
            return kacheln[zeile, spalte];
        }

        public void SetTile(int spalte, int zeile, int wert)
        {
            if (spalte < 0 || spalte >= Columns || zeile < 0 || zeile >= Rows) return;
            kacheln[zeile, spalte] = wert;
        }

        // Versuch einen Block in eine Richtung zu schieben
        public bool TryPushBlock(int spalte, int zeile, int richtungX, int richtungY)
        {
            if (GetTile(spalte, zeile) != 5) return false;

            int zielSpalte = spalte + richtungX;
            int zielZeile  = zeile  + richtungY;
            int zielTyp    = GetTile(zielSpalte, zielZeile);

            if (zielTyp == 4) // Wasser : Block versinct
            {
                SetTile(spalte, zeile, unterKachel[zeile, spalte]);
                unterKachel[zeile, spalte] = 0;
                SetTile(zielSpalte, zielZeile, 6);
                return true;
            }

            if (zielTyp == 0 || zielTyp == 2 || zielTyp == 6) // Gras, Weg oder versunkener Block
            {
                int gespeichert = unterKachel[zeile, spalte];
                unterKachel[zeile, spalte] = 0;
                SetTile(spalte, zeile, gespeichert);

                unterKachel[zielZeile, zielSpalte] = zielTyp;
                SetTile(zielSpalte, zielZeile, 5);
                return true;
            }

            return false;
        }

        // Block ziehen (wird von Player.cs aufgerufen)
        public bool MoveBlock(int vonSpalte, int vonZeile, int nachSpalte, int nachZeile)
        {
            if (GetTile(vonSpalte, vonZeile) != 5) return false;

            int zielTyp = GetTile(nachSpalte, nachZeile);
            if (zielTyp != 0 && zielTyp != 2 && zielTyp != 6) return false;

            int gespeichert = unterKachel[vonZeile, vonSpalte];
            unterKachel[vonZeile, vonSpalte] = 0;
            SetTile(vonSpalte, vonZeile, gespeichert);

            unterKachel[nachZeile, nachSpalte] = zielTyp;
            SetTile(nachSpalte, nachZeile, 5);

            return true;
        }
    }
}
