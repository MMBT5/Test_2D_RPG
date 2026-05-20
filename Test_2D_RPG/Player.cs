using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Test_2D_RPG
{
    public class Player
    {
        private Texture2D spriteSheet;
        public Vector2 Position;
        private float speed = 120f;

        private Rectangle sourceRect = new Rectangle(0, 0, 30, 30);

        // Verhindert dass der Block jedes Frame weitergeschoben wird
        private float pushCooldown = 0f;
        private const float PushCooldownTime = 0.25f;

        public Player(Texture2D spriteSheet, Vector2 startPosition)
        {
            this.spriteSheet = spriteSheet;
            Position = startPosition;
        }

        public void Update(GameTime gameTime, Map currentMap)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            speed = 120f;

            if (pushCooldown > 0)
                pushCooldown -= delta;

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.LeftShift)) speed = 240f;

            Vector2 direction = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up)) direction.Y -= 1;
            if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down)) direction.Y += 1;
            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left)) direction.X -= 1;
            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right)) direction.X += 1;

            if (direction != Vector2.Zero)
                direction.Normalize();

            Vector2 newPosition = Position + direction * speed * delta;

            // Hitbox: untere Hälfte des Sprites
            int hitboxOffsetX = 4;
            int hitboxOffsetY = 24;
            int hitboxWidth = 20;
            int hitboxHeight = 12;

            int leftCol = (int)((newPosition.X + hitboxOffsetX) / Map.TileSize);
            int rightCol = (int)((newPosition.X + hitboxOffsetX + hitboxWidth - 1) / Map.TileSize);
            int topRow = (int)((newPosition.Y + hitboxOffsetY) / Map.TileSize);
            int bottomRow = (int)((newPosition.Y + hitboxOffsetY + hitboxHeight - 1) / Map.TileSize);

            bool blocked = currentMap.IsSolid(leftCol, topRow)
                        || currentMap.IsSolid(rightCol, topRow)
                        || currentMap.IsSolid(leftCol, bottomRow)
                        || currentMap.IsSolid(rightCol, bottomRow);

            // ── Schiebe-Logik ────────────────────────────────────────
            if (blocked && pushCooldown <= 0 && direction != Vector2.Zero)
            {
                // Schieberichtung: nur in die dominante Richtung schieben
                int pushCol = 0, pushRow = 0;

                if (Math.Abs(direction.X) > Math.Abs(direction.Y) + 0.1f)
                    pushCol = direction.X > 0 ? 1 : -1;
                else if (Math.Abs(direction.Y) > Math.Abs(direction.X) + 0.1f)
                    pushRow = direction.Y > 0 ? 1 : -1;

                if (pushCol != 0 || pushRow != 0)
                {
                    // Mitte der Hitbox (aktuelle Position) + 1 Tile in Schieberichtung
                    int hitCenterCol = (int)((Position.X + hitboxOffsetX + hitboxWidth / 2f) / Map.TileSize);
                    int hitCenterRow = (int)((Position.Y + hitboxOffsetY + hitboxHeight / 2f) / Map.TileSize);

                    int blockCol = hitCenterCol + pushCol;
                    int blockRow = hitCenterRow + pushRow;

                    if (currentMap.TryPushBlock(blockCol, blockRow, pushCol, pushRow))
                    {
                        pushCooldown = PushCooldownTime;

                        // Kollision nach dem Push neu berechnen
                        blocked = currentMap.IsSolid(leftCol, topRow)
                               || currentMap.IsSolid(rightCol, topRow)
                               || currentMap.IsSolid(leftCol, bottomRow)
                               || currentMap.IsSolid(rightCol, bottomRow);
                    }
                }
            }
            // ─────────────────────────────────────────────────────────

            if (!blocked)
                Position = newPosition;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle dest = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
            spriteBatch.Draw(spriteSheet, dest, sourceRect, Color.White);
        }
    }
}