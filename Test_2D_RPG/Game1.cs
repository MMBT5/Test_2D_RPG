using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Test_2D_RPG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Player player;
        private Map currentMap;
        private Map map5; // Referenz auf Map 5 fuer Ziel-Pruefung

        private bool gameWon = false;
        private Texture2D winOverlay;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth  = 512;
            graphics.PreferredBackBufferHeight = 512;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Texturen laden
            Texture2D grass = Content.Load<Texture2D>("Tiles/Grass_Middle");
            Texture2D path  = Content.Load<Texture2D>("Tiles/Path_Middle");
            Texture2D tree  = Content.Load<Texture2D>("Outdoor decoration/Oak_Tree");
            Texture2D water = Content.Load<Texture2D>("Tiles/Water_Middle");

            // Holzblock: braune Farbe (austauschbar mit echtem Sprite)
            Texture2D woodBlock = new Texture2D(GraphicsDevice, 1, 1);
            woodBlock.SetData(new Color[] { new Color(139, 90, 43) });

            // Versunkener Block: blaugruen
            Texture2D sunkenBlock = new Texture2D(GraphicsDevice, 1, 1);
            sunkenBlock.SetData(new Color[] { new Color(60, 110, 100) });

            // Ziel-Tile: gold
            Texture2D goalTile = new Texture2D(GraphicsDevice, 1, 1);
            goalTile.SetData(new Color[] { new Color(255, 200, 0) });

            // Gewonnen-Overlay: transparent gold
            winOverlay = new Texture2D(GraphicsDevice, 1, 1);
            winOverlay.SetData(new Color[] { Color.White });

            // ═══════════════════════════════════════════════════
            // MAP 1 – ZENTRUM
            // Raetsel: 1 Block (Typ 5) bei (3,11), Wasser (Typ 4) bei (3,12)
            // Loesung:  Block nach OSTEN schieben -> landet im Wasser -> Typ 6
            // Raetsel-Bereich: oben-rechts, eingerahmt durch Baeume
            // Ausgaenge: Nord (Zeile 0, Sp.7-8), Ost (Z.13-14, Sp.15),
            //            Sued (Zeile 15, Sp.7-8), West (Z.6-7, Sp.0)
            // ═══════════════════════════════════════════════════
            int[,] map1Data = new int[,]
            {
                { 1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1 }, // Z  0: Nord-Ausgang Sp.7-8
                { 1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1 }, // Z  1
                { 1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1 }, // Z  2: Raetsel-Bereich oben
                { 1,0,0,0,0,0,0,0,0,1,0,5,4,0,0,1 }, // Z  3: Block(11), Wasser(12)
                { 1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1 }, // Z  4: Raetsel-Bereich unten
                { 1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1 }, // Z  5
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  6: West-Ausgang Sp.0
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  7: West-Ausgang Sp.0
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  8
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  9
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 10
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 11
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 12
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2 }, // Z 13: Ost-Ausgang Sp.15
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // Z 14: Ost-Ausgang Sp.15
                { 1,3,3,3,3,3,3,0,0,3,3,3,3,3,3,3 }, // Z 15: Sued-Ausgang Sp.7-8
            };

            // ═══════════════════════════════════════════════════
            // MAP 2 – OSTEN
            // Raetsel: 2 Bloecke (Z.6, Sp.5 und 6), Wasser (Z.5, Sp.5 und 6)
            // Loesung:  Beide Bloecke nach NORDEN schieben
            // Raetsel-Bereich: linke Mitte
            // Ausgang: West (Z.13-14, Sp.0)
            // ═══════════════════════════════════════════════════
            int[,] map2Data = new int[,]
            {
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Z  0
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  1
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  2
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  3
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Z  4: Wand oben Raetsel
                { 1,0,0,0,0,4,4,0,0,0,0,0,0,0,0,1 }, // Z  5: Wasser Sp.5+6
                { 1,0,0,0,0,5,5,0,0,0,0,0,0,0,0,1 }, // Z  6: Bloecke Sp.5+6
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  7: Spieler schiebt von hier
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Z  8: Wand unten Raetsel
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  9
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 10
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 11
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 12
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 13: West-Ausgang
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 14: West-Ausgang
                { 3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3 }, // Z 15
            };

            // ═══════════════════════════════════════════════════
            // MAP 3 – NORDEN
            // Raetsel: 2 Bloecke (Z.8, Sp.6 und 7), Wasser (Z.7, Sp.6 und 7)
            // Loesung:  Beide Bloecke nach NORDEN schieben
            // Raetsel-Bereich: untere Mitte (nahe Sued-Ausgang)
            // Ausgang: Sued (Z.15, Sp.7-8)
            // ═══════════════════════════════════════════════════
            int[,] map3Data = new int[,]
            {
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Z  0
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  1
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  2
                { 1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1 }, // Z  3: Deko-Baeume
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  4
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  5
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Z  6: Wand oben Raetsel
                { 1,0,0,0,0,0,4,4,0,0,0,0,0,0,0,1 }, // Z  7: Wasser Sp.6+7
                { 1,0,0,0,0,0,5,5,0,0,0,0,0,0,0,1 }, // Z  8: Bloecke Sp.6+7
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  9: Spieler schiebt von hier
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Z 10: Wand unten Raetsel
                { 1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1 }, // Z 11: Deko-Baeume
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 12
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 13
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 14
                { 1,3,3,3,3,3,3,0,0,3,3,3,3,3,3,3 }, // Z 15: Sued-Ausgang Sp.7-8
            };

            // ═══════════════════════════════════════════════════
            // MAP 4 – WESTEN
            // Raetsel: 2 Bloecke (Z.6-7, Sp.6), Wasser (Z.6-7, Sp.5)
            // Loesung:  Beide Bloecke nach WESTEN schieben
            // Sperrt geheimen Westbereich (Sp.0-4): dort kann spaeter Schluessel liegen
            // Ausgang: Ost (Z.6-7, Sp.15)
            // ═══════════════════════════════════════════════════
            int[,] map4Data = new int[,]
            {
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Z  0
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  1
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  2
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  3
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  4
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Z  5: Wand oben Raetsel
                { 0,0,0,0,0,4,5,0,0,0,0,0,0,0,0,0 }, // Z  6: Wasser(5) Block(6)
                { 0,0,0,0,0,4,5,0,0,0,0,0,0,0,0,0 }, // Z  7: Wasser(5) Block(6)
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Z  8: Wand unten Raetsel
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  9
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 10
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 11
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 12
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 13
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 14
                { 3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3 }, // Z 15
            };

            // ═══════════════════════════════════════════════════
            // MAP 5 – SUEDEN (FINAL)
            // Raetsel: 3 Bloecke (Z.4, Sp.6-8), Wasser (Z.3, Sp.6-8)
            // Loesung:  Alle 3 Bloecke nach NORDEN schieben (in beliebiger Reihenfolge)
            //           -> Weg zu Ziel-Tile (Z.1, Sp.7) wird frei
            // Ausgang: Nord (Z.0, Sp.7-8)
            // Ziel (Typ 7): Zeile 1, Spalte 7 (hinter dem Raesel-Kanal)
            // ═══════════════════════════════════════════════════
            int[,] map5Data = new int[,]
            {
                { 1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1 }, // Z  0: Nord-Ausgang Sp.7-8
                { 1,1,1,1,1,1,0,7,0,0,1,1,1,1,1,1 }, // Z  1: ZIEL bei Sp.7 (Typ 7=Gold)
                { 1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1 }, // Z  2: Korridor hinter Raetsel
                { 1,1,1,1,1,1,4,4,4,1,1,1,1,1,1,1 }, // Z  3: Wasser Sp.6-8 (Raetsel-Kanal)
                { 1,1,1,1,1,1,5,5,5,1,1,1,1,1,1,1 }, // Z  4: Bloecke Sp.6-8 (schiebbar)
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  5: Spielerbereich (schiebt von hier)
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  6
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  7
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  8
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z  9
                { 1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1 }, // Z 10: Deko-Baeume
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 11
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 12
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 13
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Z 14
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Z 15: Sued-Wand
            };

            Map map1 = new Map(map1Data, grass, path, tree, water, woodBlock, sunkenBlock, goalTile);
            Map map2 = new Map(map2Data, grass, path, tree, water, woodBlock, sunkenBlock, goalTile);
            Map map3 = new Map(map3Data, grass, path, tree, water, woodBlock, sunkenBlock, goalTile);
            Map map4 = new Map(map4Data, grass, path, tree, water, woodBlock, sunkenBlock, goalTile);
            map5      = new Map(map5Data, grass, path, tree, water, woodBlock, sunkenBlock, goalTile);

            map1.East  = map2; map2.West  = map1;
            map1.North = map3; map3.South = map1;
            map1.West  = map4; map4.East  = map1;
            map1.South = map5; map5.North = map1;

            currentMap = map1;

            Texture2D playerSheet = Content.Load<Texture2D>("Player/Player");
            player = new Player(playerSheet, new Vector2(64, 224));
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            if (!gameWon)
            {
                player.Update(gameTime, currentMap);

                int screenSize = Map.Columns * Map.TileSize; // 512

                if (player.Position.X + 32 > screenSize && currentMap.East != null)
                {
                    currentMap = currentMap.East;
                    player.Position = new Vector2(1, player.Position.Y);
                }
                else if (player.Position.X < 0 && currentMap.West != null)
                {
                    currentMap = currentMap.West;
                    player.Position = new Vector2(screenSize - 33, player.Position.Y);
                }
                else if (player.Position.Y + 32 > screenSize && currentMap.South != null)
                {
                    currentMap = currentMap.South;
                    player.Position = new Vector2(player.Position.X, 1);
                }
                else if (player.Position.Y < 0 && currentMap.North != null)
                {
                    currentMap = currentMap.North;
                    player.Position = new Vector2(player.Position.X, screenSize - 33);
                }

                // Ziel-Erkennung: nur auf Map 5 pruefen
                if (currentMap == map5)
                {
                    // Tile unter der Hitbox-Mitte des Spielers
                    int tileCol = (int)((player.Position.X + 16) / Map.TileSize);
                    int tileRow = (int)((player.Position.Y + 28) / Map.TileSize);
                    if (currentMap.GetTile(tileCol, tileRow) == 7)
                        gameWon = true;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            currentMap.DrawGround(spriteBatch);
            currentMap.DrawTrees(spriteBatch);
            player.Draw(spriteBatch);
            currentMap.DrawTreesUnten(spriteBatch);

            // Gewonnen-Overlay: goldener Bildschirm-Flash
            if (gameWon)
            {
                spriteBatch.Draw(winOverlay,
                    new Rectangle(0, 0, 512, 512),
                    Color.Gold * 0.55f);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}