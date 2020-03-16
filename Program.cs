using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Media;
using System.ComponentModel;
using System.IO;

namespace Projet_Info_S4
{
    public class Program
    {
        static void Main()
        {
            /*  byte[] fichier = File.ReadAllBytes("Image\\ImageCopie.bmp");

            Console.WriteLine("\n Header \n");
            for (int i = 0; i < 14; i++)
            {
                Console.Write(fichier[i] + " ");
            }
            Console.WriteLine("\n Header info \n");
            for (int i = 14; i < 54; i++)
            {
                Console.Write(fichier[i] + " ");
            }
            Console.ReadKey();

            Console.WriteLine("\n Image \n");
              for (int i = 54; i < fichier.Length; i +=60)
              {
                  for (int j = i; j<i+60; j++)
                  {
                      Console.Write(fichier[j] + " ");
                  }
                  Console.WriteLine();
              }*/
            MyImage lac = new MyImage("Image\\lena.bmp");
            Process.Start("Image\\lena.bmp");
            lac.Convolution();
            lac.From_Image_To_File();
            Process.Start("Image\\copie_lena.bmp");
            Console.ReadKey();
        }
        static void TestConvertionNB()
        {
            Process.Start("Image\\lac.bmp");
            Bitmap b = new Bitmap("Image\\lac.bmp");
            for (int i = 0; i < b.Height; i++)
            {
                for (int j = 0; j < b.Width; j++)
                {
                    Color mycolor = b.GetPixel(j, i);
                    int a = (mycolor.R + mycolor.G + mycolor.B) / 3;
                    int couleur = a > 127 ? 0 : 255;
                    b.SetPixel(j, i, Color.FromArgb(couleur, couleur, couleur));
                }
            }
            b.Save("test1.bmp");
            Process.Start("test1.bmp");
            Console.ReadLine();
        }
        static void TestConvertionNuanceGris()
        {
            Process.Start("Image\\lac.bmp");
            Bitmap b = new Bitmap("Image\\lac.bmp");
            for (int i = 0; i < b.Height; i++)
            {
                for (int j = 0; j < b.Width; j++)
                {
                    Color mycolor = b.GetPixel(j, i);
                    int a = (mycolor.R + mycolor.G + mycolor.B) / 3;
                    b.SetPixel(j, i, Color.FromArgb(a, a, a));
                }
            }
            b.Save("test2.bmp");
            Process.Start("test2.bmp");
            Console.ReadLine();
        }
    }
}
