using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_Info_S4
{
    public class QR
    {
        #region Attributs
        private int[] mode = { 0, 0, 1, 0 };
        private int[] nbCaracteres = new int[9];
        private int[] donnees;
        private int[] correction;
        private int[] masque = { 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0 };
        private char[] listeCarac = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ', '$', '%', '*', '+', '-', '.', '/', ':' };
        private int version = 0;
        #endregion

        #region Constructeurs
        public QR(string phrase)
        {
            if (phrase.Length < 48)
            {
                if (phrase.Length < 26)
                {
                    donnees = new int[152];
                    this.version = 1;
                }
                if (phrase.Length > 25)
                {
                    donnees = new int[272];
                    this.version = 2;
                }
                bool phrasePaire = false;
                if ((phrase.Length) % 2 == 0)
                {
                    phrasePaire = true;
                }
                string paire = "";  //initialisation paire de 2 lettres
                int indexCarac1 = -1;
                int indexCarac2 = -1;
                int valeurEntier;
                int indexTabDonnees = 13;
                int[] chaineCaracBit = new int[11]; //initalisation de la chaine de 11bits qui représente la paire de mots en bits
                for (int i = 0; i < phrase.Length - 1; i += 2)       //on fait la boucle en laissant le ou les 2 derniers char (car pb si phrase.length est impaire)
                {
                    paire += phrase[i];    //on note la paire
                    paire += phrase[i + 1];

                    for (int j = 0; j < listeCarac.Length; j++)
                    {
                        if (Convert.ToChar(paire[0]) == listeCarac[j])
                        {
                            indexCarac1 = j;    //on trouve l'index de la lettre associée à notre bibliothèque de cararc
                        }
                        if (Convert.ToChar(paire[1]) == listeCarac[j])
                        {
                            indexCarac2 = j;    //pareil pour la deuxième lettre de la paire
                        }
                    }
                    valeurEntier = 45 * indexCarac1 + indexCarac2;  //on calcul la valeur de la paire en nombre entier
                    chaineCaracBit = Convertir_Int_To_nBit(valeurEntier, 11);  //on convertit la valeur de la paire en chaine de 11 bits
                    int indexchainecar = 0;
                    for (int c = indexTabDonnees; c < indexTabDonnees + 11; c++)
                    {
                        this.donnees[c] = chaineCaracBit[indexchainecar];   //on place les 11bits de la chaine précédemment obtenu dans l'attribut "donnees"
                        indexchainecar++;                              //on implement de 1 l'index du tab chaine de bits
                    }
                    indexTabDonnees += 11;          //on implement de 11 l'index du tab "donnees" car chaque paire de caractères est codée sur 11 bits
                    paire = ""; //on remet la paire à "0"
                }
                if (phrasePaire == true)
                {
                    paire += phrase[phrase.Length - 2];
                    paire += phrase[phrase.Length - 1];
                    for (int j = 0; j < listeCarac.Length; j++)
                    {
                        if (Convert.ToChar(paire[0]) == listeCarac[j])
                        {
                            indexCarac1 = j;    //on trouve l'index de la lettre associée à notre bibliothèque de cararc
                        }
                        if (Convert.ToChar(paire[1]) == listeCarac[j])
                        {
                            indexCarac2 = j;    //pareil pour la deuxième lettre de la paire
                        }
                    }
                    valeurEntier = 45 * indexCarac1 + indexCarac2;  //on calcul la valeur de la paire en nombre entier
                    chaineCaracBit = Convertir_Int_To_nBit(valeurEntier, 11);  //on convertit la valeur de la paire en chaine de 11 bits

                    int indexchainecar = 0;
                    for (int c = indexTabDonnees; c < indexTabDonnees + 11; c++)
                    {
                        this.donnees[c] = chaineCaracBit[indexchainecar];   //on place les 11bits de la chaine précédemment obtenu dans l'attribut "donnees"
                        indexchainecar++;                              //on implement de 1 l'index du tab chaine de bits
                    }
                    indexTabDonnees += 11;          //on implement de 11 l'index du tab "donnees" car chaque paire de caractères est codée sur 11 bits
                    paire = ""; //on remet la paire à "0"
                }
                if (phrasePaire == false)
                {
                    paire += phrase[phrase.Length - 1];

                    for (int j = 0; j < listeCarac.Length; j++)
                    {
                        if (Convert.ToChar(paire[0]) == listeCarac[j])
                        {
                            indexCarac1 = j;    //on trouve l'index de la lettre associée à notre bibliothèque de cararc
                        }
                    }

                    valeurEntier = indexCarac1;
                    chaineCaracBit = Convertir_Int_To_nBit(valeurEntier, 6);

                    int indexchainecar = 0;
                    for (int c = indexTabDonnees; c < indexTabDonnees + 6; c++)
                    {
                        this.donnees[c] = chaineCaracBit[indexchainecar];   //on place les 11bits de la chaine précédemment obtenu dans l'attribut "donnees"
                        indexchainecar++;                              //on implement de 1 l'index du tab chaine de bits
                    }
                    indexTabDonnees += 6;          //on implement de 11 l'index du tab "donnees" car chaque paire de caractères est codée sur 11 bits
                    paire = ""; //on remet la paire à "0"
                }
                while (indexTabDonnees % 8 != 0)
                {
                    indexTabDonnees++;
                }
                int nbrOctetRajouter = (donnees.Length - indexTabDonnees) / 8;
                int[] huitbit236 = Convertir_Int_To_nBit(236, 8);
                int[] huitbit17 = Convertir_Int_To_nBit(17, 8);
                int temp = 0;
                while (nbrOctetRajouter > 0)
                {
                    temp = 0;
                    for (int c = indexTabDonnees; c < indexTabDonnees + 8; c++)
                    {
                        this.donnees[c] = huitbit236[temp];   //on place les 11bits de la chaine précédemment obtenu dans l'attribut "donnees"
                        temp++;
                    }
                    indexTabDonnees += 8;
                    nbrOctetRajouter--;

                    if (nbrOctetRajouter > 0)
                    {
                        temp = 0;
                        for (int c = indexTabDonnees; c < indexTabDonnees + 8; c++)
                        {
                            this.donnees[c] = huitbit17[temp];   //on place les 11bits de la chaine précédemment obtenu dans l'attribut "donnees"
                            temp++;
                        }
                        indexTabDonnees += 8;
                        nbrOctetRajouter--;
                    }
                }

            }
        }
        #endregion

        #region Proprietés
        public int[] Mode
        {
            get { return this.mode; }
            set { for (int i = 0; i < value.Length; i++) { this.mode[i] = value[i]; } }
        }
        public int[] NbCaracteres
        {
            get { return this.nbCaracteres; }
        }
        public int[] Donnees
        {
            get { return this.donnees; }
            set { for (int i = 0; i < value.Length; i++) { this.donnees[i] = value[i]; } }
        }
        public int[] Correction
        {
            get { return this.correction; }
            set { for (int i = 0; i < value.Length; i++) { this.correction[i] = value[i]; } }
        }
        public int[] Masque
        {
            get { return this.masque; }
            set { for (int i = 0; i < value.Length; i++) { this.masque[i] = value[i]; } }
        }
        #endregion

        //Méthodes
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
    }
}
