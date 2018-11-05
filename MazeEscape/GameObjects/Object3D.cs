using MazeEscape.CustomClasses;
using MazeEscape.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MazeEscape.GameObjects
{
    public class Object3D : Collider, IObject3D
    {
        public Vector3 Position { get; set; }
        public bool Visible { get; set; }
        public Model Model { get; set; }

        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public ContentManager Content { get; set; }
        public Camera Camera { get; set; }

        public override BoundingBox ColliderBox => new BoundingBox(Position - new Vector3(1,1,1), Position + new Vector3(1, 1, 1));

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
        public Object3D(ContentManager content, Camera camera, Model model)
        {
            Scale = new Vector3(1, 1, 1);
            Content = content;
            this.Camera = camera;
            Model = model;
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
                    effect.EnableDefaultLighting();
                }
                //mesh.Draw();


            }

            var worldMatrix = Matrix.CreateScale(Scale) * Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateTranslation(position);



            Model.Draw(worldMatrix, Camera.View, Camera.Projection);
        }
    }
}
