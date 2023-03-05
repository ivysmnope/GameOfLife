using CS_Game_Of_Life.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS_Game_Of_Life.Entities {
    internal class Camera {
        #region attributes
        public float Zoom { get; set; }
        public Vector2 Position { get; private set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; private set; }
        public float Speed { get; private set; }

        public Rectangle Limits { get; private set; }

        public bool CanMove { get; set; } = true;
        #endregion

        #region Constructor
        public Camera(float zoom = 1, Vector2? position = null, float rotation = 0, Vector2? origin = null, float speed = 2, Rectangle? limits = null) {
            Zoom = zoom;
            Position = position ?? Vector2.Zero;
            Rotation = rotation;
            Origin = origin ?? Vector2.Zero;
            Speed = speed;
            Limits = limits ?? new Rectangle(0, 0, 800, 800);
        }
        #endregion

        #region Methods
        public void Move(Vector2 velocity) {
            Position += velocity;
            LimitInBounds();
        }

        public Matrix GetTransform() {
            var translationMatrix = Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0));
            var rotationMatrix = Matrix.CreateRotationZ(Rotation);
            var scaleMatrix = Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));
            var originMatrix = Matrix.CreateTranslation(new Vector3(Origin.X, Origin.Y, 0));

            return translationMatrix * rotationMatrix * scaleMatrix * originMatrix;
        }

        public void LimitInBounds() {
            if (Position.X > Limits.Left) {
                Position = new Vector2(0, Position.Y);
            }
            if (Position.X - Limits.Width / Zoom - Entity.Width / Zoom < -Limits.Right) {
                Position = new Vector2(Position.X + Entity.Width / Zoom, Position.Y);
            }
            if (Position.Y > Limits.Top) {
                Position = new Vector2(Position.X, 0);
            }
            if (Position.Y - Limits.Height / Zoom - Entity.Height / Zoom < -Limits.Bottom) {
                Position = new Vector2(Position.X, Position.Y + Entity.Height / Zoom);
            }
        }
        #endregion
    }
}
