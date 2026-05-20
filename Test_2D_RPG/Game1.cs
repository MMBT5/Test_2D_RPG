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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 512;
            graphics.PreferredBackBufferHeight = 512;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // ── Texturen laden ───────────────────────────────────────
            Texture2D grass = Content.Load<Texture2D>("Tiles/Grass_Middle");
            Texture2D path = Content.Load<Texture2D>("Tiles/Path_Middle");
            Texture2D tree = Content.Load<Texture2D>("Outdoor decoration/Oak_Tree");
            Texture2D water = Content.Load<Texture2D>("Tiles/Water_Middle");

            // Holzblock – Platzhalter (braune Farbe), austauschbar mit echtem Sprite:
            // Content.Load<Texture2D>("Tiles/Wood_Block")
            Texture2D woodBlock = new Texture2D(GraphicsDevice, 1, 1);
            woodBlock.SetData(new Color[] { new Color(139, 90, 43) });

            // Versunkener Block – Platzhalter (blau-grün), austauschbar:
            // Content.Load<Texture2D>("Tiles/Sunken_Block")
            Texture2D sunkenBlock = new Texture2D(GraphicsDevice, 1, 1);
            sunkenBlock.SetData(new Color[] { new Color(60, 110, 100) });

            // ── Map-Daten ────────────────────────────────────────────
            // Legende:
            //   0 = Gras       (betretbar)
            //   1 = Baum       (hinter Spieler, blockiert)
            //   2 = Weg        (betretbar)
            //   3 = Baum       (vor Spieler, blockiert)
            //   4 = Wasser     (blockiert)
            //   5 = Holzblock  (schiebbar, blockiert)
            //   6 = Versunken  (Block im Wasser, betretbar)

            // ── Map 1 – Zentrum ──────────────────────────────────────
            // Ausgänge: Nord (Zeile 0, Spalten 7-8)
            //           Ost  (Zeilen 13-14, Spalte 15)
            //           Süd  (Zeile 15, Spalten 7-8)
            //           West (Zeilen 6-7, Spalte 0)
            int[,] map1Data = new int[,]
            {
                { 1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1 }, // Zeile  0: Nord-Ausgang Sp.7-8
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile  1
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile  2
                { 1,0,0,0,0,0,0,0,2,2,2,2,0,0,0,1 }, // Zeile  3
                { 1,0,0,0,0,0,0,0,2,0,0,0,0,0,0,1 }, // Zeile  4
                { 1,0,0,0,0,0,0,0,2,0,0,0,0,0,0,1 }, // Zeile  5
                { 0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,1 }, // Zeile  6: West-Ausgang Sp.0
                { 0,0,0,0,2,2,2,2,2,0,0,0,0,0,0,1 }, // Zeile  7: West-Ausgang Sp.0
                { 1,0,0,0,0,0,0,0,2,0,0,0,0,0,0,1 }, // Zeile  8
                { 1,0,0,0,0,0,0,0,2,0,0,0,0,0,0,1 }, // Zeile  9
                { 1,0,0,0,0,0,0,0,2,0,0,0,0,0,0,1 }, // Zeile 10
                { 1,0,0,0,0,0,0,0,2,2,2,2,2,2,0,1 }, // Zeile 11
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,2,0,1 }, // Zeile 12
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2 }, // Zeile 13: Ost-Ausgang
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // Zeile 14: Ost-Ausgang
                { 1,3,3,3,3,3,3,0,0,3,3,3,3,3,3,3 }, // Zeile 15: Süd-Ausgang Sp.7-8
            };

            // ── Map 2 – Osten ─────────────────────────────────────────
            // Ausgang: West (Zeilen 13-14, Spalte 0)
            int[,] map2Data = new int[,]
            {
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // West-Ausgang
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // West-Ausgang
                { 3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3 },
            };

            // ── Map 3 – Norden ────────────────────────────────────────
            // Ausgang: Süd (Zeile 15, Spalten 7-8) → verbindet mit Map1 Nord
            int[,] map3Data = new int[,]
            {
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1 }, // Deko-Bäume
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1 }, // Deko-Bäume
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,3,3,3,3,3,3,0,0,3,3,3,3,3,3,3 }, // Zeile 15: Süd-Ausgang Sp.7-8
            };

            // ── Map 4 – Westen (Schiebe-Rätsel) ──────────────────────
            // Ausgang: Ost (Zeilen 6-7, Spalte 15) → verbindet mit Map1 West
            //
            // Rätsel: Wasser bei Spalte 5, Zeilen 6-7 blockiert den Weg.
            // Holzblöcke (5) bei Spalte 6, Zeilen 6-7.
            // Spieler schiebt Blöcke nach links → Block landet im Wasser
            // → wird zu Typ 6 (versunken, betretbar) → Weg zur Westseite frei.
            //
            // Komplette Wand bei Zeile 5 und 8 → kein Umgehen möglich.
            int[,] map4Data = new int[,]
            {
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Zeile  0
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile  1 } geheime
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile  2 } Westseite
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile  3 } (später
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile  4 }  Schlüssel)
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Zeile  5: vollständige Wand oben
                { 0,0,0,0,0,4,5,0,0,0,0,0,0,0,0,0 }, // Zeile  6: GAP – Wasser Sp.5, Block Sp.6
                { 0,0,0,0,0,4,5,0,0,0,0,0,0,0,0,0 }, // Zeile  7: GAP – Wasser Sp.5, Block Sp.6
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Zeile  8: vollständige Wand unten
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile  9
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile 10
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile 11
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile 12
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile 13
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }, // Zeile 14
                { 3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3 }, // Zeile 15
            };

            // ── Map 5 – Süden ─────────────────────────────────────────
            // Ausgang: Nord (Zeile 0, Spalten 7-8) → verbindet mit Map1 Süd
            int[,] map5Data = new int[,]
            {
                { 1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1 }, // Zeile  0: Nord-Ausgang Sp.7-8
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,1 }, // Deko-Bäume
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }, // Süd-Wand geschlossen
            };

            // ── Maps erstellen ───────────────────────────────────────
            Map map1 = new Map(map1Data, grass, path, tree, water, woodBlock, sunkenBlock);
            Map map2 = new Map(map2Data, grass, path, tree, water, woodBlock, sunkenBlock);
            Map map3 = new Map(map3Data, grass, path, tree, water, woodBlock, sunkenBlock);
            Map map4 = new Map(map4Data, grass, path, tree, water, woodBlock, sunkenBlock);
            Map map5 = new Map(map5Data, grass, path, tree, water, woodBlock, sunkenBlock);

            // ── Verbindungen setzen ──────────────────────────────────
            map1.East = map2; map2.West = map1;
            map1.North = map3; map3.South = map1;
            map1.West = map4; map4.East = map1;
            map1.South = map5; map5.North = map1;

            currentMap = map1;

            // ── Spieler ──────────────────────────────────────────────
            Texture2D playerSheet = Content.Load<Texture2D>("Player/Player");
            player = new Player(playerSheet, new Vector2(64, 224)); // Mitte links
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

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

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}