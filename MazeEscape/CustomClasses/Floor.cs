using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjektTestowy.CustomClasses
{
    public class Floor
    {
        private int floorWidth;
        private int floorHeight;
        private VertexBuffer floorBuffer;
        private GraphicsDevice device;
        Color[] floorColors = new Color[2] { Color.White, Color.Black };

        public Floor(GraphicsDevice device, int width, int height)
        {
            this.device = device;
            floorWidth = width;
            floorHeight = height;
            BuildFloorBuffer();
        }

        private void BuildFloorBuffer()
        {
            List<VertexPositionColor> vertexList = new List<VertexPositionColor>();
            int counter = 0;

            for (int x = 0; x < floorWidth; x++)
            {
                counter++;
                for (int z = 0; z < floorHeight; z++)
                {
                    counter++;

                    foreach (var vertex in FloorTile(x, z, floorColors[counter % 2]))
                    {
                        vertexList.Add(vertex);
                    }
                }
            }

            floorBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, vertexList.Count, BufferUsage.None);
            floorBuffer.SetData(vertexList.ToArray());
        }

        private List<VertexPositionColor> FloorTile(int xOffset, int zOffset, Color tileColor)
        {
            var vList = new List<VertexPositionColor>()
            {
                new VertexPositionColor(new Vector3(0 + xOffset, 0, 0 + zOffset), tileColor),
                new VertexPositionColor(new Vector3(1 + xOffset, 0, 0 + zOffset), tileColor),
                new VertexPositionColor(new Vector3(0 + xOffset, 0, 1 + zOffset), tileColor),
                new VertexPositionColor(new Vector3(1 + xOffset, 0, 0 + zOffset), tileColor),
                new VertexPositionColor(new Vector3(1 + xOffset, 0, 1 + zOffset), tileColor),
                new VertexPositionColor(new Vector3(0 + xOffset, 0, 1 + zOffset), tileColor),
            };

            return vList;
        }

        public void Draw(Camera camera, BasicEffect effect)
        {
            effect.VertexColorEnabled = true;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.World = Matrix.Identity;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(floorBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, floorBuffer.VertexCount / 3);
            }
        }

    }
}
