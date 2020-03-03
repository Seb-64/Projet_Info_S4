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
    public class MyImage
    {
        //Attributs
        private string nomFichier;
        private string type;
        private int tailleFichier;
        private int tailleOffset;
        private int hauteur;
        private int largeur;
        private int nbBitsCouleur;
        private Pixel[,] image;
        //Constructeurs
        public MyImage(string nomFichier)
        {
            if (File.Exists(nomFichier))
            {
                this.nomFichier = nomFichier;
                byte[] fichier = File.ReadAllBytes(nomFichier);
                char one = Convert.ToChar(fichier[0]);          //Pb pour .csv (à régler)
                char two = Convert.ToChar(fichier[1]);
                this.type = one.ToString() + two.ToString();
                byte[] tabTaille = { fichier[2], fichier[3], fichier[4], fichier[5] };
                byte[] tabOffset = { fichier[10], fichier[11], fichier[12], fichier[13] };
                this.tailleFichier = Convertir_Endian_To_Int(tabTaille);
                this.tailleOffset = Convertir_Endian_To_Int(tabOffset);
                byte[] tabLargeur = { fichier[18], fichier[19], fichier[20], fichier[21] };
                byte[] tabHauteur = { fichier[22], fichier[23], fichier[24], fichier[25] };
                this.hauteur = Convertir_Endian_To_Int(tabHauteur);
                this.largeur = Convertir_Endian_To_Int(tabLargeur);
                byte[] tabNbBitsCouleur = { fichier[28], fichier[29] };
                this.nbBitsCouleur = Convertir_Endian_To_Int(tabNbBitsCouleur);
                this.image = new Pixel[hauteur, largeur];
                int k = tailleOffset;
                for (int ligne = 0; ligne < hauteur; ligne++)
                {
                    for (int colonne = 0; colonne < largeur; colonne++)
                    {
                        this.image[ligne, colonne] = new Pixel(fichier[k], fichier[k + 1], fichier[k + 2]);
                        k += 3;
                    }
                }
            }
            else { Console.WriteLine("Fichier inexistant"); }
        }
        public MyImage(MyImage aCopier)
        {
            this.nomFichier = aCopier.nomFichier.Insert(6, "copie.");
            this.type = aCopier.type;
            this.tailleFichier = aCopier.tailleFichier;
            this.tailleOffset = aCopier.tailleOffset;
            this.hauteur = aCopier.hauteur;
            this.largeur = aCopier.largeur;
            this.nbBitsCouleur = aCopier.nbBitsCouleur;
            this.image = aCopier.image;
        }
        public MyImage() { }

        //Méthode
        /// <summary>
        /// Cette fonction convertit un tableau d'octets de taille 4 au format little Endian en un nombre entier
        /// </summary>
        /// <param name="tab">Ce tableau contient les 4 octets qui nous interessent</param>
        /// <returns>Retourne un entier correspondant à la conversion</returns>
        public int Convertir_Endian_To_Int(byte[] tab)
        {
            double result = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                result += tab[i] * Math.Pow(2, 8 * i);
            }
            return Convert.ToInt32(result);
        }
        /// <summary>
        /// Cette fonction convertit un nombre entier en un tableau d'octets de taille 4 au format little Endian
        /// </summary>
        /// <param name="valeur">Valeur à convertir en little Endian</param>
        /// <returns>Retourne un tableau d'octets de taille 4 correspondant à la conversion</returns>
        public byte[] Convertir_Int_To_Endian(int valeur)
        {
            byte[] resultat = new byte[4];
            long reste = 0;
            long quotient = 0;
            for (int i = 0; i < resultat.Length; i++)
            {
                quotient = valeur / Convert.ToInt64(Math.Pow(2, 8 * (3 - i)));
                reste = valeur - quotient * Convert.ToInt64(Math.Pow(2, 8 * (3 - i)));
                valeur = Convert.ToInt32(reste);
                resultat[3 - i] = (byte)Convert.ToByte(quotient);
            }
            return resultat;
        }
        /// <summary>
        /// Fonction -1 du constructeur : on créé un fichier avec autant d'octet que le fichier précédent, on les initialise tous à 0 puis on attribut les valeurs au header aux octets spécifiques. Enfin, on ajoute les datas des pixels de l'image codés sur 3 octets.
        /// </summary>
        public void From_Image_To_File()
        {
            byte[] fichier = new byte[this.tailleFichier];
            for (int i = 0; i < 54; i++)
            {
                fichier[i] = (byte)0; //on attribut tout d'abord une valeur aux header et header info
            }

            byte[] tabType = { Convert.ToByte(this.type[0]), Convert.ToByte(this.type[1]) };
            fichier[0] = tabType[0]; fichier[1] = tabType[1];

            byte[] tabTaille = Convertir_Int_To_Endian(this.tailleFichier);
            fichier[2] = tabTaille[0]; fichier[3] = tabTaille[1]; fichier[4] = tabTaille[2]; fichier[5] = tabTaille[3];

            byte[] tabTailleOffset = Convertir_Int_To_Endian(this.tailleOffset);
            fichier[10] = tabTailleOffset[0]; fichier[11] = tabTailleOffset[1]; fichier[12] = tabTailleOffset[2]; fichier[13] = tabTailleOffset[3];

            fichier[14] = 40;

            byte[] tabLargeur = Convertir_Int_To_Endian(this.largeur);
            fichier[18] = tabLargeur[0]; fichier[19] = tabLargeur[1]; fichier[20] = tabLargeur[2]; fichier[21] = tabLargeur[3];

            byte[] tabHauteur = Convertir_Int_To_Endian(this.hauteur);
            fichier[22] = tabHauteur[0]; fichier[23] = tabHauteur[1]; fichier[24] = tabHauteur[2]; fichier[25] = tabHauteur[3];

            byte[] tabNbrBitCouleur = Convertir_Int_To_Endian(this.nbBitsCouleur);
            fichier[28] = tabNbrBitCouleur[0]; fichier[29] = tabNbrBitCouleur[1];

            int t = 54;

            for (int l = 0; l < this.hauteur; l++)
            {
                for (int c = 0; c < this.largeur; c++)
                {
                    fichier[t] = this.image[l, c].R;    //on attribue les octets des pixel R, G et B à la matrice de byte
                    fichier[t + 1] = this.image[l, c].G;
                    fichier[t + 2] = this.image[l, c].B;
                    t += 3;
                }
            }
            string nouveauNom = this.nomFichier.Insert(6, "copie_");
            File.WriteAllBytes(nouveauNom, fichier);
        }
        /// <summary>
        /// Changement des valeurs RGB de l'image : on effectue la moyenne sur chaque couleurs des pixels puis on leur attribut cette valeur
        /// </summary>
        public void Couleur_To_Nuance_Gris()
        {
            for (int ligne = 0; ligne < hauteur; ligne++)
            {
                for (int colonne = 0; colonne < largeur; colonne++)
                {
                    byte nuanceGris = Convert.ToByte((this.image[ligne, colonne].R + this.image[ligne, colonne].G + this.image[ligne, colonne].B) / 3);
                    this.image[ligne, colonne].R = nuanceGris;
                    this.image[ligne, colonne].G = nuanceGris;
                    this.image[ligne, colonne].B = nuanceGris;
                }
            }
        }
        /// <summary>
        /// Changement des valeurs RGB de l'image : si la moyenne est supérieure à 127, le pixel devient noir, sinon le pixel devient blanc
        /// </summary>
        public void Couleur_Noir_Blanc()
        {
            for (int l = 0; l < hauteur; l++)
            {
                for (int c = 0; c < largeur; c++)
                {
                    byte moy = Convert.ToByte((this.image[l, c].R + this.image[l, c].G + this.image[l, c].B) / 3);
                    byte result = (byte)(moy > 127 ? 0 : 255);
                    this.image[l, c].R = result;
                    this.image[l, c].G = result;
                    this.image[l, c].B = result;
                }
            }
        }
        /// <summary>
        /// Fonction qui fait un effet miroir sur la photo : les pixels à gauche du milieu vont passer de l'autre côté du milieu de la photo.
        /// On prend en mémoire tout d'abord le pixel à droite qui va être remplacé, puis ce dernier devient le pixel correspondant à droite et enfin, le pixel à gauche prend la valeur prise en mémoire au début de l'algo.
        /// On répète cette action pour tous les pixels à gauche du milieu de la photo.
        /// </summary>
        public void Effet_Miroir()
        {
            for (int ligne = 0; ligne < hauteur; ligne++)
            {
                for (int colonne = 0; colonne < largeur / 2; colonne++)
                {
                    Pixel pixelTempo = this.image[ligne, largeur - colonne - 1];
                    this.image[ligne, largeur - colonne - 1] = this.image[ligne, colonne];
                    this.image[ligne, colonne] = pixelTempo;
                }
            }
        }
        public void Rotation()
        {

        }
    }
}
