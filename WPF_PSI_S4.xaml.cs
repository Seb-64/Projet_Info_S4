using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Projet_Info_S4;

namespace Projet_Info_S4
{
    /// <summary>
    /// Logique d'interaction pour WPF_PSI_S4.xaml
    /// </summary>
    public partial class WPF_PSI_S4 : UserControl
    {
        public WPF_PSI_S4()
        {
            InitializeComponent();
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            this.rbtnAgran.IsChecked = this.rbtnBords.IsChecked = this.rbtnContour.IsChecked = this.rbtnContraste.IsChecked = this.rbtnFlou.IsChecked = this.rbtnFlouG.IsChecked =
                this.rbtnGris.IsChecked = this.rbtnMiroir.IsChecked = this.rbtnNB.IsChecked = this.rbtnRepoussage.IsChecked = this.rbtnRetre.IsChecked = this.rbtnRot.IsChecked = false;
            this.ChoisirUneImage.Text = "Choisir une image...";
            this.ModifSelectionnée.Text = "";
        }
        private void btnAppliquer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbtn_Checked(object sender, RoutedEventArgs e)
        {
            this.ModifSelectionnée.Text = (string)((RadioButton)sender).Content;
        }

        private void Coco_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void Lac_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void Lena_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
