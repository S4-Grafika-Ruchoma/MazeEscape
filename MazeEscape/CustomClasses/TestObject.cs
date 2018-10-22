using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjektTestowy.Interfaces;

namespace ProjektTestowy.CustomClasses
{
    public class TestObject : IObject3D
    {
        public Vector3 Position { get; set; }
        public bool Visible { get; set; }
        public Vector3 Rotation { get; set; }
        public Model Model { get; set; }

        public VertexPositionColor[] triangleVertices { get; set; }
        public VertexBuffer vertexBuffer { get; set; }

        public void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
