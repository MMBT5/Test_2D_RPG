using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Test_2D_RPG
{
    public class Player
    {
        private Texture2D _spriteSheet;
        private Vector2 _position;
        private float _speed = 150f;

        // Der Ausschnitt aus dem Spritesheet (erster Frame)
        private Rectangle _sourceRect = new Rectangle(0, 0, 32, 32);

        public Player(Texture2D spriteSheet, Vector2 startPosition)
        {
            _spriteSheet = spriteSheet;
            _position = startPosition;
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboard = Keyboard.GetState();

            Vector2 direction = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up)) direction.Y -= 1;
            if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down)) direction.Y += 1;
            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left)) direction.X -= 1;
            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right)) direction.X += 1;

            if (direction != Vector2.Zero)
                direction.Normalize();

            _position += direction * _speed * delta;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle destination = new Rectangle((int)_position.X, (int)_position.Y, 32, 32);
            spriteBatch.Draw(_spriteSheet, destination, _sourceRect, Color.White);
        }
    }
}