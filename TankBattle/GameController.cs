using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace TankBattle
{
    public class GameController
    {
        private int numberPlayers;
        private int numberRounds;
        private int currentRound;
        private static Random randomNumber = new Random();

        public GameController(int numPlayers, int numRounds)
        {
            this.numberPlayers = numPlayers;
            this.numberRounds = numRounds;

            int[] tankController = new int[numPlayers];
            currentRound = numRounds;
            int[] effect = new int[100];

        }

        public int NumPlayers()
        {
            return this.numberPlayers;
        }

        public int GetRound()
        {
            return currentRound;
        }

        public int GetTotalRounds()
        {
            return this.numberRounds;
        }

        public void RegisterPlayer(int playerNum, TankController player)
        {
            throw new NotImplementedException();
        }

        public TankController GetPlayerNumber(int playerNum)
        {
            throw new NotImplementedException();
        }

        public BattleTank GetGameplayTank(int playerNum)
        {
            throw new NotImplementedException();
        }

        public static Color TankColour(int playerNum)
        {
            Color playerColour;

            Color[] colours = new Color[10];
            colours[0] = Color.Azure;
            colours[1] = Color.Coral;
            colours[2] = Color.ForestGreen;
            colours[3] = Color.Fuchsia;
            colours[4] = Color.Maroon;
            colours[5] = Color.MidnightBlue;
            colours[6] = Color.OrangeRed;
            colours[7] = Color.PaleGreen;
            colours[8] = Color.Yellow;
            colours[9] = Color.Purple;

            playerColour = colours[playerNum-1];

            return playerColour;
                            
        }

        public static int[] GetPlayerPositions(int numPlayers)
        {
            int[] playerPositions = new int[numPlayers];
            int deltaPlayers;

            for (int playerNum = 1; playerNum <= numPlayers; playerNum++) {
                deltaPlayers = (Map.WIDTH / numPlayers);
                playerPositions[playerNum-1] = ((deltaPlayers * playerNum) - (deltaPlayers / 2)) - 2;
            }

            return playerPositions;
        }

        public static void RandomReorder(int[] array)
        {
            int arrSize = array.Length;
            int temp = 0;
            for (int i = 0; i < arrSize; i++) {
                temp = randomNumber.Next(0, arrSize-1);
                if (array.Contains(temp)) {
                    array[i] = temp;
                } else {
                    i--;
                }
            }
        }

        public void StartGame()
        {
            throw new NotImplementedException();
        }

        public void NewRound()
        {
            throw new NotImplementedException();
        }

        public Map GetBattlefield()
        {
            throw new NotImplementedException();
        }

        public void DrawTanks(Graphics graphics, Size displaySize)
        {
            throw new NotImplementedException();
        }

        public BattleTank CurrentPlayerTank()
        {
            throw new NotImplementedException();
        }

        public void AddEffect(Effect weaponEffect)
        {
            throw new NotImplementedException();
        }

        public bool WeaponEffectTick()
        {
            throw new NotImplementedException();
        }

        public void DrawAttacks(Graphics graphics, Size displaySize)
        {
            throw new NotImplementedException();
        }

        public void RemoveEffect(Effect weaponEffect)
        {
            throw new NotImplementedException();
        }

        public bool CheckCollidedTank(float projectileX, float projectileY)
        {
            throw new NotImplementedException();
        }

        public void InflictDamage(float damageX, float damageY, float explosionDamage, float radius)
        {
            throw new NotImplementedException();
        }

        public bool Gravity()
        {
            throw new NotImplementedException();
        }

        public bool TurnOver()
        {
            throw new NotImplementedException();
        }

        public void ScoreWinner()
        {
            throw new NotImplementedException();
        }

        public void NextRound()
        {
            throw new NotImplementedException();
        }
        
        public int GetWind()
        {
            throw new NotImplementedException();
        }
    }
}
