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

        private Map map1, map2, map3, map4, map5;
        private bool[] switchActivated = new bool[4];
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

            Texture2D grass = Content.Load<Texture2D>("Tiles/Grass_Middle");
            Texture2D path  = Content.Load<Texture2D>("Tiles/Path_Middle");
            Texture2D water = Content.Load<Texture2D>("Tiles/Water_Middle");
            Texture2D tree  = Content.Load<Texture2D>("Outdoor decoration/Oak_Tree");

            Texture2D woodBlock   = MakePixel(new Color(139,  90,  43));
            Texture2D sunkenBlock = MakePixel(new Color( 60, 110, 100));
            Texture2D goalTile    = MakePixel(new Color(255, 200,   0));
            Texture2D switchTile  = MakePixel(new Color(255, 140,   0));
            Texture2D doorTile    = MakePixel(new Color(120,  20,  20));
            Texture2D usedSwitch  = MakePixel(new Color( 80,  80,  80));
            winOverlay            = MakePixel(Color.White);

            //  MAP 1 – Mitte
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

            //  MAP 2 – Rechts
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

            //  MAP 3 – Oben
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

            //  MAP 4 – Links
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

            //  MAP 5 – SUEDEN / ZIEL  (unveraendert)
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

            currentMap = map1;

            Texture2D playerSheet = Content.Load<Texture2D>("Player/Player");
            player = new Player(playerSheet, new Vector2(7 * 32, 11 * 32));
        }

        private Texture2D MakePixel(Color color)
        {
            Texture2D t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData(new Color[] { color });
            return t;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            if (!gameWon)
            {
                player.Update(gameTime, currentMap);
                CheckRoomTransition();
                CheckSwitchAndGoal();
            }

            base.Update(gameTime);
        }

        private void CheckRoomTransition()
        {
            int s = Map.Columns * Map.TileSize; // 512

            if      (player.Position.X + 32 > s && currentMap.East  != null) { currentMap = currentMap.East;  player.Position = new Vector2(1,   player.Position.Y); }
            else if (player.Position.X < 0        && currentMap.West  != null) { currentMap = currentMap.West;  player.Position = new Vector2(s-33, player.Position.Y); }
            else if (player.Position.Y + 32 > s && currentMap.South != null) { currentMap = currentMap.South; player.Position = new Vector2(player.Position.X, 1);   }
            else if (player.Position.Y < 0        && currentMap.North != null) { currentMap = currentMap.North; player.Position = new Vector2(player.Position.X, s-33); }
        }

        private void CheckSwitchAndGoal()
        {
            int col  = (int)((player.Position.X + 16) / Map.TileSize);
            int row  = (int)((player.Position.Y + 28) / Map.TileSize);
            int tile = currentMap.GetTile(col, row);

            if (tile == 8)
            {
                if      (currentMap == map1 && !switchActivated[0]) ActivateSwitch(0, 3);
                else if (currentMap == map2 && !switchActivated[1]) ActivateSwitch(1, 5);
                else if (currentMap == map3 && !switchActivated[2]) ActivateSwitch(2, 7);
                else if (currentMap == map4 && !switchActivated[3]) ActivateSwitch(3, 9);
            }

            if (currentMap == map5 && tile == 7) gameWon = true;
        }

        private void ActivateSwitch(int index, int gateRow)
        {
            switchActivated[index] = true;
            int col = (int)((player.Position.X + 16) / Map.TileSize);
            int row = (int)((player.Position.Y + 28) / Map.TileSize);
            currentMap.SetTile(col, row, 10);
            map5.SetTile(7, gateRow, 0);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            currentMap.DrawGround(spriteBatch);
            currentMap.DrawTrees(spriteBatch);
            player.Draw(spriteBatch);
            currentMap.DrawTreesUnten(spriteBatch);

            if (gameWon)
                spriteBatch.Draw(winOverlay, new Rectangle(0, 0, 512, 512), Color.Gold * 0.55f);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}