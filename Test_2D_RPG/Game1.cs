using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Test_2D_RPG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Player spieler;
        private Map aktuelleMap;

        private Map map1, map2, map3, map4, map5;
        private bool[] schalterAktiv = new bool[4];
        private bool gewonnen = false;
        private Texture2D gewonnenBild;

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

            Texture2D grass = Content.Load<Texture2D>("Tiles/Grass_Middle");
            Texture2D path  = Content.Load<Texture2D>("Tiles/Path_Middle");
            Texture2D water = Content.Load<Texture2D>("Tiles/Water_Middle");
            Texture2D tree  = Content.Load<Texture2D>("Outdoor decoration/Oak_Tree");

            Texture2D woodBlock   = FarbeAlsTextur(new Color(139,  90,  43));
            Texture2D sunkenBlock = FarbeAlsTextur(new Color( 60, 110, 100));
            Texture2D goalTile    = FarbeAlsTextur(new Color(255, 200,   0));
            Texture2D switchTile  = FarbeAlsTextur(new Color(255, 140,   0));
            Texture2D doorTile    = FarbeAlsTextur(new Color(120,  20,  20));
            Texture2D usedSwitch  = FarbeAlsTextur(new Color( 80,  80,  80));
            gewonnenBild          = FarbeAlsTextur(Color.White);

            //  MAP 1 - Mitte
            int[,] map1Data = new int[,]
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

            //  MAP 2 - Rechts
            int[,] map2Data = new int[,]
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

            //  MAP 3 - Oben
            int[,] map3Data = new int[,]
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

            //  MAP 4 - Links
            int[,] map4Data = new int[,]
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

            //  MAP 5 - Zielmap mit den 4 Toren
            int[,] map5Data = new int[,]
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

            map1 = new Map(map1Data, grass, path, tree, water, woodBlock, sunkenBlock, goalTile, switchTile, doorTile, usedSwitch);
            map2 = new Map(map2Data, grass, path, tree, water, woodBlock, sunkenBlock, goalTile, switchTile, doorTile, usedSwitch);
            map3 = new Map(map3Data, grass, path, tree, water, woodBlock, sunkenBlock, goalTile, switchTile, doorTile, usedSwitch);
            map4 = new Map(map4Data, grass, path, tree, water, woodBlock, sunkenBlock, goalTile, switchTile, doorTile, usedSwitch);
            map5 = new Map(map5Data, grass, path, tree, water, woodBlock, sunkenBlock, goalTile, switchTile, doorTile, usedSwitch);

            map1.East  = map2; map2.West  = map1;
            map1.North = map3; map3.South = map1;
            map1.West  = map4; map4.East  = map1;
            map1.South = map5; map5.North = map1;

            aktuelleMap = map1;

            Texture2D spielerBild = Content.Load<Texture2D>("Player/Player");
            spieler = new Player(spielerBild, new Vector2(7 * 32, 11 * 32));
        }

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

        // prueft ob der Spieler den Rand erreicht hat und wechselt die Karte
        private void KarteWechseln()
        {
            if (spieler.Position.X + 32 > 512 && aktuelleMap.East != null)
            {
                aktuelleMap = aktuelleMap.East;
                spieler.Position = new Vector2(1, spieler.Position.Y);
            }
            else if (spieler.Position.X < 0 && aktuelleMap.West != null)
            {
                aktuelleMap = aktuelleMap.West;
                spieler.Position = new Vector2(479, spieler.Position.Y);
            }
            else if (spieler.Position.Y + 32 > 512 && aktuelleMap.South != null)
            {
                aktuelleMap = aktuelleMap.South;
                spieler.Position = new Vector2(spieler.Position.X, 1);
            }
            else if (spieler.Position.Y < 0 && aktuelleMap.North != null)
            {
                aktuelleMap = aktuelleMap.North;
                spieler.Position = new Vector2(spieler.Position.X, 479);
            }
        }

        // schaut ob der Spieler auf einem Schalter oder dem Ziel steht
        private void SchalterUndZielPruefen()
        {
            int spalte = (int)((spieler.Position.X + 16) / 32);
            int zeile  = (int)((spieler.Position.Y + 28) / 32);
            int kachel = aktuelleMap.GetTile(spalte, zeile);

            if (kachel == 8)
            {
                if      (aktuelleMap == map1 && !schalterAktiv[0]) SchalterAktivieren(0, 3);
                else if (aktuelleMap == map2 && !schalterAktiv[1]) SchalterAktivieren(1, 5);
                else if (aktuelleMap == map3 && !schalterAktiv[2]) SchalterAktivieren(2, 7);
                else if (aktuelleMap == map4 && !schalterAktiv[3]) SchalterAktivieren(3, 9);
            }

            if (aktuelleMap == map5 && kachel == 7)
                gewonnen = true;
        }

        private void SchalterAktivieren(int index, int torZeile)
        {
            schalterAktiv[index] = true;
            int spalte = (int)((spieler.Position.X + 16) / 32);
            int zeile  = (int)((spieler.Position.Y + 28) / 32);
            aktuelleMap.SetTile(spalte, zeile, 10);
            map5.SetTile(7, torZeile, 0);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            aktuelleMap.DrawGround(spriteBatch);
            aktuelleMap.DrawTrees(spriteBatch);
            spieler.Draw(spriteBatch);
            aktuelleMap.DrawTreesUnten(spriteBatch);

            if (gewonnen)
                spriteBatch.Draw(gewonnenBild, new Rectangle(0, 0, 512, 512), Color.Gold * 0.55f);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
