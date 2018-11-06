using MazeEscape.CustomClasses;
using MazeEscape.Enums;
using MazeEscape.Interfaces;
using MazeEscape.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeEscape.AI
{
    public class Enemy : Collider, IObject3D
    {
        public Vector3 Position { get; set; }
        public bool Visible { get; set; }
        public Vector3 Rotation { get; set; }
        public Model Model { get; set; }
        public ContentManager Content { get; set; }
        public Vector3 Scale { get; set; }
        public Camera Camera { get; set; }
        public ColliderType Type => ColliderType.Enemy;
        public Line EnemyPlayerLine;
        public override BoundingBox ColliderBox => new BoundingBox(Position - new Vector3(0.9f), Position + new Vector3(0.9f));
        public Vector3 MoveVector { get; set; }
		  public SoundManager SoundManager { get; set; }

		public Enemy(Vector3 Position, Model Model, ContentManager Content, Camera Camera, SoundManager SoundManager)
        {
            this.Position = Position;
            this.Model = Model;
            this.Content = Content;
            this.Visible = true;
            this.Camera = Camera;
            EnemyPlayerLine = new Line(this.Position, Camera.Position);
            MoveVector = new Vector3(0.1f, 0, 0);
			this.SoundManager = SoundManager;

		  }
        public void Move()
        {

        }

        List<Vector3> Directions = new List<Vector3>()
        {
            new Vector3(0.1f, 0, 0),
            new Vector3(-0.1f, 0, 0),
            new Vector3(0, 0, 0.1f),
            new Vector3(0, 0, -0.1f),
        };
        public void Step()
        {
            this.Position += MoveVector;
            var list = Camera.ColliderObjects.Where(a => a.Intersects(ColliderBox)).ToList();
            if (list.Any())
            {
                Random rnd = new Random();
                this.Position -= MoveVector;

                MoveVector = Directions[rnd.Next(0, 4)];
				}
				else
				SoundManager.Play("enemy_step");

		}



        public void Draw()
        {
            DrawAt(Position);
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

        public void Load(string path)
        {

        }
    }
}
