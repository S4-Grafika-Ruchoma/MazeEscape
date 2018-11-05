using MazeEscape.CustomClasses;
using MazeEscape.Enums;
using MazeEscape.Interfaces;
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

		public Enemy(Vector3 Position, Model Model, ContentManager Content, Camera Camera)
		{
			this.Position = Position;
			this.Model = Model;
			this.Content = Content;
			this.Visible = true;
			this.Camera = Camera;
		}
		public void Move()
		{

		}
		public void Step()
		{

		}
		public override BoundingBox ColliderBox => new BoundingBox(Position - new Vector3(0.3f), Position + new Vector3(0.3f));

	

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
