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
        #region Attributs
        private string nomFichier;
        private string type;
        private int tailleFichier;
        private int tailleOffset;
        private int hauteur;
        private int largeur;
        private int nbBitsCouleur;
        private Pixel[,] image;
        #endregion

        #region Constructeurs
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
                        this.image[ligne, colonne] = new Pixel(fichier[k + 2], fichier[k + 1], fichier[k]);
                        k += 3;
                    }
                }
            }
            else { Console.WriteLine("Fichier inexistant"); }
        }
        public MyImage(MyImage aCopier)
        {
            this.nomFichier = aCopier.nomFichier.Replace(".bmp", "_copie.bmp");
            this.type = aCopier.type;
            this.tailleFichier = aCopier.tailleFichier;
            this.tailleOffset = aCopier.tailleOffset;
            this.hauteur = aCopier.hauteur;
            this.largeur = aCopier.largeur;
            this.nbBitsCouleur = aCopier.nbBitsCouleur;
            this.image = aCopier.image;
        }
        public MyImage() { }
        #endregion

        #region Méthodes
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
                    fichier[t] = this.image[l, c].B;    //on attribue les octets des pixel R, G et B à la matrice de byte
                    fichier[t + 1] = this.image[l, c].G;
                    fichier[t + 2] = this.image[l, c].R;
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
        public MyImage Rotation(int angle)
        {
            MyImage rotation = new MyImage();
            int diagonale = (int)Math.Sqrt(largeur * largeur + hauteur * hauteur);
            Pixel[,] rota = new Pixel[diagonale + 10, diagonale + 10];
            rotation.nomFichier = "Images\\rotation.bmp";
            rotation.type = "BM";
            rotation.tailleFichier = diagonale * diagonale * 24 + 54;
            rotation.tailleOffset = 54;
            rotation.nbBitsCouleur = 24;

            for (int i = 0; i < diagonale; i++)
            {
                for (int j = 0; j < diagonale; j++)
                {
                    rota[i, j] = new Pixel(255, 255, 255);
                }
            }

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    int[] tab = new int[2];
                    tab = Rot(largeur / 2 - j, hauteur / 2 - i, angle);

                    if (tab[1] < diagonale / 2 && tab[0] < diagonale / 2)
                    {
                        rota[diagonale / 2 - tab[1], diagonale / 2 - tab[0]] = image[i, j];
                    }
                    else if (tab[1] < diagonale / 2 && tab[0] >= diagonale / 2)
                    {
                        rota[diagonale / 2 - tab[1], tab[0] - diagonale / 2] = image[i, j];
                    }
                    else if (tab[1] >= diagonale / 2 && tab[0] < diagonale / 2)
                    {
                        rota[tab[1] - diagonale / 2, diagonale / 2 - tab[0]] = image[i, j];
                    }
                    else if (tab[1] >= diagonale / 2 && tab[0] >= diagonale / 2)
                    {
                        rota[tab[1] - diagonale / 2, tab[0] - diagonale / 2] = image[i, j];
                    }
                }
            }
            rotation.hauteur = diagonale;
            rotation.largeur = diagonale;
            rotation.image = rota;
            return rotation;
        }
        private int[] Rot(int x, int y, double angle)
        {
            int[] tab = new int[2];
            angle *= Math.PI / 180;
            x += (int)(-y * Math.Tan(angle / 2));
            y += (int)(x * Math.Sin(angle));
            x += (int)(-y * Math.Tan(angle / 2));
            tab[0] = x;
            tab[1] = y;
            return tab;
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
        /// <summary>
        /// Fonction qui créer une image fractale, dans notre cas l'ensemble de Mandelbrot
        /// </summary>
        public void Fractale()
        {
            int hgt = this.largeur;
            int wid = this.hauteur;
            double dc_r = (0.6 - (-2.1)) / (wid - 1);
            double dc_i = (1.2 - (-1.2)) / (hgt - 1);

            double c_r = -2.1;
            for (int i = 0; i < wid; i++)
            {
                double c_i = -1.2;
                for (int j = 0; j < hgt; j++)
                {
                    double z_r = 0;
                    double z_i = 0;
                    double z2_r = 0;
                    double z2_i = 0;
                    int clr = 1;
                    while ((clr <= 50) && (z2_r + z2_i < 4))
                    {
                        z2_r = z_r * z_r;
                        z2_i = z_i * z_i;
                        z_i = 2 * z_i * z_r + c_i;
                        z_r = z2_r - z2_i + c_r;
                        clr++;
                    }

                    if (clr > 50) { image[i, j].R = image[i, j].G = image[i, j].B = 0; }
                    else { image[i, j].R = image[i, j].G = image[i, j].B = 255; }

                    c_i += dc_i;
                }
                c_r += dc_r;
            }
        }
        /// <summary>
        /// Fonction statique qui créer d'abord une instance de MyImage null puis on donne à tout ses attributs les valeurs pour définir une image Bitmap.
        /// Ensuite, avec la hauteur et la largeur rentrées en paramètre, on calcule la taille du fichier et on créer une matrice de pixel noir de la taille de l'image.
        /// </summary>
        /// <param name="hauteur">Hauteur de l'image</param>
        /// <param name="largeur">Largeur de l'image</param>
        /// <returns>Nouvelle image Bitmap noir de taille hauteur*largeur</returns>
        public static MyImage CréerMyImage(int hauteur, int largeur)
        {
            MyImage image = new MyImage();
            image.nomFichier = "Images\\nouvelle_image.bmp";
            image.type = "BM";
            image.tailleFichier = hauteur * largeur * 24 + 54;
            image.tailleOffset = 54;
            image.hauteur = hauteur;
            image.largeur = largeur;
            image.nbBitsCouleur = 24;
            image.image = new Pixel[hauteur, largeur];

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    image.image[i, j] = new Pixel(0, 0, 0);
                }
            }
            return image;
        }
        /// <summary>
        /// Création des histogrammes de couleur rouge, vert et bleu.
        /// Tout d'abord on créer la nouvelle image sur laquelle va apparaître les 3 histogrammes, de 256 de large, qui correspond au nombre de possibilité d'intensité du pixel
        /// et de 3x256=768 de hauteur : chaque histogramme tiendra dans un carré de 256 de côté.
        /// On compte ensuite le nombre de pixel qui ont la même intensité de couleur entre 0 et 255 pour chacune des 3 couleurs.
        /// On trouve laquelle de ces intensités entre toutes les couleurs est la plus grande pour ensuite faire une échelle par rapport à cette dernière.
        /// Enfin on Map les valeurs d'intensités des couleurs, qui vont de 0 à la valeur max calculée précédemment, entre 0 et 256 grâce à la fonction Map.
        /// Si la valeur retourné est supérieure à la ligne en construction, alors on change la valeur du pixel à la même valeur de la ligne pour construire l'histogramme.
        /// Finalement, on retourne l'histogramme créé dans une imgae MyImage dans laquelle on a précisé tous ses attributs préalablement.
        /// </summary>
        /// <returns>Histogramme des 3 couleurs des pixels sur des carrés de 256x256</returns>
        public MyImage Histogramme()
        {
            MyImage histo = new MyImage();
            histo.hauteur = 768;
            histo.largeur = 256;
            Pixel[,] histogramme = new Pixel[768, 256];

            int[] tabHistoR = new int[256];
            int[] tabHistoG = new int[256];
            int[] tabHistoB = new int[256];

            int compteurR = 0;
            int compteurG = 0;
            int compteurB = 0;
            int valMax = 0;

            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < this.hauteur; j++)
                {
                    for (int k = 0; k < this.largeur; k++)
                    {
                        if (image[j, k].R == i) compteurR++;
                        if (image[j, k].G == i) compteurG++;
                        if (image[j, k].B == i) compteurB++;
                    }
                }

                if (compteurR > compteurG && compteurR > compteurB && compteurR > valMax) { valMax = compteurR; }
                else if (compteurG > compteurR && compteurG > compteurB && compteurG > valMax) { valMax = compteurG; }
                else if (compteurB > compteurR && compteurB > compteurG && compteurB > valMax) { valMax = compteurB; }

                tabHistoR[i] = compteurR;
                tabHistoG[i] = compteurG;
                tabHistoB[i] = compteurB;

                compteurR = 0;
                compteurG = 0;
                compteurB = 0;
            }
            for (int i = 0; i < 256; i++)
            {
                for (int ligne = 0; ligne < 256; ligne++)
                {
                    histogramme[ligne, i] = new Pixel(0, 0, 0);
                    histogramme[ligne + 256, i] = new Pixel(0, 0, 0);
                    histogramme[ligne + 512, i] = new Pixel(0, 0, 0);
                    if (ligne < Map(tabHistoR[i], 0, valMax, 0, 256)) { histogramme[ligne, i].R = (byte)i; }
                    else { histogramme[ligne, i].R = (byte)0; }
                    if (ligne < Map(tabHistoG[i], 0, valMax, 0, 256)) { histogramme[ligne + 256, i].G = (byte)i; }
                    else { histogramme[ligne + 256, i].G = (byte)0; }
                    if (ligne < Map(tabHistoB[i], 0, valMax, 0, 256)) { histogramme[ligne + 512, i].B = (byte)i; }
                    else { histogramme[ligne + 512, i].B = (byte)0; }
                }
            }

            histo.nomFichier = this.nomFichier.Replace(".bmp", "_histogramme.bmp");
            histo.type = "BM";
            histo.tailleFichier = 768 * 256 * 24 + 54;
            histo.tailleOffset = 54;
            histo.nbBitsCouleur = 24;
            histo.image = histogramme;

            return histo;
        }
        /// <summary>
        /// Fonction qui prend en entrée une valeur x, comprise entre in_min et in_max, et qui renvoie une valeur proportionnelle comrpise entre out_min et out_max
        /// </summary>
        /// <param name="x">variable entière à changer</param>
        /// <param name="in_min">borne min entrée</param>
        /// <param name="in_max">borne max entrée</param>
        /// <param name="out_min">borne min sortie</param>
        /// <param name="out_max">borne max sortie</param>
        /// <returns></returns>
        public int Map(int x, int in_min, int in_max, int out_min, int out_max)
        {
            if ((in_max - in_min) == 0) return x;
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
        public MyImage Encoder2Images(MyImage imageACopier)
        {
            MyImage imageDansImage = new MyImage();
            if (this.largeur >= imageACopier.largeur && this.hauteur >= imageACopier.hauteur)
            {
                imageDansImage.image = new Pixel[hauteur, largeur];
                imageDansImage.type = "BM";
                imageDansImage.tailleFichier = this.tailleFichier;
                imageDansImage.tailleOffset = 54;
                imageDansImage.nbBitsCouleur = 24;
                imageDansImage.nomFichier = this.nomFichier.Replace(".bmp", "_encoder.bmp");
                imageDansImage.hauteur = this.hauteur;
                imageDansImage.largeur = this.largeur;
                if (this.largeur == imageACopier.largeur && this.hauteur == imageACopier.hauteur)
                {
                    for (int i = 0; i < hauteur; i++)
                    {
                        for (int j = 0; j < largeur; j++)
                        {
                            imageDansImage.image[i, j] = new Pixel(0, 0, 0);

                            int valeurR = this.image[i, j].R;
                            int valeurG = this.image[i, j].G;
                            int valeurB = this.image[i, j].B;
                            int valeurRcopie = imageACopier.image[i, j].R;
                            int valeurGcopie = imageACopier.image[i, j].G;
                            int valeurBcopie = imageACopier.image[i, j].B;

                            int[] bitR = Convertir_Int_To_nBit(valeurR, 8);
                            int[] bitG = Convertir_Int_To_nBit(valeurG, 8);
                            int[] bitB = Convertir_Int_To_nBit(valeurB, 8);
                            int[] bitRcopie = Convertir_Int_To_nBit(valeurRcopie, 8);
                            int[] bitGcopie = Convertir_Int_To_nBit(valeurGcopie, 8);
                            int[] bitBcopie = Convertir_Int_To_nBit(valeurBcopie, 8);

                            bitR[4] = bitRcopie[0]; bitR[5] = bitRcopie[1]; bitR[6] = bitRcopie[2]; bitR[7] = bitRcopie[3];
                            bitG[4] = bitGcopie[0]; bitG[5] = bitGcopie[1]; bitG[6] = bitGcopie[2]; bitR[7] = bitGcopie[3];
                            bitB[4] = bitBcopie[0]; bitB[5] = bitBcopie[1]; bitB[6] = bitBcopie[2]; bitR[7] = bitBcopie[3];

                            imageDansImage.image[i, j].R = (byte)Convertir_nBit_To_Int(bitR, 8);
                            imageDansImage.image[i, j].G = (byte)Convertir_nBit_To_Int(bitG, 8);
                            imageDansImage.image[i, j].B = (byte)Convertir_nBit_To_Int(bitB, 8);
                        }
                    }
                }
            }
            return imageDansImage;
        }
        public int[] Convertir_Int_To_nBit(int valeur, int nbBit)
        {
            int[] resultat = new int[nbBit];
            long reste = 0;
            long quotient = 0;
            for (int i = 0; i < resultat.Length; i++)
            {
                quotient = valeur / Convert.ToInt64(Math.Pow(2, (nbBit - 1 - i)));
                reste = valeur - quotient * Convert.ToInt64(Math.Pow(2, (nbBit - 1 - i)));
                valeur = Convert.ToInt32(reste);
                resultat[i] = (int)(quotient);
            }
            return resultat;
        }
        public int Convertir_nBit_To_Int(int[] valeur, int nbBit)
        {
            double result = 0;
            for (int i = 0; i < valeur.Length; i++)
            {
                result += valeur[nbBit - 1 - i] * Math.Pow(2, i);
            }
            return Convert.ToInt32(result);
        }
        #endregion
    }
}
