using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

namespace ARTChessApplication.Cases.Pions
{
    /// <summary>
    /// Interaction logic for CavN.xaml
    /// </summary>
    public partial class CavN : TagVisualization
    {
        public CavN()
        {
            InitializeComponent();
        }

        private void CavN_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: customize CavN's UI based on this.VisualizedTag here
        }
    }
}
