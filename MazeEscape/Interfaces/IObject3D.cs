using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjektTestowy.Interfaces
{
    public interface IObject3D
    {
        Vector3 Position { get; set; }
        
        bool Visible { get; set; }

        Vector3 Rotation { get; set; }

        Model Model { get; set; }

        void Draw();

    }
}
