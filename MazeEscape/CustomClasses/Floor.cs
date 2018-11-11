﻿using System;
using MazeEscape.Enums;
using MazeEscape.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MazeEscape.CustomClasses
{
    public class Floor : Collider, IObject3D
    {
        public Effect lighting { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }

        public Vector3 Position { get; set; }
        public bool Visible { get; set; }
        public Model Model { get; set; }

        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public ContentManager Content { get; set; }
        public Camera Camera { get; set; }

        public override BoundingBox ColliderBox => new BoundingBox(new Vector3(0,-1,0), new Vector3(80,0,80));
        public override ColliderType Type { get; set; }


        public Floor(ContentManager content, Camera camera, string path = null)
        {
            Scale = new Vector3(1, 1, 1);
            Content = content;
            this.Camera = camera;
            if (!string.IsNullOrEmpty(path))
            {
                Load(path);
            }
        }
        public Floor(ContentManager content, Camera camera, Model model)
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
            var transform = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transform);

            foreach (var mesh in Model.Meshes)
            {
                try
                {
                    var currentTexture = ((BasicEffect)(mesh.Effects[0])).Texture;
                    lighting.Parameters["DiffuseTexture"].SetValue(currentTexture);

                    var worldMatrix = Matrix.CreateScale(Scale) * Matrix.CreateRotationX(Rotation.X) *
                                      Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z) * transform[mesh.ParentBone.Index] *
                                      Matrix.CreateTranslation(position);

                    lighting.Parameters["World"].SetValue(worldMatrix);
                    lighting.Parameters["WorldViewProj"].SetValue(worldMatrix * Camera.View * Camera.Projection);


                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.World = worldMatrix;
                        effect.View = Camera.View;
                        effect.Projection = Camera.Projection;
                        effect.Alpha = 1f;

                    }

                    foreach (var meshParts in mesh.MeshParts)
                    {
                        GraphicsDevice.SetVertexBuffer(meshParts.VertexBuffer, meshParts.VertexOffset);
                        GraphicsDevice.Indices = meshParts.IndexBuffer;
                        lighting.CurrentTechnique.Passes[0].Apply();

                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, meshParts.StartIndex,
                            meshParts.PrimitiveCount);

                    }
                    //mesh.Draw();


                }
                catch (Exception ex)
                {
                }

                var worldMatrix2 = Matrix.CreateScale(Scale * 0.9f) * Matrix.CreateRotationX(Rotation.X) *
                                   Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z) *
                                   Matrix.CreateTranslation(position);

                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = worldMatrix2;
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;
                    effect.Alpha = 1f;

                }
            }

            //var worldMatrix3 = Matrix.CreateScale(Scale * 0.9f) * Matrix.CreateRotationX(Rotation.X) *
            //                   Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z) *
            //                   Matrix.CreateTranslation(position);
            //Model.Draw(worldMatrix3, Camera.View, Camera.Projection);
        }
    }
}
