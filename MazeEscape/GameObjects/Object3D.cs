using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjektTestowy.CustomClasses;
using ProjektTestowy.Interfaces;

namespace MazeEscape.Interfaces
{
    public class Object3D : IObject3D
    {
        public Vector3 Position { get; set; }
        public bool Visible { get; set; }
        public Model Model { get; set; }

        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public ContentManager Content { get; set; }
        public Camera Camera { get; set; }

        public Object3D(ContentManager content, Camera camera, string path = null)
        {
            Scale = new Vector3(1, 1, 1);
            Content = content;
            this.Camera = camera;
            if (!string.IsNullOrEmpty(path))
            {
                Load(path);
            }
        }


        public void Draw()
        {
            DrawAt(Position);
        }

        public void Load(string path)
        {
            Model = Content.Load<Model>(path);
        }

        public void DrawAt(Vector3 position)
        {
            //var transform = new Matrix[Model.Bones.Count];
            //Model.CopyAbsoluteBoneTransformsTo(transform);
            foreach (var mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.CreateTranslation(position);
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;
                    effect.Alpha = 1;
                }
                //mesh.Draw();
            }

            var worldMatrix = Matrix.CreateScale(Scale) * Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateTranslation(position);


            Model.Draw(worldMatrix, Camera.View, Camera.Projection);
        }
    }
}
