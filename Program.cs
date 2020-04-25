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
using ReedSolomon;
using System.Collections;

namespace Projet_Info_S4
{
    public class Program
    {
        static void Main()
        {
            /*byte[] fichier = File.ReadAllBytes("Images\\lac.bmp");

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
            //MyImage image = MyImage.CréerMyImage(500,500);
            MyImage image = new MyImage("Images\\lac.bmp");
            //MyImage histo = image.Histogramme();
            //Process.Start("Image\\lac.bmp");
            //histo.Histogramme();
            image = image.Rotation(-180);
            image.From_Image_To_File();
            Process.Start("Images\\rotation_copie.bmp");

            //Process.Start("Image\\lac_copie.bmp");
            //Console.ReadKey();



            /*Encoding u8 = Encoding.UTF8;
            string a = "HELLO WORD";
            int iBC = u8.GetByteCount(a);
            byte[] bytesa = u8.GetBytes(a);
            string b = "HELLO WORF";
            byte[] bytesb = u8.GetBytes(b);
            //byte[] result = ReedSolomonAlgorithm.Encode(bytesa, 7);
            //Privilégiez l'écriture suivante car par défaut le type choisi est DataMatrix 
            byte[] result = ReedSolomonAlgorithm.Encode(bytesa, 7, ErrorCorrectionCodeType.QRCode);
            byte[] result1 = ReedSolomonAlgorithm.Decode(bytesb, result);
            foreach (byte val in a) Console.Write(val + " ");
            Console.WriteLine();



            QR test = new QR("HELLO WORLD");
            for(int i = 0; i < test.Donnees.Length-11; i+=11)
            {
                for(int j = 0; j < 11; j++)
                {
                    Console.Write(test.Donnees[i + j]);
                }
                Console.WriteLine();
            }
            Console.ReadLine();


            string phrase = "01234567890123456789012345";
            phrase = phrase.ToUpper();
            QR test = new QR(phrase);


            for (int i = 0; i < test.Donnees.Length; i += 8)
            {
                for (int j = i; j < i + 8; j++)
                {
                    Console.Write(test.Donnees[j]);

                }
                Console.WriteLine();
            }
            Console.ReadKey();*/
        }
    }
}
