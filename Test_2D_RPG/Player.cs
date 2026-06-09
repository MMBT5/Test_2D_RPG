using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Test_2D_RPG
{
    public class Player
    {
        private Texture2D spielerBild;
        public Vector2 Position;

        private Rectangle quellRechteck = new Rectangle(0, 0, 30, 30);

        private float schiebeWartezeit = 0f;
        private float ziehWartezeit = 0f;

        public Player(Texture2D bild, Vector2 startPosition)
        {
            spielerBild = bild;
            Position = startPosition;
        }

        public void Update(GameTime gameTime, Map aktuelleMap)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (schiebeWartezeit > 0) schiebeWartezeit -= dt;
            if (ziehWartezeit > 0) ziehWartezeit -= dt;

            KeyboardState kb = Keyboard.GetState();
            bool zieht = kb.IsKeyDown(Keys.E);

            // Geschwindigkeit, Sprint mit Shift
            float speed = 120f;
            if (kb.IsKeyDown(Keys.LeftShift)) speed = 220f;

            // Bewegungsrichtung einlesen
            Vector2 dir = Vector2.Zero;
            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up))    dir.Y = -1;
            if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down))  dir.Y =  1;
            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))  dir.X = -1;
            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) dir.X =  1;
            if (dir != Vector2.Zero) dir.Normalize();

            Vector2 neuePos = Position + dir * speed * dt;

            // Schieberichtung bestimmen
            int schiebeX = 0;
            int schiebeY = 0;
            if      (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) schiebeX =  1;
            else if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))  schiebeX = -1;
            else if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down))  schiebeY =  1;
            else if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up))    schiebeY = -1;

            // Kachelposition vor der Bewegung merken fürs ziehen
            int alteSpalte = (int)((Position.X + 14) / 32);
            int alteZeile  = (int)((Position.Y + 30) / 32);

            // Kollision prüfen - 4 Ecken der Hitbox
            // Hitbox ist kleiner als der Sprite damit man zwischen Baeumen durchpasst
            // Hit boc nur der Untere Teil des Sprite
            int links  = (int)((neuePos.X + 4)  / 32);
            int rechts = (int)((neuePos.X + 23) / 32);
            int oben   = (int)((neuePos.Y + 24) / 32);
            int unten  = (int)((neuePos.Y + 35) / 32);

            bool blockiert = aktuelleMap.IsSolid(links, oben)
                          || aktuelleMap.IsSolid(rechts, oben)
                          || aktuelleMap.IsSolid(links, unten)
                          || aktuelleMap.IsSolid(rechts, unten);

            // Schieben-----------------------------------------------
            if (blockiert && !zieht && schiebeWartezeit <= 0 && (schiebeX != 0 || schiebeY != 0))
            {
                int mX = (int)((Position.X + 14) / 32);
                int mY = (int)((Position.Y + 30) / 32);

                if (aktuelleMap.TryPushBlock(mX + schiebeX, mY + schiebeY, schiebeX, schiebeY))
                {
                    schiebeWartezeit = 0.22f;

                    // nach dem Schieben nochmal pruefen ob der Weg jetzt frei ist
                    blockiert = aktuelleMap.IsSolid(links, oben)
                             || aktuelleMap.IsSolid(rechts, oben)
                             || aktuelleMap.IsSolid(links, unten)
                             || aktuelleMap.IsSolid(rechts, unten);
                }
            }

            // Bewegen------------------------------------------------
            if (!blockiert)
                Position = neuePos;

            // Ziehen (E halten + bewegen)----------------------------
            if (!blockiert && zieht && ziehWartezeit <= 0 && (schiebeX != 0 || schiebeY != 0))
            {
                int neueSpalte = (int)((Position.X + 14) / 32);
                int neueZeile  = (int)((Position.Y + 30) / 32);

                bool wechselX = schiebeX != 0 && neueSpalte != alteSpalte;
                bool wechselY = schiebeY != 0 && neueZeile  != alteZeile;

                if (wechselX)
                {
                    int hinterSpalte = alteSpalte - schiebeX;

                    if (aktuelleMap.GetTile(hinterSpalte, alteZeile) == 5)
                    {
                        // Spieler minimal wegbewegen damit die Hitbox nicht mit Block überlappt
                        if (schiebeX > 0 && Position.X < (alteSpalte + 1) * 32 - 4)
                            Position = new Vector2((alteSpalte + 1) * 32 - 4, Position.Y);
                        if (schiebeX < 0 && Position.X > alteSpalte * 32 - 24)
                            Position = new Vector2(alteSpalte * 32 - 24, Position.Y);

                        aktuelleMap.MoveBlock(hinterSpalte, alteZeile, alteSpalte, alteZeile);
                        ziehWartezeit = 0.22f;
                    }
                }
                else if (wechselY)
                {
                    int hinterZeile = alteZeile - schiebeY;

                    if (aktuelleMap.GetTile(alteSpalte, hinterZeile) == 5)
                    {
                        if (schiebeY > 0 && Position.Y < (alteZeile + 1) * 32 - 24)
                            Position = new Vector2(Position.X, (alteZeile + 1) * 32 - 24);
                        if (schiebeY < 0 && Position.Y > alteZeile * 32 - 36)
                            Position = new Vector2(Position.X, alteZeile * 32 - 36);

                        aktuelleMap.MoveBlock(alteSpalte, hinterZeile, alteSpalte, alteZeile);
                        ziehWartezeit = 0.22f;
                    }
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle bereich = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
            spriteBatch.Draw(spielerBild, bereich, quellRechteck, Color.White);
        }
    }
}
