using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MazeEscape.Interfaces
{
    public interface IObject3D
    {
        Effect lighting { get; set; }
        Vector3 Position { get; set; }
        
        bool Visible { get; set; }

        Vector3 Rotation { get; set; }

        Model Model { get; set; }

		Vector3 Scale { get; set; }

		void Draw();
        void DrawAt(Vector3 position);

        ContentManager Content { get; set; }

        void Load(string path);

    }
}
