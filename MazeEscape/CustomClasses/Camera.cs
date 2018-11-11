using System.Collections.Generic;
using System.Linq;
using MazeEscape.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeEscape.CustomClasses
{
    public class Camera : GameComponent
    {
        private Vector3 cameraPosition;
        public Vector3 cameraRotation;

        public int Collected { get; set; }

        public BoundingBox ColliderBox => new BoundingBox(cameraPosition - new Vector3(0.3f), cameraPosition + new Vector3(0.3f));
        public ColliderType ColliderType => ColliderType.Camera;


        public BoundingBox GrabCollider => new BoundingBox(cameraPosition - new Vector3(0.2f), cameraPosition + new Vector3(0.2f));
        public bool Falshlight { get; set; }

        public float cameraSpeed { get; set; }
        public Vector3 cameraLookAt;

        private Vector3 mouseRotationBuffer;
        private MouseState currentMouseState;
        private MouseState prevMouseState;

        public Matrix World { get; set; }

        public bool ShowLines { get; set; }

        public bool NoClip { get; set; }

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

        public Game1 game { get; set; }

        public Camera(Game game, Vector3 position, Vector3 rotation, float speed) : base(game)
        {
            Collected = 0;
            this.game = game as Game1;
            ColliderObjects = new List<Collider>();
            NoClip = AppConfig._DEBUG_AUTO_NO_CLIP_;
            ShowColliders = false;

            cameraSpeed = speed;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Game.GraphicsDevice.Viewport.AspectRatio, 0.05f, 1000);
            MoveTo(position, rotation);

            Falshlight = true;

            ShowLines = AppConfig._DEBUG_SHOW_DIRECTION_TO_CENTER_;

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

        private Vector3 PreviewMove3D(Vector3 amount)
        {
            var rotate = Matrix.CreateRotationX(cameraRotation.X) * Matrix.CreateRotationY(cameraRotation.Y) * Matrix.CreateRotationZ(cameraRotation.Z);

            var movment = new Vector3(amount.X, amount.Y, amount.Z);
            movment = Vector3.Transform(movment, rotate);

            return cameraPosition + movment;
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
        public List<Collider> ColliderObjects { get; set; }
        public List<Collider> CollectablesObjects { get; set; }
        public BoundingBox EndCollider { get; set; }
        public Vector3 NextLevelStartPosition { get; set; }

        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }

        private KeyboardState prevState;

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

                if (NoClip)
                {
                    if (keyboardState.IsKeyDown(Keys.Space))
                        moveVector.Y = 1;
                    if (keyboardState.IsKeyDown(Keys.C))
                        moveVector.Y = -1;
                }
                else
                {
                    // Grawitacja
                    //moveVector.Y = -1.0f;
                }

                if (CollectablesObjects.Any(a => a.ColliderBox.Intersects(GrabCollider)))
                {
                    if (keyboardState.IsKeyDown(Keys.E) && prevState.IsKeyUp(Keys.E))
                    {
                        CollectablesObjects.RemoveAll(a=>a.ColliderBox.Intersects(GrabCollider));
                        Collected++;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.F) && prevState.IsKeyUp(Keys.F))
                    Falshlight = !Falshlight;

                if (keyboardState.IsKeyDown(Keys.P) && prevState.IsKeyUp(Keys.P))
                    ShowLines = !ShowLines;

                if (moveVector != Vector3.Zero)
                {
                    moveVector.Normalize();
                    moveVector *= dt * cameraSpeed;

                    Move(moveVector);
                }


                if (!NoClip)
                {
                    var list = ColliderObjects.Where(a => a.ColliderBox.Intersects(ColliderBox)).ToList();
                    if (list.Any())
                    {
                        if (!IsEndLevelCollision())
                        {
                            Move(moveVector * -1);
                        }

                    }

                    if (IsEnemyCollision())
                    {
                        game.Exit();
                    }
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

                prevState = keyboardState;

                base.Update(gameTime);

            }
        }
        
        public bool IsEndLevelCollision()
        {
            return EndCollider.Intersects(this.ColliderBox);
        }

        public bool IsEnemyCollision()
        {
            return game.enemy.ColliderBox.Intersects(this.ColliderBox);
        }

        public void DrawCollider(BasicEffect basicEffect, GraphicsDevice GraphicsDevice)
        {
            using (var line = new Line())
            {
                var n = ColliderBox.Min;
                var x = ColliderBox.Max;

                line.DrawLine(basicEffect, GraphicsDevice, n, new Vector3(n.X, n.Y, x.Z), Color.Red); // N -> N+X.z 1-2
                line.DrawLine(basicEffect, GraphicsDevice, n, new Vector3(n.X, x.Y, n.Z), Color.Red); // N -> N+X.y 1-3
                line.DrawLine(basicEffect, GraphicsDevice, n, new Vector3(x.X, n.Y, n.Z), Color.Red); // N -> N+X.z 1-4

                line.DrawLine(basicEffect, GraphicsDevice, x, new Vector3(n.X, x.Y, x.Z), Color.Blue); // X -> X+N.x 5-6
                line.DrawLine(basicEffect, GraphicsDevice, x, new Vector3(x.X, n.Y, x.Z), Color.Blue); // X -> X+N.y 5-7
                line.DrawLine(basicEffect, GraphicsDevice, x, new Vector3(x.X, x.Y, n.Z), Color.Blue); // X -> X+N.z 5-8

                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(x.X, n.Y, n.Z), new Vector3(x.X, n.Y, x.Z),
                    Color.Salmon); // 4-7
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(x.X, n.Y, x.Z), new Vector3(n.X, n.Y, x.Z),
                    Color.Salmon); // 7-2
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(n.X, n.Y, x.Z), new Vector3(n.X, x.Y, x.Z),
                    Color.Salmon); // 2-6

                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(n.X, x.Y, x.Z), new Vector3(n.X, x.Y, n.Z),
                    Color.Cyan); // 6-3
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(n.X, x.Y, n.Z), new Vector3(x.X, x.Y, n.Z),
                    Color.Cyan); // 3-8
                line.DrawLine(basicEffect, GraphicsDevice, new Vector3(x.X, x.Y, n.Z), new Vector3(x.X, n.Y, n.Z),
                    Color.Cyan); // 8-4
            }
        }

        public void AddColliderObject(Collider col)
        {
            ColliderObjects.Add(col);
        }

        public void AddColliderObjects(List<Collider> cols)
        {
            ColliderObjects.AddRange(cols);
        }
        public void AddColectableObject(Collider col)
        {
            CollectablesObjects.Add(col);
        }

        public void AddColectableObjects(List<Collider> cols)
        {
            CollectablesObjects.AddRange(cols);
        }

        public void ResetColiders()
        {
            ColliderObjects = new List<Collider>();
            CollectablesObjects = new List<Collider>();
        }
    }
}
