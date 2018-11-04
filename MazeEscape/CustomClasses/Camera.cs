﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeEscape;
using MazeEscape.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjektTestowy.CustomClasses
{
    public class Camera : GameComponent
    {
        private Vector3 cameraPosition;
        public Vector3 cameraRotation;

        public float cameraSpeed { get; set; }
        public Vector3 cameraLookAt;

        private Vector3 mouseRotationBuffer;
        private MouseState currentMouseState;
        private MouseState prevMouseState;

        public BoundingBox Collider => new BoundingBox(cameraPosition - new Vector3(0.3f), cameraPosition + new Vector3(0.3f));

        public bool ShowCenterLine { get; set; }

        public bool AllowClimb { get; set; }

        public Vector3 Position
        {
            get { return cameraPosition; }
            set
            {
                cameraPosition = value;
                UpdateLookAt();
            }
        }

        public Vector3 Rotation
        {
            get { return cameraRotation; }
            set
            {
                cameraRotation = value;
                UpdateLookAt();
            }
        }

        public Matrix Projection { get; protected set; }

        public Matrix View => Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);

        public Camera(Game game, Vector3 position, Vector3 rotation, float speed) : base(game)
        {
            AllowClimb = AppConfig._DEBUG_AUTO_NO_CLIP_;
            ShowColliders = false;

            cameraSpeed = speed;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Game.GraphicsDevice.Viewport.AspectRatio, 0.05f, 1000);
            MoveTo(position, rotation);

            ShowCenterLine = AppConfig._DEBUG_SHOW_DIRECTION_TO_CENTER_;

            prevMouseState = Mouse.GetState();
        }

        private void MoveTo(Vector3 pos, Vector3 rot)
        {
            Position = pos;
            Rotation = rot;
        }

        private void UpdateLookAt()
        {
            var rotationMartix = Matrix.CreateRotationX(cameraRotation.X) * Matrix.CreateRotationY(cameraRotation.Y);

            var lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMartix);
            cameraLookAt = cameraPosition + lookAtOffset;
        }

        private Vector3 PreviewMove(Vector3 amount)
        {
            var rotate = Matrix.CreateRotationY(cameraRotation.Y);

            var movment = new Vector3(amount.X, amount.Y, amount.Z);
            movment = Vector3.Transform(movment, rotate);

            return cameraPosition + movment;
        }

        public Vector3 cameraAxiesPosition
        {
            get
            {
                var rotate = Matrix.CreateRotationX(cameraRotation.X) * Matrix.CreateRotationY(cameraRotation.Y) * Matrix.CreateRotationZ(cameraRotation.Z);
                var movment = new Vector3(-0.3f, -0.15f, 0.5f);
                movment = Vector3.Transform(movment, rotate);

                return cameraPosition + movment;
            }
        }

        public bool ShowColliders { get; internal set; }
        public List<Object3D> Map { get; set; }

        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }

        public override void Update(GameTime gameTime)
        {
            if (Game.IsActive)
            {
                currentMouseState = Mouse.GetState();
                var keyboardState = Keyboard.GetState();
                var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                var moveVector = Vector3.Zero;

                if (keyboardState.IsKeyDown(Keys.W))
                    moveVector.Z = 1;
                if (keyboardState.IsKeyDown(Keys.S))
                    moveVector.Z = -1;

                if (keyboardState.IsKeyDown(Keys.A))
                    moveVector.X = 1;
                if (keyboardState.IsKeyDown(Keys.D))
                    moveVector.X = -1;

                if (AllowClimb)
                {
                    if (keyboardState.IsKeyDown(Keys.Space))
                        moveVector.Y = 1;
                    if (keyboardState.IsKeyDown(Keys.C))
                        moveVector.Y = -1;
                }

                if (keyboardState.IsKeyDown(Keys.P))
                    ShowCenterLine = !ShowCenterLine;

                if (moveVector != Vector3.Zero)
                {
                    moveVector.Normalize();
                    moveVector *= dt * cameraSpeed;

                    Move(moveVector);
                }


                if (Map.Any(a => a.Collider.Intersects(Collider)))
                {
                    Move(moveVector*-1);
                }

                #region Camera Mouse
                if (currentMouseState != prevMouseState)
                {
                    var deltaX = currentMouseState.X - (Game.GraphicsDevice.Viewport.Width / 2);
                    var deltaY = currentMouseState.Y - (Game.GraphicsDevice.Viewport.Height / 2);

                    mouseRotationBuffer.X -= 0.05F * deltaX * dt;
                    mouseRotationBuffer.Y -= 0.05F * deltaY * dt;

                    if (mouseRotationBuffer.Y < MathHelper.ToRadians(-75))
                        mouseRotationBuffer.Y -= mouseRotationBuffer.Y - MathHelper.ToRadians(-75);

                    if (mouseRotationBuffer.Y > MathHelper.ToRadians(75))
                        mouseRotationBuffer.Y -= mouseRotationBuffer.Y - MathHelper.ToRadians(75);

                    Rotation = new Vector3(
                        -MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-75f), MathHelper.ToRadians(75f)),
                        MathHelper.WrapAngle(mouseRotationBuffer.X),
                        0);
                }

                Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

                prevMouseState = currentMouseState;
                #endregion

                base.Update(gameTime);

            }
        }
    }
}
