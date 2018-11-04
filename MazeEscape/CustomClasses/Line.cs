using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MazeEscape.CustomClasses
{
    public class Line : IDisposable
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        private VertexPositionColor[] vertices;

        public Line(Vector3 startPoint, Vector3 endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public Line() { }

        public void DrawLine(BasicEffect basicEffect, GraphicsDevice graphicsDevice)
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();
            vertices = new[] { new VertexPositionColor(startPoint, Color.White), new VertexPositionColor(endPoint, Color.White) };
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }

        public void DrawLine(BasicEffect basicEffect, GraphicsDevice graphicsDevice, Vector3 start, Vector3 end)
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();
            vertices = new[] { new VertexPositionColor(start, Color.White), new VertexPositionColor(end, Color.White) };
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }

        public void DrawLine(BasicEffect basicEffect, GraphicsDevice graphicsDevice, Vector3 start, Vector3 end, Color color)
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();
            vertices = new[] { new VertexPositionColor(start, color), new VertexPositionColor(end, color) };
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }

        public void Dispose()
        {
        }
    }
}
