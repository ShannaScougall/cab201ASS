using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TankBattle;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace TankBattleTestSuite
{
    class RequirementException : Exception
    {
        public RequirementException()
        {
        }

        public RequirementException(string message) : base(message)
        {
        }

        public RequirementException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    class Test
    {
        #region Testing Code

        private delegate bool TestCase();

        private static string ErrorDescription = null;

        private static void SetErrorDescription(string desc)
        {
            ErrorDescription = desc;
        }

        private static bool FloatEquals(float a, float b)
        {
            if (Math.Abs(a - b) < 0.01) return true;
            return false;
        }

        private static Dictionary<string, string> unitTestResults = new Dictionary<string, string>();

        private static void Passed(string name, string comment)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[passed] ");
            Console.ResetColor();
            Console.Write("{0}", name);
            if (comment != "")
            {
                Console.Write(": {0}", comment);
            }
            if (ErrorDescription != null)
            {
                throw new Exception("ErrorDescription found for passing test case");
            }
            Console.WriteLine();
        }
        private static void Failed(string name, string comment)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[failed] ");
            Console.ResetColor();
            Console.Write("{0}", name);
            if (comment != "")
            {
                Console.Write(": {0}", comment);
            }
            if (ErrorDescription != null)
            {
                Console.Write("\n{0}", ErrorDescription);
                ErrorDescription = null;
            }
            Console.WriteLine();
        }
        private static void FailedToMeetRequirement(string name, string comment)
        {
            Console.Write("[      ] ");
            Console.Write("{0}", name);
            if (comment != "")
            {
                Console.Write(": ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("{0}", comment);
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        private static void DoTest(TestCase test)
        {
            // Have we already completed this test?
            if (unitTestResults.ContainsKey(test.Method.ToString()))
            {
                return;
            }

            bool passed = false;
            bool metRequirement = true;
            string exception = "";
            try
            {
                passed = test();
            }
            catch (RequirementException e)
            {
                metRequirement = false;
                exception = e.Message;
            }
            catch (Exception e)
            {
                exception = e.GetType().ToString();
            }

            string className = test.Method.ToString().Replace("Boolean Test", "").Split('0')[0];
            string fnName = test.Method.ToString().Split('0')[1];

            if (metRequirement)
            {
                if (passed)
                {
                    unitTestResults[test.Method.ToString()] = "Passed";
                    Passed(string.Format("{0}.{1}", className, fnName), exception);
                }
                else
                {
                    unitTestResults[test.Method.ToString()] = "Failed";
                    Failed(string.Format("{0}.{1}", className, fnName), exception);
                }
            }
            else
            {
                unitTestResults[test.Method.ToString()] = "Failed";
                FailedToMeetRequirement(string.Format("{0}.{1}", className, fnName), exception);
            }
            Cleanup();
        }

        private static Stack<string> errorDescriptionStack = new Stack<string>();


        private static void Requires(TestCase test)
        {
            string result;
            bool wasTested = unitTestResults.TryGetValue(test.Method.ToString(), out result);

            if (!wasTested)
            {
                // Push the error description onto the stack (only thing that can change, not that it should)
                errorDescriptionStack.Push(ErrorDescription);

                // Do the test
                DoTest(test);

                // Pop the description off
                ErrorDescription = errorDescriptionStack.Pop();

                // Get the proper result for out
                wasTested = unitTestResults.TryGetValue(test.Method.ToString(), out result);

                if (!wasTested)
                {
                    throw new Exception("This should never happen");
                }
            }

            if (result == "Failed")
            {
                string className = test.Method.ToString().Replace("Boolean Test", "").Split('0')[0];
                string fnName = test.Method.ToString().Split('0')[1];

                throw new RequirementException(string.Format("-> {0}.{1}", className, fnName));
            }
            else if (result == "Passed")
            {
                return;
            }
            else
            {
                throw new Exception("This should never happen");
            }

        }

        #endregion

        #region Test Cases
        private static GameController InitialiseGame()
        {
            Requires(TestGameController0GameController);
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0HumanOpponent);
            Requires(TestGameController0RegisterPlayer);

            GameController game = new GameController(2, 1);
            Chassis tank = Chassis.CreateTank(1);
            TankController player1 = new HumanOpponent("player1", tank, Color.Orange);
            TankController player2 = new HumanOpponent("player2", tank, Color.Purple);
            game.RegisterPlayer(1, player1);
            game.RegisterPlayer(2, player2);
            return game;
        }
        private static void Cleanup()
        {
            while (Application.OpenForms.Count > 0)
            {
                Application.OpenForms[0].Dispose();
            }
        }
        private static bool TestGameController0GameController()
        {
            GameController game = new GameController(2, 1);
            return true;
        }
        private static bool TestGameController0NumPlayers()
        {
            Requires(TestGameController0GameController);

            GameController game = new GameController(2, 1);
            return game.NumPlayers() == 2;
        }
        private static bool TestGameController0GetTotalRounds()
        {
            Requires(TestGameController0GameController);

            GameController game = new GameController(3, 5);
            return game.GetTotalRounds() == 5;
        }
        private static bool TestGameController0RegisterPlayer()
        {
            Requires(TestGameController0GameController);
            Requires(TestChassis0CreateTank);

            GameController game = new GameController(2, 1);
            Chassis tank = Chassis.CreateTank(1);
            TankController player = new HumanOpponent("playerName", tank, Color.Orange);
            game.RegisterPlayer(1, player);
            return true;
        }
        private static bool TestGameController0GetPlayerNumber()
        {
            Requires(TestGameController0GameController);
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0HumanOpponent);

            GameController game = new GameController(2, 1);
            Chassis tank = Chassis.CreateTank(1);
            TankController player = new HumanOpponent("playerName", tank, Color.Orange);
            game.RegisterPlayer(1, player);
            return game.GetPlayerNumber(1) == player;
        }
        private static bool TestGameController0TankColour()
        {
            Color[] arrayOfColours = new Color[8];
            for (int i = 0; i < 8; i++)
            {
                arrayOfColours[i] = GameController.TankColour(i + 1);
                for (int j = 0; j < i; j++)
                {
                    if (arrayOfColours[j] == arrayOfColours[i]) return false;
                }
            }
            return true;
        }
        private static bool TestGameController0GetPlayerPositions()
        {
            int[] positions = GameController.GetPlayerPositions(8);
            for (int i = 0; i < 8; i++)
            {
                if (positions[i] < 0) return false;
                if (positions[i] > 160) return false;
                for (int j = 0; j < i; j++)
                {
                    if (positions[j] == positions[i]) return false;
                }
            }
            return true;
        }
        private static bool TestGameController0RandomReorder()
        {
            int[] ar = new int[100];
            for (int i = 0; i < 100; i++)
            {
                ar[i] = i;
            }
            GameController.RandomReorder(ar);
            for (int i = 0; i < 100; i++)
            {
                if (ar[i] != i)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool TestGameController0StartGame()
        {
            GameController game = InitialiseGame();
            game.StartGame();

            foreach (Form f in Application.OpenForms)
            {
                if (f is GameForm)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool TestGameController0GetBattlefield()
        {
            Requires(TestMap0Map);
            GameController game = InitialiseGame();
            game.StartGame();
            Map battlefield = game.GetBattlefield();
            if (battlefield != null) return true;

            return false;
        }
        private static bool TestGameController0CurrentPlayerTank()
        {
            Requires(TestGameController0GameController);
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0HumanOpponent);
            Requires(TestGameController0RegisterPlayer);
            Requires(TestBattleTank0GetPlayerNumber);

            GameController game = new GameController(2, 1);
            Chassis tank = Chassis.CreateTank(1);
            TankController player1 = new HumanOpponent("player1", tank, Color.Orange);
            TankController player2 = new HumanOpponent("player2", tank, Color.Purple);
            game.RegisterPlayer(1, player1);
            game.RegisterPlayer(2, player2);

            game.StartGame();
            BattleTank ptank = game.CurrentPlayerTank();
            if (ptank.GetPlayerNumber() != player1 && ptank.GetPlayerNumber() != player2)
            {
                return false;
            }
            if (ptank.CreateTank() != tank)
            {
                return false;
            }

            return true;
        }

        private static bool TestChassis0CreateTank()
        {
            Chassis tank = Chassis.CreateTank(1);
            if (tank != null) return true;
            else return false;
        }
        private static bool TestChassis0DisplayTankSprite()
        {
            Requires(TestChassis0CreateTank);
            Chassis tank = Chassis.CreateTank(1);

            int[,] tankGraphic = tank.DisplayTankSprite(45);
            if (tankGraphic.GetLength(0) != 12) return false;
            if (tankGraphic.GetLength(1) != 16) return false;
            // We don't really care what the tank looks like, but the 45 degree tank
            // should at least look different to the -45 degree tank
            int[,] tankGraphic2 = tank.DisplayTankSprite(-45);
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    if (tankGraphic2[y, x] != tankGraphic[y, x])
                    {
                        return true;
                    }
                }
            }

            SetErrorDescription("Tank with turret at -45 degrees looks the same as tank with turret at 45 degrees");

            return false;
        }
        private static void DisplayLine(int[,] array)
        {
            string report = "";
            report += "A line drawn from 3,0 to 0,3 on a 4x4 array should look like this:\n";
            report += "0001\n";
            report += "0010\n";
            report += "0100\n";
            report += "1000\n";
            report += "The one produced by Chassis.DrawLine() looks like this:\n";
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    report += array[y, x] == 1 ? "1" : "0";
                }
                report += "\n";
            }
            SetErrorDescription(report);
        }
        private static bool TestChassis0DrawLine()
        {
            int[,] ar = new int[,] { { 0, 0, 0, 0 },
                                     { 0, 0, 0, 0 },
                                     { 0, 0, 0, 0 },
                                     { 0, 0, 0, 0 } };
            Chassis.DrawLine(ar, 3, 0, 0, 3);

            // Ideally, the line we want to see here is:
            // 0001
            // 0010
            // 0100
            // 1000

            // However, as we aren't that picky, as long as they have a 1 in every row and column
            // and nothing in the top-left and bottom-right corners

            int[] rows = new int[4];
            int[] cols = new int[4];
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (ar[y, x] == 1)
                    {
                        rows[y] = 1;
                        cols[x] = 1;
                    }
                    else if (ar[y, x] > 1 || ar[y, x] < 0)
                    {
                        // Only values 0 and 1 are permitted
                        SetErrorDescription(string.Format("Somehow the number {0} got into the array.", ar[y, x]));
                        return false;
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (rows[i] == 0)
                {
                    DisplayLine(ar);
                    return false;
                }
                if (cols[i] == 0)
                {
                    DisplayLine(ar);
                    return false;
                }
            }
            if (ar[0, 0] == 1)
            {
                DisplayLine(ar);
                return false;
            }
            if (ar[3, 3] == 1)
            {
                DisplayLine(ar);
                return false;
            }

            return true;
        }
        private static bool TestChassis0GetTankArmour()
        {
            Requires(TestChassis0CreateTank);
            // As long as it's > 0 we're happy
            Chassis tank = Chassis.CreateTank(1);
            if (tank.GetTankArmour() > 0) return true;
            return false;
        }
        private static bool TestChassis0Weapons()
        {
            Requires(TestChassis0CreateTank);
            // As long as there's at least one result and it's not null / a blank string, we're happy
            Chassis tank = Chassis.CreateTank(1);
            if (tank.Weapons().Length == 0) return false;
            if (tank.Weapons()[0] == null) return false;
            if (tank.Weapons()[0] == "") return false;
            return true;
        }

        private static TankController CreateTestingPlayer()
        {
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0HumanOpponent);

            Chassis tank = Chassis.CreateTank(1);
            TankController player = new HumanOpponent("player1", tank, Color.Aquamarine);
            return player;
        }

        private static bool TestTankController0HumanOpponent()
        {
            Requires(TestChassis0CreateTank);

            Chassis tank = Chassis.CreateTank(1);
            TankController player = new HumanOpponent("player1", tank, Color.Aquamarine);
            if (player != null) return true;
            return false;
        }
        private static bool TestTankController0CreateTank()
        {
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0HumanOpponent);

            Chassis tank = Chassis.CreateTank(1);
            TankController p = new HumanOpponent("player1", tank, Color.Aquamarine);
            if (p.CreateTank() == tank) return true;
            return false;
        }
        private static bool TestTankController0PlayerName()
        {
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0HumanOpponent);

            const string PLAYER_NAME = "kfdsahskfdajh";
            Chassis tank = Chassis.CreateTank(1);
            TankController p = new HumanOpponent(PLAYER_NAME, tank, Color.Aquamarine);
            if (p.PlayerName() == PLAYER_NAME) return true;
            return false;
        }
        private static bool TestTankController0GetColour()
        {
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0HumanOpponent);

            Color playerColour = Color.Chartreuse;
            Chassis tank = Chassis.CreateTank(1);
            TankController p = new HumanOpponent("player1", tank, playerColour);
            if (p.GetColour() == playerColour) return true;
            return false;
        }
        private static bool TestTankController0WonRound()
        {
            TankController p = CreateTestingPlayer();
            p.WonRound();
            return true;
        }
        private static bool TestTankController0GetScore()
        {
            Requires(TestTankController0WonRound);

            TankController p = CreateTestingPlayer();
            int wins = p.GetScore();
            p.WonRound();
            if (p.GetScore() == wins + 1) return true;
            return false;
        }
        private static bool TestHumanOpponent0StartRound()
        {
            TankController p = CreateTestingPlayer();
            p.StartRound();
            return true;
        }
        private static bool TestHumanOpponent0CommenceTurn()
        {
            Requires(TestGameController0StartGame);
            Requires(TestGameController0GetPlayerNumber);
            GameController game = InitialiseGame();

            game.StartGame();

            // Find the gameplay form
            GameForm gameplayForm = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f is GameForm)
                {
                    gameplayForm = f as GameForm;
                }
            }
            if (gameplayForm == null)
            {
                SetErrorDescription("Gameplay form was not created by GameController.StartGame()");
                return false;
            }

            // Find the control panel
            Panel controlPanel = null;
            foreach (Control c in gameplayForm.Controls)
            {
                if (c is Panel)
                {
                    foreach (Control cc in c.Controls)
                    {
                        if (cc is NumericUpDown || cc is Label || cc is TrackBar)
                        {
                            controlPanel = c as Panel;
                        }
                    }
                }
            }

            if (controlPanel == null)
            {
                SetErrorDescription("Control panel was not found in GameForm");
                return false;
            }

            // Disable the control panel to check that NewTurn enables it
            controlPanel.Enabled = false;

            game.GetPlayerNumber(1).CommenceTurn(gameplayForm, game);

            if (!controlPanel.Enabled)
            {
                SetErrorDescription("Control panel is still disabled after HumanPlayer.NewTurn()");
                return false;
            }
            return true;

        }
        private static bool TestHumanOpponent0ReportHit()
        {
            TankController p = CreateTestingPlayer();
            p.ReportHit(0, 0);
            return true;
        }

        private static bool TestBattleTank0BattleTank()
        {
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);
            return true;
        }
        private static bool TestBattleTank0GetPlayerNumber()
        {
            Requires(TestBattleTank0BattleTank);
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);
            if (playerTank.GetPlayerNumber() == p) return true;
            return false;
        }
        private static bool TestBattleTank0CreateTank()
        {
            Requires(TestBattleTank0BattleTank);
            Requires(TestTankController0CreateTank);
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);
            if (playerTank.CreateTank() == playerTank.GetPlayerNumber().CreateTank()) return true;
            return false;
        }
        private static bool TestBattleTank0GetAim()
        {
            Requires(TestBattleTank0BattleTank);
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);
            float angle = playerTank.GetAim();
            if (angle >= -90 && angle <= 90) return true;
            return false;
        }
        private static bool TestBattleTank0Aim()
        {
            Requires(TestBattleTank0BattleTank);
            Requires(TestBattleTank0GetAim);
            float angle = 75;
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);
            playerTank.Aim(angle);
            if (FloatEquals(playerTank.GetAim(), angle)) return true;
            return false;
        }
        private static bool TestBattleTank0GetCurrentPower()
        {
            Requires(TestBattleTank0BattleTank);
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);

            playerTank.GetCurrentPower();
            return true;
        }
        private static bool TestBattleTank0SetPower()
        {
            Requires(TestBattleTank0BattleTank);
            Requires(TestBattleTank0GetCurrentPower);
            int power = 65;
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);
            playerTank.SetPower(power);
            if (playerTank.GetCurrentPower() == power) return true;
            return false;
        }
        private static bool TestBattleTank0GetWeaponIndex()
        {
            Requires(TestBattleTank0BattleTank);

            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);

            playerTank.GetWeaponIndex();
            return true;
        }
        private static bool TestBattleTank0SetWeaponIndex()
        {
            Requires(TestBattleTank0BattleTank);
            Requires(TestBattleTank0GetWeaponIndex);
            int weapon = 3;
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);
            playerTank.SetWeaponIndex(weapon);
            if (playerTank.GetWeaponIndex() == weapon) return true;
            return false;
        }
        private static bool TestBattleTank0Draw()
        {
            Requires(TestBattleTank0BattleTank);
            Size bitmapSize = new Size(640, 480);
            Bitmap image = new Bitmap(bitmapSize.Width, bitmapSize.Height);
            Graphics graphics = Graphics.FromImage(image);
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);
            playerTank.Draw(graphics, bitmapSize);
            graphics.Dispose();

            for (int y = 0; y < bitmapSize.Height; y++)
            {
                for (int x = 0; x < bitmapSize.Width; x++)
                {
                    if (image.GetPixel(x, y) != image.GetPixel(0, 0))
                    {
                        // Something changed in the image, and that's good enough for me
                        return true;
                    }
                }
            }
            SetErrorDescription("Nothing was drawn.");
            return false;
        }
        private static bool TestBattleTank0XPos()
        {
            Requires(TestBattleTank0BattleTank);

            TankController p = CreateTestingPlayer();
            int x = 73;
            int y = 28;
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, x, y, game);
            if (playerTank.XPos() == x) return true;
            return false;
        }
        private static bool TestBattleTank0YPos()
        {
            Requires(TestBattleTank0BattleTank);

            TankController p = CreateTestingPlayer();
            int x = 73;
            int y = 28;
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, x, y, game);
            if (playerTank.YPos() == y) return true;
            return false;
        }
        private static bool TestBattleTank0Fire()
        {
            Requires(TestBattleTank0BattleTank);

            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);
            playerTank.Fire();
            return true;
        }
        private static bool TestBattleTank0InflictDamage()
        {
            Requires(TestBattleTank0BattleTank);
            TankController p = CreateTestingPlayer();

            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);
            playerTank.InflictDamage(10);
            return true;
        }
        private static bool TestBattleTank0Alive()
        {
            Requires(TestBattleTank0BattleTank);
            Requires(TestBattleTank0InflictDamage);

            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            BattleTank playerTank = new BattleTank(p, 32, 32, game);
            if (!playerTank.Alive()) return false;
            playerTank.InflictDamage(playerTank.CreateTank().GetTankArmour());
            if (playerTank.Alive()) return false;
            return true;
        }
        private static bool TestBattleTank0Gravity()
        {
            Requires(TestGameController0GetBattlefield);
            Requires(TestMap0DestroyTerrain);
            Requires(TestBattleTank0BattleTank);
            Requires(TestBattleTank0InflictDamage);
            Requires(TestBattleTank0Alive);
            Requires(TestBattleTank0CreateTank);
            Requires(TestChassis0GetTankArmour);

            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            game.StartGame();
            // Unfortunately we need to rely on DestroyTerrain() to get rid of any terrain that may be in the way
            game.GetBattlefield().DestroyTerrain(Map.WIDTH / 2.0f, Map.HEIGHT / 2.0f, 20);
            BattleTank playerTank = new BattleTank(p, Map.WIDTH / 2, Map.HEIGHT / 2, game);
            int oldX = playerTank.XPos();
            int oldY = playerTank.YPos();

            playerTank.Gravity();

            if (playerTank.XPos() != oldX)
            {
                SetErrorDescription("Caused X coordinate to change.");
                return false;
            }
            if (playerTank.YPos() != oldY + 1)
            {
                SetErrorDescription("Did not cause Y coordinate to increase by 1.");
                return false;
            }

            int initialArmour = playerTank.CreateTank().GetTankArmour();
            // The tank should have lost 1 armour from falling 1 tile already, so do
            // (initialArmour - 2) damage to the tank then drop it again. That should kill it.

            if (!playerTank.Alive())
            {
                SetErrorDescription("Tank died before we could check that fall damage worked properly");
                return false;
            }
            playerTank.InflictDamage(initialArmour - 2);
            if (!playerTank.Alive())
            {
                SetErrorDescription("Tank died before we could check that fall damage worked properly");
                return false;
            }
            playerTank.Gravity();
            if (playerTank.Alive())
            {
                SetErrorDescription("Tank survived despite taking enough falling damage to destroy it");
                return false;
            }

            return true;
        }
        private static bool TestMap0Map()
        {
            Map battlefield = new Map();
            return true;
        }
        private static bool TestMap0TerrainAt()
        {
            Requires(TestMap0Map);

            bool foundTrue = false;
            bool foundFalse = false;
            Map battlefield = new Map();
            for (int y = 0; y < Map.HEIGHT; y++)
            {
                for (int x = 0; x < Map.WIDTH; x++)
                {
                    if (battlefield.TerrainAt(x, y))
                    {
                        foundTrue = true;
                    }
                    else
                    {
                        foundFalse = true;
                    }
                }
            }

            if (!foundTrue)
            {
                SetErrorDescription("IsTileAt() did not return true for any tile.");
                return false;
            }

            if (!foundFalse)
            {
                SetErrorDescription("IsTileAt() did not return false for any tile.");
                return false;
            }

            return true;
        }
        private static bool TestMap0CheckTankCollision()
        {
            Requires(TestMap0Map);
            Requires(TestMap0TerrainAt);

            Map battlefield = new Map();
            for (int y = 0; y <= Map.HEIGHT - Chassis.HEIGHT; y++)
            {
                for (int x = 0; x <= Map.WIDTH - Chassis.WIDTH; x++)
                {
                    int colTiles = 0;
                    for (int iy = 0; iy < Chassis.HEIGHT; iy++)
                    {
                        for (int ix = 0; ix < Chassis.WIDTH; ix++)
                        {

                            if (battlefield.TerrainAt(x + ix, y + iy))
                            {
                                colTiles++;
                            }
                        }
                    }
                    if (colTiles == 0)
                    {
                        if (battlefield.CheckTankCollision(x, y))
                        {
                            SetErrorDescription("Found collision where there shouldn't be one");
                            return false;
                        }
                    }
                    else
                    {
                        if (!battlefield.CheckTankCollision(x, y))
                        {
                            SetErrorDescription("Didn't find collision where there should be one");
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        private static bool TestMap0TankYPosition()
        {
            Requires(TestMap0Map);
            Requires(TestMap0TerrainAt);

            Map battlefield = new Map();
            for (int x = 0; x <= Map.WIDTH - Chassis.WIDTH; x++)
            {
                int lowestValid = 0;
                for (int y = 0; y <= Map.HEIGHT - Chassis.HEIGHT; y++)
                {
                    int colTiles = 0;
                    for (int iy = 0; iy < Chassis.HEIGHT; iy++)
                    {
                        for (int ix = 0; ix < Chassis.WIDTH; ix++)
                        {

                            if (battlefield.TerrainAt(x + ix, y + iy))
                            {
                                colTiles++;
                            }
                        }
                    }
                    if (colTiles == 0)
                    {
                        lowestValid = y;
                    }
                }

                int placedY = battlefield.TankYPosition(x);
                if (placedY != lowestValid)
                {
                    SetErrorDescription(string.Format("Tank was placed at {0},{1} when it should have been placed at {0},{2}", x, placedY, lowestValid));
                    return false;
                }
            }
            return true;
        }
        private static bool TestMap0DestroyTerrain()
        {
            Requires(TestMap0Map);
            Requires(TestMap0TerrainAt);

            Map battlefield = new Map();
            for (int y = 0; y < Map.HEIGHT; y++)
            {
                for (int x = 0; x < Map.WIDTH; x++)
                {
                    if (battlefield.TerrainAt(x, y))
                    {
                        battlefield.DestroyTerrain(x, y, 0.5f);
                        if (battlefield.TerrainAt(x, y))
                        {
                            SetErrorDescription("Attempted to destroy terrain but it still exists");
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            SetErrorDescription("Did not find any terrain to destroy");
            return false;
        }
        private static bool TestMap0Gravity()
        {
            Requires(TestMap0Map);
            Requires(TestMap0TerrainAt);
            Requires(TestMap0DestroyTerrain);

            Map battlefield = new Map();
            for (int x = 0; x < Map.WIDTH; x++)
            {
                if (battlefield.TerrainAt(x, Map.HEIGHT - 1))
                {
                    if (battlefield.TerrainAt(x, Map.HEIGHT - 2))
                    {
                        // Seek up and find the first non-set tile
                        for (int y = Map.HEIGHT - 2; y >= 0; y--)
                        {
                            if (!battlefield.TerrainAt(x, y))
                            {
                                // Do a gravity step and make sure it doesn't slip down
                                battlefield.Gravity();
                                if (!battlefield.TerrainAt(x, y + 1))
                                {
                                    SetErrorDescription("Moved down terrain even though there was no room");
                                    return false;
                                }

                                // Destroy the bottom-most tile
                                battlefield.DestroyTerrain(x, Map.HEIGHT - 1, 0.5f);

                                // Do a gravity step and make sure it does slip down
                                battlefield.Gravity();

                                if (battlefield.TerrainAt(x, y + 1))
                                {
                                    SetErrorDescription("Terrain didn't fall");
                                    return false;
                                }

                                // Otherwise this seems to have worked
                                return true;
                            }
                        }


                    }
                }
            }
            SetErrorDescription("Did not find any appropriate terrain to test");
            return false;
        }
        private static bool TestEffect0SetCurrentGame()
        {
            Requires(TestBoom0Boom);
            Requires(TestGameController0GameController);

            Effect weaponEffect = new Boom(1, 1, 1);
            GameController game = new GameController(2, 1);
            weaponEffect.SetCurrentGame(game);
            return true;
        }
        private static bool TestBullet0Bullet()
        {
            Requires(TestBoom0Boom);
            TankController player = CreateTestingPlayer();
            Boom explosion = new Boom(1, 1, 1);
            Bullet projectile = new Bullet(25, 25, 45, 30, 0.02f, explosion, player);
            return true;
        }
        private static bool TestBullet0Step()
        {
            Requires(TestGameController0StartGame);
            Requires(TestBoom0Boom);
            Requires(TestBullet0Bullet);
            Requires(TestEffect0SetCurrentGame);
            GameController game = InitialiseGame();
            game.StartGame();
            TankController player = game.GetPlayerNumber(1);
            Boom explosion = new Boom(1, 1, 1);

            Bullet projectile = new Bullet(25, 25, 45, 100, 0.01f, explosion, player);
            projectile.SetCurrentGame(game);
            projectile.Step();

            // We can't really test this one without a substantial framework,
            // so we just call it and hope that everything works out

            return true;
        }
        private static bool TestBullet0Draw()
        {
            Requires(TestGameController0StartGame);
            Requires(TestGameController0GetPlayerNumber);
            Requires(TestBoom0Boom);
            Requires(TestBullet0Bullet);
            Requires(TestEffect0SetCurrentGame);

            Size bitmapSize = new Size(640, 480);
            Bitmap image = new Bitmap(bitmapSize.Width, bitmapSize.Height);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.Black); // Blacken out the image so we can see the projectile
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            game.StartGame();
            TankController player = game.GetPlayerNumber(1);
            Boom explosion = new Boom(1, 1, 1);

            Bullet projectile = new Bullet(25, 25, 45, 100, 0.01f, explosion, player);
            projectile.SetCurrentGame(game);
            projectile.Draw(graphics, bitmapSize);
            graphics.Dispose();

            for (int y = 0; y < bitmapSize.Height; y++)
            {
                for (int x = 0; x < bitmapSize.Width; x++)
                {
                    if (image.GetPixel(x, y) != image.GetPixel(0, 0))
                    {
                        // Something changed in the image, and that's good enough for me
                        return true;
                    }
                }
            }
            SetErrorDescription("Nothing was drawn.");
            return false;
        }
        private static bool TestBoom0Boom()
        {
            TankController player = CreateTestingPlayer();
            Boom explosion = new Boom(1, 1, 1);

            return true;
        }
        private static bool TestBoom0Explode()
        {
            Requires(TestBoom0Boom);
            Requires(TestEffect0SetCurrentGame);
            Requires(TestGameController0GetPlayerNumber);
            Requires(TestGameController0StartGame);

            GameController game = InitialiseGame();
            game.StartGame();
            TankController player = game.GetPlayerNumber(1);
            Boom explosion = new Boom(1, 1, 1);
            explosion.SetCurrentGame(game);
            explosion.Explode(25, 25);

            return true;
        }
        private static bool TestBoom0Step()
        {
            Requires(TestBoom0Boom);
            Requires(TestEffect0SetCurrentGame);
            Requires(TestGameController0GetPlayerNumber);
            Requires(TestGameController0StartGame);
            Requires(TestBoom0Explode);

            GameController game = InitialiseGame();
            game.StartGame();
            TankController player = game.GetPlayerNumber(1);
            Boom explosion = new Boom(1, 1, 1);
            explosion.SetCurrentGame(game);
            explosion.Explode(25, 25);
            explosion.Step();

            // Again, we can't really test this one without a full framework

            return true;
        }
        private static bool TestBoom0Draw()
        {
            Requires(TestBoom0Boom);
            Requires(TestEffect0SetCurrentGame);
            Requires(TestGameController0GetPlayerNumber);
            Requires(TestGameController0StartGame);
            Requires(TestBoom0Explode);
            Requires(TestBoom0Step);

            Size bitmapSize = new Size(640, 480);
            Bitmap image = new Bitmap(bitmapSize.Width, bitmapSize.Height);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.Black); // Blacken out the image so we can see the explosion
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            game.StartGame();
            TankController player = game.GetPlayerNumber(1);
            Boom explosion = new Boom(10, 10, 10);
            explosion.SetCurrentGame(game);
            explosion.Explode(25, 25);
            // Step it for a bit so we can be sure the explosion is visible
            for (int i = 0; i < 10; i++)
            {
                explosion.Step();
            }
            explosion.Draw(graphics, bitmapSize);

            for (int y = 0; y < bitmapSize.Height; y++)
            {
                for (int x = 0; x < bitmapSize.Width; x++)
                {
                    if (image.GetPixel(x, y) != image.GetPixel(0, 0))
                    {
                        // Something changed in the image, and that's good enough for me
                        return true;
                    }
                }
            }
            SetErrorDescription("Nothing was drawn.");
            return false;
        }

        private static GameForm InitialiseGameForm(out NumericUpDown angleCtrl, out TrackBar powerCtrl, out Button fireCtrl, out Panel controlPanel, out ListBox weaponSelect)
        {
            Requires(TestGameController0StartGame);

            GameController game = InitialiseGame();

            angleCtrl = null;
            powerCtrl = null;
            fireCtrl = null;
            controlPanel = null;
            weaponSelect = null;

            game.StartGame();
            GameForm gameplayForm = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f is GameForm)
                {
                    gameplayForm = f as GameForm;
                }
            }
            if (gameplayForm == null)
            {
                SetErrorDescription("GameController.StartGame() did not create a GameForm and that is the only way GameForm can be tested");
                return null;
            }

            bool foundDisplayPanel = false;
            bool foundControlPanel = false;

            foreach (Control c in gameplayForm.Controls)
            {
                // The only controls should be 2 panels
                if (c is Panel)
                {
                    // Is this the control panel or the display panel?
                    Panel p = c as Panel;

                    // The display panel will have 0 controls.
                    // The control panel will have separate, of which only a few are mandatory
                    int controlsFound = 0;
                    bool foundFire = false;
                    bool foundAngle = false;
                    bool foundAngleLabel = false;
                    bool foundPower = false;
                    bool foundPowerLabel = false;


                    foreach (Control pc in p.Controls)
                    {
                        controlsFound++;

                        // Mandatory controls for the control panel are:
                        // A 'Fire!' button
                        // A NumericUpDown for controlling the angle
                        // A TrackBar for controlling the power
                        // "Power:" and "Angle:" labels

                        if (pc is Label)
                        {
                            Label lbl = pc as Label;
                            if (lbl.Text.ToLower().Contains("angle"))
                            {
                                foundAngleLabel = true;
                            }
                            else
                            if (lbl.Text.ToLower().Contains("power"))
                            {
                                foundPowerLabel = true;
                            }
                        }
                        else
                        if (pc is Button)
                        {
                            Button btn = pc as Button;
                            if (btn.Text.ToLower().Contains("fire"))
                            {
                                foundFire = true;
                                fireCtrl = btn;
                            }
                        }
                        else
                        if (pc is TrackBar)
                        {
                            foundPower = true;
                            powerCtrl = pc as TrackBar;
                        }
                        else
                        if (pc is NumericUpDown)
                        {
                            foundAngle = true;
                            angleCtrl = pc as NumericUpDown;
                        }
                        else
                        if (pc is ListBox)
                        {
                            weaponSelect = pc as ListBox;
                        }
                    }

                    if (controlsFound == 0)
                    {
                        foundDisplayPanel = true;
                    }
                    else
                    {
                        if (!foundFire)
                        {
                            SetErrorDescription("Control panel lacks a \"Fire!\" button OR the display panel incorrectly contains controls");
                            return null;
                        }
                        else
                        if (!foundAngle)
                        {
                            SetErrorDescription("Control panel lacks an angle NumericUpDown OR the display panel incorrectly contains controls");
                            return null;
                        }
                        else
                        if (!foundPower)
                        {
                            SetErrorDescription("Control panel lacks a power TrackBar OR the display panel incorrectly contains controls");
                            return null;
                        }
                        else
                        if (!foundAngleLabel)
                        {
                            SetErrorDescription("Control panel lacks an \"Angle:\" label OR the display panel incorrectly contains controls");
                            return null;
                        }
                        else
                        if (!foundPowerLabel)
                        {
                            SetErrorDescription("Control panel lacks a \"Power:\" label OR the display panel incorrectly contains controls");
                            return null;
                        }

                        foundControlPanel = true;
                        controlPanel = p;
                    }

                }
                else
                {
                    SetErrorDescription(string.Format("Unexpected control ({0}) named \"{1}\" found in GameForm", c.GetType().FullName, c.Name));
                    return null;
                }
            }

            if (!foundDisplayPanel)
            {
                SetErrorDescription("No display panel found");
                return null;
            }
            if (!foundControlPanel)
            {
                SetErrorDescription("No control panel found");
                return null;
            }
            return gameplayForm;
        }

        private static bool TestGameForm0GameForm()
        {
            NumericUpDown angle;
            TrackBar power;
            Button fire;
            Panel controlPanel;
            ListBox weaponSelect;
            GameForm gameplayForm = InitialiseGameForm(out angle, out power, out fire, out controlPanel, out weaponSelect);

            if (gameplayForm == null) return false;

            return true;
        }
        private static bool TestGameForm0EnableTankButtons()
        {
            Requires(TestGameForm0GameForm);
            GameController game = InitialiseGame();
            game.StartGame();

            // Find the gameplay form
            GameForm gameplayForm = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f is GameForm)
                {
                    gameplayForm = f as GameForm;
                }
            }
            if (gameplayForm == null)
            {
                SetErrorDescription("Gameplay form was not created by GameController.StartGame()");
                return false;
            }

            // Find the control panel
            Panel controlPanel = null;
            foreach (Control c in gameplayForm.Controls)
            {
                if (c is Panel)
                {
                    foreach (Control cc in c.Controls)
                    {
                        if (cc is NumericUpDown || cc is Label || cc is TrackBar)
                        {
                            controlPanel = c as Panel;
                        }
                    }
                }
            }

            if (controlPanel == null)
            {
                SetErrorDescription("Control panel was not found in GameForm");
                return false;
            }

            // Disable the control panel to check that EnableControlPanel enables it
            controlPanel.Enabled = false;

            gameplayForm.EnableTankButtons();

            if (!controlPanel.Enabled)
            {
                SetErrorDescription("Control panel is still disabled after GameForm.EnableTankButtons()");
                return false;
            }
            return true;

        }
        private static bool TestGameForm0Aim()
        {
            Requires(TestGameForm0GameForm);
            NumericUpDown angle;
            TrackBar power;
            Button fire;
            Panel controlPanel;
            ListBox weaponSelect;
            GameForm gameplayForm = InitialiseGameForm(out angle, out power, out fire, out controlPanel, out weaponSelect);

            if (gameplayForm == null) return false;

            float testAngle = 27;

            gameplayForm.Aim(testAngle);
            if (FloatEquals((float)angle.Value, testAngle)) return true;

            else
            {
                SetErrorDescription(string.Format("Attempted to set angle to {0} but angle is {1}", testAngle, (float)angle.Value));
                return false;
            }
        }
        private static bool TestGameForm0SetPower()
        {
            Requires(TestGameForm0GameForm);
            NumericUpDown angle;
            TrackBar power;
            Button fire;
            Panel controlPanel;
            ListBox weaponSelect;
            GameForm gameplayForm = InitialiseGameForm(out angle, out power, out fire, out controlPanel, out weaponSelect);

            if (gameplayForm == null) return false;

            int testPower = 71;

            gameplayForm.SetPower(testPower);
            if (power.Value == testPower) return true;

            else
            {
                SetErrorDescription(string.Format("Attempted to set power to {0} but power is {1}", testPower, power.Value));
                return false;
            }
        }
        private static bool TestGameForm0SetWeaponIndex()
        {
            Requires(TestGameForm0GameForm);
            NumericUpDown angle;
            TrackBar power;
            Button fire;
            Panel controlPanel;
            ListBox weaponSelect;
            GameForm gameplayForm = InitialiseGameForm(out angle, out power, out fire, out controlPanel, out weaponSelect);

            if (gameplayForm == null) return false;

            gameplayForm.SetWeaponIndex(0);

            // WeaponSelect is optional behaviour, so it's okay if it's not implemented here, as long as the method works.
            return true;
        }
        private static bool TestGameForm0Fire()
        {
            Requires(TestGameForm0GameForm);
            // This is something we can't really test properly without a proper framework, so for now we'll just click
            // the button and make sure it disables the control panel
            NumericUpDown angle;
            TrackBar power;
            Button fire;
            Panel controlPanel;
            ListBox weaponSelect;
            GameForm gameplayForm = InitialiseGameForm(out angle, out power, out fire, out controlPanel, out weaponSelect);

            controlPanel.Enabled = true;
            fire.PerformClick();
            if (controlPanel.Enabled)
            {
                SetErrorDescription("Control panel still enabled immediately after clicking fire button");
                return false;
            }

            return true;
        }
        private static void UnitTests()
        {
            DoTest(TestGameController0GameController);
            DoTest(TestGameController0NumPlayers);
            DoTest(TestGameController0GetTotalRounds);
            DoTest(TestGameController0RegisterPlayer);
            DoTest(TestGameController0GetPlayerNumber);
            DoTest(TestGameController0TankColour);
            DoTest(TestGameController0GetPlayerPositions);
            DoTest(TestGameController0RandomReorder);
            DoTest(TestGameController0StartGame);
            DoTest(TestGameController0GetBattlefield);
            DoTest(TestGameController0CurrentPlayerTank);
            DoTest(TestChassis0CreateTank);
            DoTest(TestChassis0DisplayTankSprite);
            DoTest(TestChassis0DrawLine);
            DoTest(TestChassis0GetTankArmour);
            DoTest(TestChassis0Weapons);
            DoTest(TestTankController0HumanOpponent);
            DoTest(TestTankController0CreateTank);
            DoTest(TestTankController0PlayerName);
            DoTest(TestTankController0GetColour);
            DoTest(TestTankController0WonRound);
            DoTest(TestTankController0GetScore);
            DoTest(TestHumanOpponent0StartRound);
            DoTest(TestHumanOpponent0CommenceTurn);
            DoTest(TestHumanOpponent0ReportHit);
            DoTest(TestBattleTank0BattleTank);
            DoTest(TestBattleTank0GetPlayerNumber);
            DoTest(TestBattleTank0CreateTank);
            DoTest(TestBattleTank0GetAim);
            DoTest(TestBattleTank0Aim);
            DoTest(TestBattleTank0GetCurrentPower);
            DoTest(TestBattleTank0SetPower);
            DoTest(TestBattleTank0GetWeaponIndex);
            DoTest(TestBattleTank0SetWeaponIndex);
            DoTest(TestBattleTank0Draw);
            DoTest(TestBattleTank0XPos);
            DoTest(TestBattleTank0YPos);
            DoTest(TestBattleTank0Fire);
            DoTest(TestBattleTank0InflictDamage);
            DoTest(TestBattleTank0Alive);
            DoTest(TestBattleTank0Gravity);
            DoTest(TestMap0Map);
            DoTest(TestMap0TerrainAt);
            DoTest(TestMap0CheckTankCollision);
            DoTest(TestMap0TankYPosition);
            DoTest(TestMap0DestroyTerrain);
            DoTest(TestMap0Gravity);
            DoTest(TestEffect0SetCurrentGame);
            DoTest(TestBullet0Bullet);
            DoTest(TestBullet0Step);
            DoTest(TestBullet0Draw);
            DoTest(TestBoom0Boom);
            DoTest(TestBoom0Explode);
            DoTest(TestBoom0Step);
            DoTest(TestBoom0Draw);
            DoTest(TestGameForm0GameForm);
            DoTest(TestGameForm0EnableTankButtons);
            DoTest(TestGameForm0Aim);
            DoTest(TestGameForm0SetPower);
            DoTest(TestGameForm0SetWeaponIndex);
            DoTest(TestGameForm0Fire);
        }
        
        #endregion
        
        #region CheckClasses

        private static bool CheckClasses()
        {
            string[] classNames = new string[] { "Program", "ComputerOpponent", "Map", "Boom", "GameForm", "GameController", "HumanOpponent", "Bullet", "TankController", "BattleTank", "Chassis", "Effect" };
            string[][] classFields = new string[][] {
                new string[] { "Main" }, // Program
                new string[] { }, // ComputerOpponent
                new string[] { "TerrainAt","CheckTankCollision","TankYPosition","DestroyTerrain","Gravity","WIDTH","HEIGHT"}, // Map
                new string[] { "Explode" }, // Boom
                new string[] { "EnableTankButtons","Aim","SetPower","SetWeaponIndex","Fire","InitialiseBuffer"}, // GameForm
                new string[] { "NumPlayers","GetRound","GetTotalRounds","RegisterPlayer","GetPlayerNumber","GetGameplayTank","TankColour","GetPlayerPositions","RandomReorder","StartGame","NewRound","GetBattlefield","DrawTanks","CurrentPlayerTank","AddEffect","WeaponEffectTick","DrawAttacks","RemoveEffect","CheckCollidedTank","InflictDamage","Gravity","TurnOver","ScoreWinner","NextRound","GetWind"}, // GameController
                new string[] { }, // HumanOpponent
                new string[] { }, // Bullet
                new string[] { "CreateTank","PlayerName","GetColour","WonRound","GetScore","StartRound","CommenceTurn","ReportHit"}, // TankController
                new string[] { "GetPlayerNumber","CreateTank","GetAim","Aim","GetCurrentPower","SetPower","GetWeaponIndex","SetWeaponIndex","Draw","XPos","YPos","Fire","InflictDamage","Alive","Gravity"}, // BattleTank
                new string[] { "DisplayTankSprite","DrawLine","CreateTankBitmap","GetTankArmour","Weapons","ShootWeapon","CreateTank","WIDTH","HEIGHT","NUM_TANKS"}, // Chassis
                new string[] { "SetCurrentGame","Step","Draw"} // Effect
            };

            Assembly assembly = Assembly.GetExecutingAssembly();

            Console.WriteLine("Checking classes for public methods...");
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsPublic)
                {
                    if (type.Namespace != "TankBattle")
                    {
                        Console.WriteLine("Public type {0} is not in the TankBattle namespace.", type.FullName);
                        return false;
                    }
                    else
                    {
                        int typeIdx = -1;
                        for (int i = 0; i < classNames.Length; i++)
                        {
                            if (type.Name == classNames[i])
                            {
                                typeIdx = i;
                                classNames[typeIdx] = null;
                                break;
                            }
                        }
                        foreach (MemberInfo memberInfo in type.GetMembers())
                        {
                            string memberName = memberInfo.Name;
                            bool isInherited = false;
                            foreach (MemberInfo parentMemberInfo in type.BaseType.GetMembers())
                            {
                                if (memberInfo.Name == parentMemberInfo.Name)
                                {
                                    isInherited = true;
                                    break;
                                }
                            }
                            if (!isInherited)
                            {
                                if (typeIdx != -1)
                                {
                                    bool fieldFound = false;
                                    if (memberName[0] != '.')
                                    {
                                        foreach (string allowedFields in classFields[typeIdx])
                                        {
                                            if (memberName == allowedFields)
                                            {
                                                fieldFound = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        fieldFound = true;
                                    }
                                    if (!fieldFound)
                                    {
                                        Console.WriteLine("The public field \"{0}\" is not one of the authorised fields for the {1} class.\n", memberName, type.Name);
                                        Console.WriteLine("Remove it or change its access level.");
                                        return false;
                                    }
                                }
                            }
                        }
                    }

                    //Console.WriteLine("{0} passed.", type.FullName);
                }
            }
            for (int i = 0; i < classNames.Length; i++)
            {
                if (classNames[i] != null)
                {
                    Console.WriteLine("The class \"{0}\" is missing.", classNames[i]);
                    return false;
                }
            }
            Console.WriteLine("All public methods okay.");
            return true;
        }
        
        #endregion

        public static void Main()
        {
            if (CheckClasses())
            {
                UnitTests();

                int passed = 0;
                int failed = 0;
                foreach (string key in unitTestResults.Keys)
                {
                    if (unitTestResults[key] == "Passed")
                    {
                        passed++;
                    }
                    else
                    {
                        failed++;
                    }
                }

                Console.WriteLine("\n{0}/{1} unit tests passed", passed, passed + failed);
                if (failed == 0)
                {
                    Console.WriteLine("Starting up TankBattle...");
                    Program.Main();
                    return;
                }
            }

            Console.WriteLine("\nPress enter to exit.");
            Console.ReadLine();
        }
    }
}
