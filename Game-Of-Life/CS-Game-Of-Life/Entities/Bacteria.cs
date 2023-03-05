using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_Game_Of_Life.Entities {
    internal class Bacteria : Entity {
        public Bacteria(GraphicsDevice _graphicsDevice, Vector2 position) {
            Position = position;
            InitialiseTexture(_graphicsDevice);
            Rectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        }
    }
}
