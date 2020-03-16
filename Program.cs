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
            MyImage image = new MyImage("Image\\coco.bmp");
            //Process.Start("Image\\lac.bmp");
            image.Convolution();
            image.From_Image_To_File();
            Process.Start("Image\\copie_coco.bmp");
            //Console.ReadKey();
        }
    }
}
