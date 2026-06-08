using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Test_2D_RPG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager grafik;
        private SpriteBatch spriteBatch;
        private Player spieler;
        private Map aktuelleMap;

        private Map map1, map2, map3, map4, map5;
        private bool[] schalterAktiv = new bool[4];
        private bool gewonnen = false;
        private Texture2D gewonnenBild;

        public Game1()
        {
            grafik = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Fenstergröße setzen
            grafik.PreferredBackBufferWidth  = 512;
            grafik.PreferredBackBufferHeight = 512;
            grafik.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Texturen laden
            Texture2D gras  = Content.Load<Texture2D>("Tiles/Grass_Middle");
            Texture2D weg   = Content.Load<Texture2D>("Tiles/Path_Middle");
            Texture2D wasser = Content.Load<Texture2D>("Tiles/Water_Middle");
            Texture2D baum   = Content.Load<Texture2D>("Outdoor decoration/Oak_Tree");

            // Farb-Platzhalter fuer Bloecke usw
            Texture2D holzBlock        = FarbeAlsTextur(new Color(139,  90,  43));
            Texture2D versunkenBlock   = FarbeAlsTextur(new Color( 60, 110, 100));
            Texture2D zielKachel       = FarbeAlsTextur(new Color(255, 200,   0));
            Texture2D schalterKachel   = FarbeAlsTextur(new Color(255, 140,   0));
            Texture2D torKachel        = FarbeAlsTextur(new Color(120,  20,  20));
            Texture2D benutzterSchalter = FarbeAlsTextur(new Color( 80,  80,  80));
            gewonnenBild               = FarbeAlsTextur(Color.White);

            // Map 1 - Mitte (Startmap)
            int[,] karte1 = new int[,]
            {
                {  1, 1, 1, 1, 1, 1, 1, 2, 0, 1, 1, 1, 1, 1, 1, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
                {  1, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 1 },
                {  1, 0, 3, 0, 8, 0, 0, 0, 0, 0, 4, 0, 0, 3, 2, 1 },
                {  1, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 3, 2, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 3, 2, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 3, 3, 2, 1 },
                {  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 3, 0, 2, 0 },
                {  2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 3, 0, 3, 0, 2, 0 },
                {  1, 0, 0, 0, 2, 0, 0, 0, 0, 0, 3, 0, 3, 0, 2, 1 },
                {  1, 0, 0, 0, 2, 0, 0, 0, 0, 0, 3, 0, 3, 0, 2, 1 },
                {  1, 0, 0, 5, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1 },
                {  1, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 3, 3, 3, 3, 3, 3, 2, 0, 3, 3, 3, 3, 3, 3, 3 },
            };

            // Map 2 - Rechts
            int[,] karte2 = new int[,]
            {
                {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 0, 1 },
                {  1, 0, 0, 3, 0, 8, 0, 4, 0, 0, 0, 0, 0, 3, 0, 1 },
                {  1, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0, 0, 0, 3, 0, 1 },
                {  1, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 3, 0, 1 },
                {  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 5, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 1 },
                {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            };

            // Map 3 - Oben
            int[,] karte3 = new int[,]
            {
                {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 3, 0, 3, 3, 3, 3, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 3, 0, 0, 0, 4, 0, 3, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 3, 0, 0, 0, 4, 8, 3, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 3, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3, 3, 3 },
            };

            // Map 4 - Links
            int[,] karte4 = new int[,]
            {
                {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 1 },
                {  1, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0, 0, 0, 3, 0, 1 },
                {  1, 0, 0, 3, 0, 0, 3, 3, 3, 3, 3, 0, 0, 3, 0, 0 },
                {  1, 0, 0, 3, 0, 0, 3, 0, 0, 0, 3, 0, 0, 3, 0, 0 },
                {  1, 0, 0, 3, 3, 0, 3, 0, 0, 0, 3, 0, 0, 3, 3, 1 },
                {  1, 0, 0, 0, 3, 0, 3, 3, 3, 3, 3, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 3, 0, 8, 4, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 1 },
                {  1, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1 },
            };

            // Map 5 - UNten
            int[,] karte5 = new int[,]
            {
                {  1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 3, 3, 3, 3, 3, 3, 9, 3, 3, 3, 3, 3, 3, 3, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 3, 3, 3, 3, 3, 3, 9, 3, 3, 3, 3, 3, 3, 3, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 3, 3, 3, 3, 3, 3, 9, 3, 3, 3, 3, 3, 3, 3, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 3, 3, 3, 3, 3, 3, 9, 3, 3, 3, 3, 3, 3, 3, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 7, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            };

            // Karten erstellen und verbinden
            map1 = new Map(karte1, gras, weg, baum, wasser, holzBlock, versunkenBlock, zielKachel, schalterKachel, torKachel, benutzterSchalter);
            map2 = new Map(karte2, gras, weg, baum, wasser, holzBlock, versunkenBlock, zielKachel, schalterKachel, torKachel, benutzterSchalter);
            map3 = new Map(karte3, gras, weg, baum, wasser, holzBlock, versunkenBlock, zielKachel, schalterKachel, torKachel, benutzterSchalter);
            map4 = new Map(karte4, gras, weg, baum, wasser, holzBlock, versunkenBlock, zielKachel, schalterKachel, torKachel, benutzterSchalter);
            map5 = new Map(karte5, gras, weg, baum, wasser, holzBlock, versunkenBlock, zielKachel, schalterKachel, torKachel, benutzterSchalter);

            // Verbindungen zwischen den Karten setzen
            map1.East  = map2; map2.West  = map1;
            map1.North = map3; map3.South = map1;
            map1.West  = map4; map4.East  = map1;
            map1.South = map5; map5.North = map1;

            aktuelleMap = map1;

            // Spieler laden und auf Startposition setzen
            Texture2D spielerBild = Content.Load<Texture2D>("Player/Player");
            spieler = new Player(spielerBild, new Vector2(7 * 32, 11 * 32));
        }

        // erstellt eine 1x1 Textur mit einer Farbe
        private Texture2D FarbeAlsTextur(Color farbe)
        {
            Texture2D t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData(new Color[] { farbe });
            return t;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            if (!gewonnen)
            {
                spieler.Update(gameTime, aktuelleMap);
                KarteWechseln();
                SchalterUndZielPruefen();
            }

            base.Update(gameTime);
        }

        // prueft ob der Spieler den Rand erreicht hat und wechelt dann die Karte
        private void KarteWechseln()
        {
            int groesse = Map.Columns * Map.TileSize;

            if      (spieler.Position.X + 32 > groesse && aktuelleMap.East  != null) { aktuelleMap = aktuelleMap.East;  spieler.Position = new Vector2(1, spieler.Position.Y); }
            else if (spieler.Position.X < 0            && aktuelleMap.West  != null) { aktuelleMap = aktuelleMap.West;  spieler.Position = new Vector2(groesse - 33, spieler.Position.Y); }
            else if (spieler.Position.Y + 32 > groesse && aktuelleMap.South != null) { aktuelleMap = aktuelleMap.South; spieler.Position = new Vector2(spieler.Position.X, 1); }
            else if (spieler.Position.Y < 0            && aktuelleMap.North != null) { aktuelleMap = aktuelleMap.North; spieler.Position = new Vector2(spieler.Position.X, groesse - 33); }
        }

        // schaut ob spieler auf schalter oder Ziel steht
        private void SchalterUndZielPruefen()
        {
            int spalte = (int)((spieler.Position.X + 16) / Map.TileSize);
            int zeile  = (int)((spieler.Position.Y + 28) / Map.TileSize);
            int kachel = aktuelleMap.GetTile(spalte, zeile);

            // Schalter aktivieren
            if (kachel == 8)
            {
                if      (aktuelleMap == map1 && !schalterAktiv[0]) SchalterAktivieren(0, 3);
                else if (aktuelleMap == map2 && !schalterAktiv[1]) SchalterAktivieren(1, 5);
                else if (aktuelleMap == map3 && !schalterAktiv[2]) SchalterAktivieren(2, 7);
                else if (aktuelleMap == map4 && !schalterAktiv[3]) SchalterAktivieren(3, 9);
            }

            // Ziel erreicht?
            if (aktuelleMap == map5 && kachel == 7)
                gewonnen = true;
        }

        private void SchalterAktivieren(int index, int torZeile)
        {
            schalterAktiv[index] = true;
            int spalte = (int)((spieler.Position.X + 16) / Map.TileSize);
            int zeile  = (int)((spieler.Position.Y + 28) / Map.TileSize);
            aktuelleMap.SetTile(spalte, zeile, 10);  // Schalter als benutzt markieren
            map5.SetTile(7, torZeile, 0);             // Tor in Map5 öffnen
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Reihenfolge ist wichtig damit Spieler hinter Bäumen erscheint
            aktuelleMap.DrawGround(spriteBatch);
            aktuelleMap.DrawTrees(spriteBatch);
            spieler.Draw(spriteBatch);
            aktuelleMap.DrawTreesUnten(spriteBatch);

            // Gewonnen-Overlay anzeigen
            if (gewonnen)
                spriteBatch.Draw(gewonnenBild, new Rectangle(0, 0, 512, 512), Color.Gold * 0.55f);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
