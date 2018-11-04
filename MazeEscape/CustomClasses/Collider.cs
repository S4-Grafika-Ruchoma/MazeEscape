using MazeEscape.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MazeEscape.CustomClasses
{
    public abstract class Collider
    {
        public abstract BoundingBox ColliderBox { get; }

        public ColliderType Type { get; set; }

        public void DrawCollider(BasicEffect basicEffect, GraphicsDevice GraphicsDevice)
        {
            using (var line = new Line())
            {
                var n = ColliderBox.Min;
                var x = ColliderBox.Max;

                line.DrawLine(basicEffect, GraphicsDevice, n, new Vector3(n.X, n.Y, x.Z), Color.Red); // N -> N+X.z 1-2
                line.DrawLine(basicEffect, GraphicsDevice, n, new Vector3(n.X, x.Y, n.Z), Color.Red); // N -> N+X.y 1-3
                line.DrawLine(basicEffect, GraphicsDevice, n, new Vector3(x.X, n.Y, n.Z), Color.Red); // N -> N+X.z 1-4

                line.DrawLine(basicEffect, GraphicsDevice, x, new Vector3(n.X, x.Y, x.Z), Color.Blue); // X -> X+N.x 5-6
                line.DrawLine(basicEffect, GraphicsDevice, x, new Vector3(x.X, n.Y, x.Z), Color.Blue); // X -> X+N.y 5-7
                line.DrawLine(basicEffect, GraphicsDevice, x, new Vector3(x.X, x.Y, n.Z), Color.Blue); // X -> X+N.z 5-8

                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(x.X, n.Y, n.Z), new Vector3(x.X, n.Y, x.Z),
                    Color.Salmon); // 4-7
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(x.X, n.Y, x.Z), new Vector3(n.X, n.Y, x.Z),
                    Color.Salmon); // 7-2
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(n.X, n.Y, x.Z), new Vector3(n.X, x.Y, x.Z),
                    Color.Salmon); // 2-6

                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(n.X, x.Y, x.Z), new Vector3(n.X, x.Y, n.Z),
                    Color.Cyan); // 6-3
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(n.X, x.Y, n.Z), new Vector3(x.X, x.Y, n.Z),
                    Color.Cyan); // 3-8
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(x.X, x.Y, n.Z), new Vector3(x.X, n.Y, n.Z),
                    Color.Cyan); // 8-4
            }
        }
    }
}
