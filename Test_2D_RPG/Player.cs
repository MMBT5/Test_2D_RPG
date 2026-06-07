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

        private float pushCooldown = 0f;
        private float pullCooldown = 0f;
        private const float CooldownTime = 0.22f;

        private const int HbOffX = 4, HbOffY = 24, HbW = 20, HbH = 12;

        public Player(Texture2D spriteSheet, Vector2 startPosition)
        {
            this.spriteSheet = spriteSheet;
            Position = startPosition;
        }

        public void Update(GameTime gameTime, Map currentMap)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            speed = 120f;

            if (pushCooldown > 0) pushCooldown -= delta;
            if (pullCooldown > 0) pullCooldown -= delta;

            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.LeftShift)) speed = 220f;
            bool isPulling = keyboard.IsKeyDown(Keys.E);

            Vector2 direction = Vector2.Zero;
            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))    direction.Y -= 1;
            if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))  direction.Y += 1;
            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))  direction.X -= 1;
            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right)) direction.X += 1;
            if (direction != Vector2.Zero) direction.Normalize();

            Vector2 newPosition = Position + direction * speed * delta;

            // Dominante Richtung
            int pushCol = 0, pushRow = 0;
            if      (Math.Abs(direction.X) > Math.Abs(direction.Y) + 0.1f)
                pushCol = direction.X > 0 ? 1 : -1;
            else if (Math.Abs(direction.Y) > Math.Abs(direction.X) + 0.1f)
                pushRow = direction.Y > 0 ? 1 : -1;

            // Tile-Mittelpunkt VOR der Bewegung
            int oldCenterCol = (int)((Position.X + HbOffX + HbW / 2f) / Map.TileSize);
            int oldCenterRow = (int)((Position.Y + HbOffY + HbH / 2f) / Map.TileSize);

            // ── Kollision ──────────────────────────────────────
            int leftCol   = (int)((newPosition.X + HbOffX)           / Map.TileSize);
            int rightCol  = (int)((newPosition.X + HbOffX + HbW - 1) / Map.TileSize);
            int topRow    = (int)((newPosition.Y + HbOffY)           / Map.TileSize);
            int bottomRow = (int)((newPosition.Y + HbOffY + HbH - 1) / Map.TileSize);

            bool blocked = currentMap.IsSolid(leftCol,  topRow)
                        || currentMap.IsSolid(rightCol, topRow)
                        || currentMap.IsSolid(leftCol,  bottomRow)
                        || currentMap.IsSolid(rightCol, bottomRow);

            // ── SCHIEBEN ───────────────────────────────────────
            if (blocked && !isPulling && pushCooldown <= 0 && (pushCol != 0 || pushRow != 0))
            {
                int hitCenterCol = (int)((Position.X + HbOffX + HbW / 2f) / Map.TileSize);
                int hitCenterRow = (int)((Position.Y + HbOffY + HbH / 2f) / Map.TileSize);

                if (currentMap.TryPushBlock(hitCenterCol + pushCol, hitCenterRow + pushRow, pushCol, pushRow))
                {
                    pushCooldown = CooldownTime;
                    blocked = currentMap.IsSolid(leftCol,  topRow)
                           || currentMap.IsSolid(rightCol, topRow)
                           || currentMap.IsSolid(leftCol,  bottomRow)
                           || currentMap.IsSolid(rightCol, bottomRow);
                }
            }

            // ── BEWEGEN ────────────────────────────────────────
            if (!blocked) Position = newPosition;

            // ── ZIEHEN (E-Taste) ───────────────────────────────
            // Verwendet Map.MoveBlock() statt roher SetTile-Aufrufe, damit
            // das underlayer-System den Tile-Typ unter dem Block korrekt
            // speichert und wiederherstellt (Weg-/Versunken-Tiles bleiben erhalten).
            if (!blocked && isPulling && pullCooldown <= 0 && (pushCol != 0 || pushRow != 0))
            {
                int newCenterCol = (int)((Position.X + HbOffX + HbW / 2f) / Map.TileSize);
                int newCenterRow = (int)((Position.Y + HbOffY + HbH / 2f) / Map.TileSize);

                bool crossedCol = pushCol != 0 && newCenterCol != oldCenterCol;
                bool crossedRow = pushRow != 0 && newCenterRow != oldCenterRow;

                if (crossedCol)
                {
                    int behindCol = oldCenterCol - pushCol;
                    if (currentMap.GetTile(behindCol, oldCenterRow) == 5)
                    {
                        // Snap-Fix: Spieler muss Hitbox-Kante aus oldCenterCol herausbewegen
                        if (pushCol > 0)
                        {
                            float safeX = (oldCenterCol + 1) * Map.TileSize - HbOffX;
                            if (Position.X < safeX) Position = new Vector2(safeX, Position.Y);
                        }
                        else
                        {
                            float safeX = oldCenterCol * Map.TileSize - HbOffX - HbW;
                            if (Position.X > safeX) Position = new Vector2(safeX, Position.Y);
                        }

                        // MoveBlock erhält und restauriert den Tile-Typ unter dem Block
                        currentMap.MoveBlock(behindCol, oldCenterRow, oldCenterCol, oldCenterRow);
                        pullCooldown = CooldownTime;
                    }
                }
                else if (crossedRow)
                {
                    int behindRow = oldCenterRow - pushRow;
                    if (currentMap.GetTile(oldCenterCol, behindRow) == 5)
                    {
                        if (pushRow > 0)
                        {
                            float safeY = (oldCenterRow + 1) * Map.TileSize - HbOffY;
                            if (Position.Y < safeY) Position = new Vector2(Position.X, safeY);
                        }
                        else
                        {
                            float safeY = oldCenterRow * Map.TileSize - HbOffY - HbH;
                            if (Position.Y > safeY) Position = new Vector2(Position.X, safeY);
                        }

                        currentMap.MoveBlock(oldCenterCol, behindRow, oldCenterCol, oldCenterRow);
                        pullCooldown = CooldownTime;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle dest = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
            spriteBatch.Draw(spriteSheet, dest, sourceRect, Color.White);
        }
    }
}
