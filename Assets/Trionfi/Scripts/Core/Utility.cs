using UnityEngine;
namespace NovelEx
{
    public class TRUtility
    {
        private static string GetHex(int num)
        {
            const string alpha = "0123456789ABCDEF";
            string ret = "" + alpha[num];
            return ret;
        }

        private static int HexToInt(char hexChar)
        {
            switch (hexChar)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'A': return 10;
                case 'B': return 11;
                case 'C': return 12;
                case 'D': return 13;
                case 'E': return 14;
                case 'F': return 15;
            }
            return -1;
        }

        public static string RGBToHex(Color color)
        {
            float red = color.r * 255;
            float green = color.g * 255;
            float blue = color.b * 255;

            string a = GetHex(Mathf.FloorToInt(red / 16));
            string b = GetHex(Mathf.RoundToInt(red) % 16);
            string c = GetHex(Mathf.FloorToInt(green / 16));
            string d = GetHex(Mathf.RoundToInt(green) % 16);
            string e = GetHex(Mathf.FloorToInt(blue / 16));
            string f = GetHex(Mathf.RoundToInt(blue) % 16);

            return a + b + c + d + e + f;
        }

        public static Color HexToRGB(string color)
        {
            color = color.Replace("#", "");

            float red = (HexToInt(color[1]) + HexToInt(color[0]) * 16f) / 255f;
            float green = (HexToInt(color[3]) + HexToInt(color[2]) * 16f) / 255f;
            float blue = (HexToInt(color[5]) + HexToInt(color[4]) * 16f) / 255f;
            Color finalColor = new Color { r = red, g = green, b = blue, a = 1 };
            return finalColor;
        }

        public static Color HexToARGB(string color)
        {
            color = color.Replace("#", "");

            float alpha = (HexToInt(color[1]) + HexToInt(color[0]) * 16f) / 255f;
            float red = (HexToInt(color[3]) + HexToInt(color[2]) * 16f) / 255f;
            float green = (HexToInt(color[5]) + HexToInt(color[4]) * 16f) / 255f;
            float blue = (HexToInt(color[7]) + HexToInt(color[6]) * 16f) / 255f;
            Color finalColor = new Color { r = red, g = green, b = blue, a = alpha };
            return finalColor;
        }
    }
}
