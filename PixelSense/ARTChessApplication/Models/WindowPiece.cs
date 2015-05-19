using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;
using System.Threading;
using Microsoft.Surface.Presentation;
using Microsoft.Surface;


namespace ARTChessApplication.Models
{
    internal sealed class WindowPiece
    {
        public delegate void ChosenHandler(int row, int column);
        public event ChosenHandler Chosen;

        public int Row { private set; get; }
        public int Column { private set; get; }

        public Border DisplayControl { private set; get; }
        public Image DisplayImage { private set; get; }

        public typePiece Type { private set; get; }

        public Microsoft.Surface.Core.TouchEventArgs touch;

        public WindowPiece(int row, int column, typePiece type, colorPiece color)
        {
            Row = row;
            Column = column;

            Type = type;
            CreateImage(type, color);
            CreateControl();
        }

        private void CreateControl()
        {
            Border border = new Border();

            border.Child = DisplayImage;
            if ((Row + Column) % 2 == 0)
                border.Background = new SolidColorBrush(Colors.Beige);
            else
                border.Background = new SolidColorBrush(Colors.Black);
            DisplayControl = border;
            border.MouseLeftButtonUp += new MouseButtonEventHandler(DisplayControl_MouseLeftButtonDown);
        }
        
        void DisplayControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Chosen != null)
                Chosen(Row, Column);
        }

        private void CreateImage(typePiece type, colorPiece color)
        {
            string imageLink = String.Empty;

            DisplayImage = new Image();
            if (type == typePiece.Rien)
                return;

            if (color == colorPiece.Noir)
            {
                if (type == typePiece.Fou)
                    imageLink += "Images/Noirs/FouN.gif";
                else if (type == typePiece.Roi)
                    imageLink += "Images/Noirs/RoiN.gif";
                else if (type == typePiece.Cavalier)
                    imageLink += "Images/Noirs/CavN.gif";
                else if (type == typePiece.Pion)
                    imageLink += "Images/Noirs/PionN.gif";
                else if (type == typePiece.Dame)
                    imageLink += "Images/Noirs/DameN.gif";
                else if (type == typePiece.Tour)
                    imageLink += "Images/Noirs/TourN.gif";
            }
            else
            {
                if (type == typePiece.Fou)
                {
                    imageLink += "Images/Blancs/FouB.gif";
                }
                else if (type == typePiece.Roi)
                {
                    imageLink += "Images/Blancs/RoiB.gif";
                }
                else if (type == typePiece.Cavalier)
                {
                    imageLink += "Images/Blancs/CavB.gif";
                }
                else if (type == typePiece.Pion)
                {
                    imageLink += "Images/Blancs/PionB.gif";
                }
                else if (type == typePiece.Dame)
                {
                    imageLink += "Images/Blancs/DameB.gif";
                }
                else if (type == typePiece.Tour)
                {
                    imageLink += "Images/Blancs/TourB.gif";
                }
            }
            BitmapImage bitmap = new BitmapImage(new Uri(imageLink, UriKind.Relative));
            DisplayImage.Source = bitmap;
        }
    }
}
