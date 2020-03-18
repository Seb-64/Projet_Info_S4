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
            string nouveauNom = this.nomFichier.Replace(".bmp", "_copie.bmp");
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
        public void Agrandissement(int coef)
        {    //coef d'agrandissement à choisir (chiffre rond)
            Pixel[,] ImageAgrandie = new Pixel[this.hauteur * coef, this.largeur * coef];    //on crée une nouvelle matrice de pixel avec la taille souhaitée

            int ll = 0;
            int cc = 0;
            for (int l = 0; l < ImageAgrandie.GetLength(0); l += coef)
            {
                for (int c = 0; c < ImageAgrandie.GetLength(1); c += coef)
                {
                    for (int m = 0; m < coef; m++)
                    {
                        for (int n = 0; n < coef; n++)
                        {
                            ImageAgrandie[l + m, c + n] = this.image[ll, cc];        //on initialise les pixels supplémentaires à la même valeur que le pixel original
                        }

                    }
                    cc++;
                }
                cc = 0;
                ll++;
            }

            this.hauteur = ImageAgrandie.GetLength(0);
            this.largeur = ImageAgrandie.GetLength(1);

            this.tailleFichier = ((this.hauteur * this.largeur * 3) + 56);  //on remet à jour la taille du fichier car elle a augmenté
            this.image = ImageAgrandie;
        }
        public void Retrecissement()
        {
            int coef = 0;
            int[] coefpossibles = new int[10];
            int t = 0;
            for (int i = 2; i < 11; i++)          //on  cherche seulement des coefs compris entre 2 et 10
            {
                if ((this.hauteur / i) % 4 == 0 && (this.largeur / i) % 4 == 0) //on test si la largeur et la hauteur est bien  multiple de 4 (nécessaire)
                {
                    coefpossibles[t] = i;               //fonction qui determine quels coeffiscients de retrécissement on peut utiliser
                    t++;                                //car problèmes avec certains coef (largeur % 4 != 0 donc fichier bmp corrompu)
                }
            }

            Console.WriteLine("Voici les différents coefficients de rétrecissement possible:");
            for (int i = 0; i < 10; i++)
            {
                if (coefpossibles[i] != 0)
                {
                    Console.WriteLine(coefpossibles[i] + "; ");             //on propose à l'utilisateur quel coeffiscient il veut utiliser (!=0)
                }

            }

            bool coefok = false;
            while (!coefok)
            {
                Console.WriteLine("Veuillez choisir un coefficient valide puis valider svp.");
                coef = Convert.ToInt32(Console.ReadLine());

                for (int i = 0; i < coefpossibles.Length; i++)
                {
                    if (coefpossibles[i] == coef)
                    {
                        coefok = true;          //vérification si le coef entré par l'utilisateur est bien valide
                    }

                }
            }
            //une fois le coef choisi, on suit le même protocole que pour l'agrandissement

            Pixel[,] imageRetreci = new Pixel[this.hauteur / coef, this.largeur / coef];

            int ll = 0;
            int cc = 0;
            for (int l = 0; l < imageRetreci.GetLength(0); l++)
            {
                for (int c = 0; c < imageRetreci.GetLength(1); c++)
                {
                    imageRetreci[l, c] = this.image[ll, cc];                //Fonction qui rétrecie l'image en fonction du coef que l'utilisateur a rentré
                    cc += coef;
                }
                cc = 0;
                ll += coef;
            }

            this.hauteur = imageRetreci.GetLength(0);
            this.largeur = imageRetreci.GetLength(1);

            this.tailleFichier = ((this.hauteur * this.largeur * 3) + 56);
            this.image = imageRetreci;

        }
        public void Rotation(int angle)
        {
            double teta = angle * Math.PI / 180;

            int newLargeur = Convert.ToInt32(this.largeur * Math.Cos(teta) + this.hauteur * Math.Sin(teta));
            int newHauteur = Convert.ToInt32(this.largeur * Math.Sin(teta) + this.hauteur * Math.Cos(teta));
            int newTailleFichier = newHauteur * newLargeur;
            byte[] fichier = new byte[newTailleFichier];
            for (int i = 0; i < 54; i++)
            {
                fichier[i] = (byte)0; //on attribut tout d'abord une valeur aux header et header info
            }

            byte[] tabType = { Convert.ToByte(this.type[0]), Convert.ToByte(this.type[1]) };
            fichier[0] = tabType[0]; fichier[1] = tabType[1];
            byte[] tabTaille = Convertir_Int_To_Endian(newTailleFichier);
            fichier[2] = tabTaille[0]; fichier[3] = tabTaille[1]; fichier[4] = tabTaille[2]; fichier[5] = tabTaille[3];
            byte[] tabTailleOffset = Convertir_Int_To_Endian(this.tailleOffset);
            fichier[10] = tabTailleOffset[0]; fichier[11] = tabTailleOffset[1]; fichier[12] = tabTailleOffset[2]; fichier[13] = tabTailleOffset[3];
            fichier[14] = 40;
            byte[] tabLargeur = Convertir_Int_To_Endian(newLargeur);
            fichier[18] = tabLargeur[0]; fichier[19] = tabLargeur[1]; fichier[20] = tabLargeur[2]; fichier[21] = tabLargeur[3];
            byte[] tabHauteur = Convertir_Int_To_Endian(newHauteur);
            fichier[22] = tabHauteur[0]; fichier[23] = tabHauteur[1]; fichier[24] = tabHauteur[2]; fichier[25] = tabHauteur[3];
            byte[] tabNbrBitCouleur = Convertir_Int_To_Endian(this.nbBitsCouleur);
            fichier[28] = tabNbrBitCouleur[0]; fichier[29] = tabNbrBitCouleur[1];

            int t = 54;
            /*Pas encore trouvé comment faire tourner l'image
            for (int l = 0; l < this.hauteur; l++)
            {
                for (int c = 0; c < this.largeur; c++)
                {
                    fichier[t CHANGEMENT] = this.image[l, c].R;    //on attribue les octets des pixel R, G et B à la matrice de byte
                    fichier[t + 1 CHANGEMENT] = this.image[l, c].G;
                    fichier[t + 2 CHANGEMENT] = this.image[l, c].B;
                    t += 3;
                }
            }*/
        }
        /// <summary>
        /// Matrice de convolution qui applique plusieurs filtre à une image
        /// Filtres : détection des contours/Renforcement des bords/Flou/Repoussage/Augmenter le contraste
        /// Diviseur : pour chaque filtre, il y a une matrice de convolution 3x3 correspondante. Le diviseur du filtre est la somme des éléments de cette matrice
        /// Application : pour chaque pixel, on prend la matrice 3x3 de pixel autour de celui selectionné, puis on la multiplie élément par élément et on fait la somme. Enfin, on divise cette somme par le diviseur qui va coder le nouveau pixel
        /// Résultat : finalement, on attribut à chaque pixel son pixel filtré dans une nouvelle matrice de byte qu'on associe avec la nouvelle image créée
        /// </summary>
        public void Convolution()
        {
            int choix = 0;
            while (choix < 1 || choix > 6)
            {
                Console.WriteLine("Vous voulez: " + "\n (1) Détection des contours" + "\n (2) Renforcement des bords" + "\n (3) Appliquer un flou" + "\n (4) Flou Gaussien" + "\n (5) Repoussage" + "\n (6) Augmenter le contraste");
                choix = Convert.ToInt32(Console.ReadLine());
            }
            int[,] matconvolution = new int[3, 3];
            Pixel[,] matricefinale = new Pixel[this.hauteur, this.largeur];
            if (choix == 1)
            {
                int[,] tab = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
                matconvolution = tab;

            }
            if (choix == 2)
            {
                int[,] tab = { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
                matconvolution = tab;
            }
            if (choix == 3)
            {
                for (int l = 0; l < matconvolution.GetLength(0); l++)
                {
                    for (int c = 0; c < matconvolution.GetLength(1); c++)
                    {
                        matconvolution[l, c] = 1;
                    }
                }
            }
            if (choix == 4)
            {
                int[,] tab = { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
                matconvolution = tab;
            }
            if (choix == 5)
            {
                int[,] tab = { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
                matconvolution = tab;
            }
            if (choix == 6)
            {
                int[,] tab = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
                matconvolution = tab;
            }

            int diviseur = 0;
            for (int l = 0; l < matconvolution.GetLength(0); l++)
            {
                for (int c = 0; c < matconvolution.GetLength(1); c++)
                {
                    diviseur += matconvolution[l, c];
                }
            }
            if (diviseur == 0) diviseur = 1;

            int l3 = 0;
            int c3 = 0;
            int sommebyteR = 0;
            int sommebyteG = 0;
            int sommebyteB = 0;
            for (int l = 0; l < matricefinale.GetLength(0); l++)
            {
                for (int c = 0; c < matricefinale.GetLength(1); c++)
                {
                    for (int l1 = l - 1; l1 <= l + 1; l1++)
                    {
                        for (int c1 = c - 1; c1 <= c + 1; c1++)
                        {
                            if (l1 != -1 && l1 != image.GetLength(0) && c1 != -1 && c1 != image.GetLength(1))
                            {
                                sommebyteR += (Convert.ToInt32(this.image[l1, c1].R) * matconvolution[l3, c3]);
                                sommebyteG += (Convert.ToInt32(this.image[l1, c1].G) * matconvolution[l3, c3]);
                                sommebyteB += (Convert.ToInt32(this.image[l1, c1].B) * matconvolution[l3, c3]);

                            }
                            c3++;
                        }
                        c3 = 0;
                        l3++;
                    }
                    l3 = 0;
                    sommebyteR /= (diviseur);
                    sommebyteG /= (diviseur);
                    sommebyteB /= (diviseur);
                    if (sommebyteR > 255) sommebyteR = 255; if (sommebyteG > 255) sommebyteG = 255; if (sommebyteB > 255) sommebyteB = 255;
                    if (sommebyteR < 0) sommebyteR = 0; if (sommebyteG < 0) sommebyteG = 0; if (sommebyteB < 0) sommebyteB = 0;                             //normalisation

                    matricefinale[l, c] = new Pixel(Convert.ToByte(sommebyteR), Convert.ToByte(sommebyteG), Convert.ToByte(sommebyteB));        //création du nouveau pixel grâce à la matrice de convolution 

                    sommebyteR = 0;
                    sommebyteG = 0;
                    sommebyteB = 0;
                }
            }
            this.image = matricefinale;
        }
    }
}
