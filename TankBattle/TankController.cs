using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankBattle
{
    abstract public class TankController
    {
        public TankController(string name, Chassis tank, Color colour)
        {
            throw new NotImplementedException();
        }
        public Chassis CreateTank()
        {
            throw new NotImplementedException();
        }
        public string PlayerName()
        {
            throw new NotImplementedException();
        }
        public Color GetColour()
        {
            throw new NotImplementedException();
        }
        public void WonRound()
        {
            throw new NotImplementedException();
        }
        public int GetScore()
        {
            throw new NotImplementedException();
        }

        public abstract void StartRound();

        public abstract void CommenceTurn(GameForm gameplayForm, GameController currentGame);

        public abstract void ReportHit(float x, float y);
    }
}
