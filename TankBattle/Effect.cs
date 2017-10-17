using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TankBattle
{
    public abstract class Effect
    {
        public void SetCurrentGame(GameController game)
        {
            throw new NotImplementedException();
        }

        public abstract void Step();
        public abstract void Draw(Graphics graphics, Size displaySize);
    }
}
