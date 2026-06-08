using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Test_2D_RPG
{
    public class Player
    {
        private Texture2D spielerBild;
        public Vector2 Position;
        private float geschwindigkeit = 120f;

        // welcher Frame vom Spritesheet wird gezeichnet
        private Rectangle quellRechteck = new Rectangle(0, 0, 30, 30);

        private float schiebeWartezeit = 0f;
        private float ziehWartezeit    = 0f;
        private const float Wartezeit  = 0.22f;

        // Hitbox Werte (Offset und Größe)
        private const int HbOffX = 4, HbOffY = 24, HbW = 20, HbH = 12;

        public Player(Texture2D bild, Vector2 startPosition)
        {
            spielerBild = bild;
            Position    = startPosition;
        }

        public void Update(GameTime gameTime, Map aktuelleMap)
        {
            float deltaZeit = (float)gameTime.ElapsedGameTime.TotalSeconds;
            geschwindigkeit = 120f;

            if (schiebeWartezeit > 0) schiebeWartezeit -= deltaZeit;
            if (ziehWartezeit    > 0) ziehWartezeit    -= deltaZeit;

            KeyboardState tastatur = Keyboard.GetState();
            if (tastatur.IsKeyDown(Keys.LeftShift)) geschwindigkeit = 220f;
            bool ziehtGerade = tastatur.IsKeyDown(Keys.E);

            // Eingabe lesen
            Vector2 richtung = Vector2.Zero;
            if (tastatur.IsKeyDown(Keys.W) || tastatur.IsKeyDown(Keys.Up))    richtung.Y -= 1;
            if (tastatur.IsKeyDown(Keys.S) || tastatur.IsKeyDown(Keys.Down))  richtung.Y += 1;
            if (tastatur.IsKeyDown(Keys.A) || tastatur.IsKeyDown(Keys.Left))  richtung.X -= 1;
            if (tastatur.IsKeyDown(Keys.D) || tastatur.IsKeyDown(Keys.Right)) richtung.X += 1;
            if (richtung != Vector2.Zero) richtung.Normalize();

            Vector2 neuePosition = Position + richtung * geschwindigkeit * deltaZeit;

            // dominante Bewegungsrichtung bestimmen (fuer schieben/ziehen)
            int schiebeX = 0, schiebeY = 0;
            if      (Math.Abs(richtung.X) > Math.Abs(richtung.Y) + 0.1f)
                schiebeX = richtung.X > 0 ? 1 : -1;
            else if (Math.Abs(richtung.Y) > Math.Abs(richtung.X) + 0.1f)
                schiebeY = richtung.Y > 0 ? 1 : -1;

            // alte Kachelposition merken (wird fuer ziehen gebraucht)
            int alteSpalte = (int)((Position.X + HbOffX + HbW / 2f) / Map.TileSize);
            int alteZeile  = (int)((Position.Y + HbOffY + HbH / 2f) / Map.TileSize);

            // Kollsionsprüfung-----------------------------------
            int linkeSpalte  = (int)((neuePosition.X + HbOffX)           / Map.TileSize);
            int rechteSpalte = (int)((neuePosition.X + HbOffX + HbW - 1) / Map.TileSize);
            int obereZeile   = (int)((neuePosition.Y + HbOffY)           / Map.TileSize);
            int untereZeile  = (int)((neuePosition.Y + HbOffY + HbH - 1) / Map.TileSize);

            bool blockiert = aktuelleMap.IsSolid(linkeSpalte,  obereZeile)
                          || aktuelleMap.IsSolid(rechteSpalte, obereZeile)
                          || aktuelleMap.IsSolid(linkeSpalte,  untereZeile)
                          || aktuelleMap.IsSolid(rechteSpalte, untereZeile);

            // Schieben-------------------------------------------
            if (blockiert && !ziehtGerade && schiebeWartezeit <= 0 && (schiebeX != 0 || schiebeY != 0))
            {
                int mitteX = (int)((Position.X + HbOffX + HbW / 2f) / Map.TileSize);
                int mitteY = (int)((Position.Y + HbOffY + HbH / 2f) / Map.TileSize);

                if (aktuelleMap.TryPushBlock(mitteX + schiebeX, mitteY + schiebeY, schiebeX, schiebeY))
                {
                    schiebeWartezeit = Wartezeit;
                    // Kollision nochmal prüfen nach dem schieben
                    blockiert = aktuelleMap.IsSolid(linkeSpalte,  obereZeile)
                             || aktuelleMap.IsSolid(rechteSpalte, obereZeile)
                             || aktuelleMap.IsSolid(linkeSpalte,  untereZeile)
                             || aktuelleMap.IsSolid(rechteSpalte, untereZeile);
                }
            }

            // Bewegen--------------------------------------------
            if (!blockiert) Position = neuePosition;

            // Ziehen (E gedrückt halten)-------------------------
            if (!blockiert && ziehtGerade && ziehWartezeit <= 0 && (schiebeX != 0 || schiebeY != 0))
            {
                int neueSpalte = (int)((Position.X + HbOffX + HbW / 2f) / Map.TileSize);
                int neueZeile  = (int)((Position.Y + HbOffY + HbH / 2f) / Map.TileSize);

                bool kachelWechselX = schiebeX != 0 && neueSpalte != alteSpalte;
                bool kachelWechselY = schiebeY != 0 && neueZeile  != alteZeile;

                if (kachelWechselX)
                {
                    int hinterSpalte = alteSpalte - schiebeX;
                    if (aktuelleMap.GetTile(hinterSpalte, alteZeile) == 5)
                    {
                        // Spieler wegschnappen damit keine Kollision entsteht
                        if (schiebeX > 0)
                        {
                            float sicherX = (alteSpalte + 1) * Map.TileSize - HbOffX;
                            if (Position.X < sicherX) Position = new Vector2(sicherX, Position.Y);
                        }
                        else
                        {
                            float sicherX = alteSpalte * Map.TileSize - HbOffX - HbW;
                            if (Position.X > sicherX) Position = new Vector2(sicherX, Position.Y);
                        }

                        aktuelleMap.MoveBlock(hinterSpalte, alteZeile, alteSpalte, alteZeile);
                        ziehWartezeit = Wartezeit;
                    }
                }
                else if (kachelWechselY)
                {
                    int hinterZeile = alteZeile - schiebeY;
                    if (aktuelleMap.GetTile(alteSpalte, hinterZeile) == 5)
                    {
                        if (schiebeY > 0)
                        {
                            float sicherY = (alteZeile + 1) * Map.TileSize - HbOffY;
                            if (Position.Y < sicherY) Position = new Vector2(Position.X, sicherY);
                        }
                        else
                        {
                            float sicherY = alteZeile * Map.TileSize - HbOffY - HbH;
                            if (Position.Y > sicherY) Position = new Vector2(Position.X, sicherY);
                        }

                        aktuelleMap.MoveBlock(alteSpalte, hinterZeile, alteSpalte, alteZeile);
                        ziehWartezeit = Wartezeit;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle zielBereich = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
            spriteBatch.Draw(spielerBild, zielBereich, quellRechteck, Color.White);
        }
    }
}
