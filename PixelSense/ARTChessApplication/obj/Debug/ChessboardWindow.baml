   M S B A M L   `   `   `  ����C  ?ARTChess, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullQ-clr-namespace:ARTChessApplication.Cases.PionsARTChessApplication.Cases.Pions  5   �   M+clr-namespace:ARTChessApplication.ResourcesARTChessApplication.Resources  6*  +    %ARTChessApplication.ChessboardWindows   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      e aMicrosoft.Surface.Presentation, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e351s)http://schemas.microsoft.com/surface/2008  5      :d2http://schemas.microsoft.com/expression/blend/2008  5   2   Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  6o   6my-clr-namespace:ARTChessApplication.Cases.Pions  6�   5my1+clr-namespace:ARTChessApplication.Resources  6*  -   5          � Title$  ARTChessApplication��5       � WindowStyle$
 None=�6�    � WindowState$ 	Maximized=�6�    � Topmost .   � SizeToContent$ WidthAndHeight=�6�   � 
ResizeMode$ NoResize=�6�   � AllowsTransparency .  � DataContext 6�  �� .��5      � $y�Visible=�6   $
��1085��6    $
��1925��6.   .+�5   
   � -   5        $��SearchProgressBar��5   -   $��12,0,1786,824q�6   $	��128��6F   $��20��6R   $��Center=�6^   $��Bottom=�6{   ��5	      �� 5
       �� EndPoint$ 1,0.5&�6&   	 �� 
StartPoint$	 0,0.5&�67   .��5      � 
 � Color$
 	#FF049D4B��6#    � Offset$ 0Q�65   � 5      $
 	#FF00A37B��6#   $ 1Q�65   5      5      .   (ARTChessApplication.Resources.ExitButton  5   
   -   5          Name$ exitButton1��5   4   .   (ARTChessApplication.Resources.ExitButton   	TouchDown$ ExitButton_TouchDown��6a   $
��Left=�6   $	��Top=�6I   $��
12,109,0,0q�6�   1   +ARTChessApplication.Resources.NewGameButton  5   
   -   5          Name$ newGameButton1��5   J   1   +ARTChessApplication.Resources.NewGameButton   	TouchDown$ NewGameButton_TouchDown��6z   $
��Left=�6   $��	10,12,0,0q�67   $	��Top=�6b   6z   .   (ARTChessApplication.Resources.UndoButton  5   
   -   5          Name$ undoButton1��5   G   .   (ARTChessApplication.Resources.UndoButton   	TouchDown$ UndoButton_TouchDown��6t   $
��Left=�6   $��	10,57,0,0q�64   $	��Top=�6\   6t   A� 5   
   -   5        $��ChessBoardGrid��5   B    A� Rows$ 8��6    A� Columns$ 8��6   $��420,12,424,12q�6+   $��Center=�6X   $	��850��6u   $	��850��6�   $��Center=�6�   6�   �� 5   
   -   5        $��label1��5   i   $��Progress bar��6   $��40��6'   $��Center=�63   $��10,195,1740,850q�6P   $��Center=�6w   $	��175��6�   ���	#FF1793E26�   �� 5   
   -   5        $��
EventLabel��5   V   $�� ��6   $��68��6   $
��Left=�6'   $��
12,167,0,0q�6B   $	��Top=�6h   $	��175��6�   �� 5   
   -	   5        $��image1��5   K   $	��200��6   $
��Left=�6   $��	330,0,0,0q�68   $
��Fill=�6Y   $	��Top=�6h   $	��200��6�   $(��"/Images/QrCodes/qr_haut_gauche.jpg��6�   �� 5   
   -
   5        $��image2��5   L   $	��200��6   $
��Left=�6   $��
1392,0,0,0q�68   $
��Fill=�6Z   $	��Top=�6i   $	��200��6�   $(��"/Images/QrCodes/qr_haut_droite.jpg��6�   �� 5   
   -   5        $��image3��5   M   $	��200��6   $
��Left=�6   $��330,880,0,0q�68   $
��Fill=�6[   $	��Top=�6j   $	��200��6�   $'��!/Images/QrCodes/qr_bas_gauche.jpg��6�   �� 5   
   -   5        $��image4��5   N   $	��200��6   $
��Left=�6   $��1392,880,0,0q�68   $
��Fill=�6\   $	��Top=�6k   $	��200��6�   $'��!/Images/QrCodes/qr_bas_droite.jpg��6�   �� 5   
   -   5        $��EventTextBlock��5   C   $
��Left=�6   $��
12,267,0,0q�6/   $�� ��6Y   $	��Top=�6a   $	��270��6y   $	��478��6�   $��15(�6�   � 5   
   -   5        $��grid1��5   I   $	��842��6   $��Center=�6   $��329,40q�69   $��Center=�6V   $	��842��6q   $y�Hidden=�6}    � RowDefinitions 5      �� 5      $	��75* �6    �� 5      $	��75* �6    �� 5      $	��75* �6    �� 5       $	��75* �6    �� 5!      $	��75* �6    �� 5"      $	��75* �6    �� 5#      $	��75* �6    �� 5$      $	��75* �6     � ColumnDefinitions 5&      �� 5'      $	��75* �6#   �� 5(      $	��75* �6#   �� 5)      $	��75* �6#   �� 5*      $	��75* �6#   �� 5+      $	��75* �6#   �� 5,      $	��75* �6#   �� 5-      $	��75* �6#   �� 5.      $	��75* �6#   .+�50      ;  5Microsoft.Surface.Presentation.Controls.TagVisualizer  -   5        $��TagVisualizer��51      $��Stretch=�52      $��Stretch=�53      $��8��6�   $��8��6�   54      �� 55   
   -   5        $��UndoChosenMoveButton��55   g   $��Undo First Move��6   $	��136��6+   $
��Left=�68   $��
126,12,0,0q�6S   $	��Top=�6�   $	��180��6�   6�   ��56   
   �� 57      $ 0.5,1&�6    $	 0.5,0&�61   .��58      � $
 	#FF3C5055��6   $ 1Q�6-   � 59      $
 	#FF03D8F5��6   $ 0Q�6-   5:      5<   	   5=      