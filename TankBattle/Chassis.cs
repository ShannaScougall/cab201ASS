using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankBattle
{
    public abstract class Chassis
    {
        public const int WIDTH = 4;
        public const int HEIGHT = 3;
        public const int NUM_TANKS = 1;

        public abstract int[,] DisplayTankSprite(float angle);

        public static void DrawLine(int[,] graphic, int X1, int Y1, int X2, int Y2) {
            
            int dx = X2 - X1;
            int dy = Y2 - Y1;
            double d_error = Math.Abs(dy / dx);
            double error = 0.0;
            int y = Y1;

            if (X1 < X2) {
                for (int x = X1; x <= X2; x++) {
                    graphic[x, y] = 1;
                    error = error + d_error;
                    if (error >= 0.5) {
                        y = y + 1;
                        error--;
                    }
                }
            } else if (X1 > X2) {
                for (int x = X1; x >= X2; x--) {
                    graphic[x, y] = 1;
                    error = error + d_error;
                    if (error >= 0.5) {
                        y = y + 1;
                        error--;
                    }
                }
            }

        }

        public Bitmap CreateTankBitmap(Color tankColour, float angle)
        {
            int[,] tankGraphic = DisplayTankSprite(angle);
            int height = tankGraphic.GetLength(0);
            int width = tankGraphic.GetLength(1);

            Bitmap bmp = new Bitmap(width, height);
            Color transparent = Color.FromArgb(0, 0, 0, 0);
            Color tankOutline = Color.FromArgb(255, 0, 0, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (tankGraphic[y, x] == 0)
                    {
                        bmp.SetPixel(x, y, transparent);
                    }
                    else
                    {
                        bmp.SetPixel(x, y, tankColour);
                    }
                }
            }

            // Outline each pixel
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    if (tankGraphic[y, x] != 0)
                    {
                        if (tankGraphic[y - 1, x] == 0)
                            bmp.SetPixel(x, y - 1, tankOutline);
                        if (tankGraphic[y + 1, x] == 0)
                            bmp.SetPixel(x, y + 1, tankOutline);
                        if (tankGraphic[y, x - 1] == 0)
                            bmp.SetPixel(x - 1, y, tankOutline);
                        if (tankGraphic[y, x + 1] == 0)
                            bmp.SetPixel(x + 1, y, tankOutline);
                    }
                }
            }

            return bmp;
        }

        public abstract int GetTankArmour();

        public abstract string[] Weapons();

        public abstract void ShootWeapon(int weapon, BattleTank playerTank, GameController currentGame);

        public static Chassis CreateTank(int tankNumber)
        {
            throw new NotImplementedException();
        }
    }
}
