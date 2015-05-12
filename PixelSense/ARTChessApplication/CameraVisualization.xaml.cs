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

namespace ARTChessApplication
{
    /// <summary>
    /// Interaction logic for CavB.xaml
    /// </summary>
    public partial class CameraVisualization : TagVisualization
    {
        public CameraVisualization()
        {
            InitializeComponent();
        }

        private void CameraVisualization_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: customize CavB's UI based on this.VisualizedTag here
        }
    }
}
