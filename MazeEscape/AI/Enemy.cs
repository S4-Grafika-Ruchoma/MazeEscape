using MazeEscape.CustomClasses;
using MazeEscape.Interfaces;
using MazeEscape.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using MazeEscape.Enums;

namespace MazeEscape.AI
{
    public class Enemy : Collider, IObject3D
    {
        public Effect lighting { get; set; }
        public Vector3 Position { get; set; }
        public bool Visible { get; set; }
        public Vector3 Rotation { get; set; }
        public Model Model { get; set; }
        public ContentManager Content { get; set; }
        public Vector3 Scale { get; set; }
        public Camera Camera { get; set; }
        public Line EnemyPlayerLine;
        public override BoundingBox ColliderBox => new BoundingBox(Position - new Vector3(0.9f), Position + new Vector3(0.9f));
        public override ColliderType Type { get; set; }
        public Vector3 MoveVector { get; set; }
        public SoundManager SoundManager { get; set; }
		  public bool IsOnPlayerPath { get; set; }
        public float DetectionDistance => Camera.Falshlight ? 15f : 9f;
        public BoundingBox FrontCollider => new BoundingBox(Position - new Vector3(DetectionDistance, 0.2f, 0.2f), Position + new Vector3(DetectionDistance, 0.2f, 0.2f));
        public BoundingBox BackCollider => new BoundingBox(Position - new Vector3(0.2f, 0.2f, DetectionDistance), Position + new Vector3(0.2f, 0.2f, DetectionDistance));

        public int timer = 0;

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
			IsOnPlayerPath = false;
        }

        public int stepCount = 0;

        float z = 0, x = 0, rotSpeed = 0.09f;

        public void Step(Pathfinding pathFinder, GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.Milliseconds;
            if (timer > 1000)
            {
                timer = 0;
            }

            if (Math.Abs(timer % 2) < 0.5f)
            {
                stepCount++;
            }
            if (stepCount >= pathFinder.PositionList.Count - 1)
            {
                stepCount = 0;
            }

            if (pathFinder.PositionList.Any())
            {
                var rot = pathFinder.PositionList[stepCount + 1] - pathFinder.PositionList[stepCount];

                if (rot.X > 0)
                {
                    z -= rotSpeed;
                }
                else if (rot.X < 0)
                {
                    z += rotSpeed;
                }
                else
                {
                    z = 0;
                }

                if (rot.Y > 0)
                {
                    x += rotSpeed;
                }
                else if (rot.Y < 0)
                {
                    x -= rotSpeed;
                }
                else
                {
                    x = 0;
                }
            }

            if (FrontCollider.Intersects(Camera.ColliderBox) || BackCollider.Intersects(Camera.ColliderBox))
            {
                //pathFinder.LastSeenPlayer =
                //    new Spot(new Point((int)Camera.Position.X / 2, (int)Camera.Position.Z / 2), ColliderType.Enemy);

				if (!IsOnPlayerPath)
				{
					//pathFinder.CreatePath(new Spot(new Point((int)Position.X / 2, (int)Position.Z / 2), ColliderType.Enemy));
					//stepCount = 0;			
				if (stepCount > pathFinder.StepsToPlayer)
				{
					pathFinder.LastSeenPlayer =
						  new Spot(new Point((int)Camera.Position.X / 2, (int)Camera.Position.Z / 2), ColliderType.Enemy);

					pathFinder.CreatePath(new Spot(new Point((int)Position.X / 2, (int)Position.Z / 2), ColliderType.Enemy));
					stepCount = 0;
				}
				}
				//	IsOnPlayerPath = true;
				//	if (IsOnPlayerPath && stepCount == pathFinder.PositionList.Count - 1)
				//			IsOnPlayerPath = false;
			}
			else
			{
				pathFinder.LastSeenPlayer = null;
			}


            Rotation = new Vector3(x, 0, z);

            if (pathFinder.PositionList.Any())
            {
                Position = new Vector3(pathFinder.PositionList[stepCount].X * 2, 1,
                    pathFinder.PositionList[stepCount].Y * 2);
            }
        }

        public void Draw()
        {
            DrawAt(Position);
        }

        public void DrawAt(Vector3 position)
        {
            //var transform = new MapTile[Model.Bones.Count];
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
