using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Media;
using System.ComponentModel;

namespace Projet_Info_S4
{
    public class Pixel
    {
        //Attributs
        private byte r;
        private byte g;
        private byte b;
        //Constructeur
        /// <summary>
        /// Cette fonction est le constructeur de la classe Pixel. Un pixel est constitué de trois couleurs : rouge, vert et bleu, c'est pourquoi il est caractérisé par trois attributs en octet
        /// </summary>
        /// <param name="r">octet permettant d'attribuer les nuances de rouge au pixel</param>
        /// <param name="g">octet permettant d'attribuer les nuances de vert au pixel</param>
        /// <param name="b">octet permettant d'attribuer les nuances de bleu au pixel</param>
        public Pixel(byte r, byte g, byte b)
        {
            if (r <= 255 || r >= 0 || g <= 255 || g >= 0 || b <= 255 || b >= 0)
            {
                this.r = r;
                this.g = g;
                this.b = b;
            }
            else { Console.WriteLine("Valeur(s) en octet incorrecte(s) pour la création du pixel"); }
        }
        //Propriétés
        /// <summary>
        /// Propriété pour récupérer ou modifier la valeur de l'octet codant les nuances de rouge
        /// </summary>
        public byte R
        {
            get { return this.r; }
            set { this.r = value; }
        }
        /// <summary>
        /// Propriété pour récupérer ou modifier la valeur de l'octet codant les nuances de vert
        /// </summary>
        public byte G
        {
            get { return this.g; }
            set { this.g = value; }
        }
        /// <summary>
        /// Propriété pour récupérer ou modifier la valeur de l'octet codant les nuances de bleu
        /// </summary>
        public byte B
        {
            get { return this.b; }
            set { this.b = value; }
        }
    }
}
