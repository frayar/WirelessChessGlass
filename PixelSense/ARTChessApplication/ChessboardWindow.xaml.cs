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
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using ARTChessApplication.Models;

using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;

using System.Speech.Synthesis;


namespace ARTChessApplication
{
    /// <summary>
    /// Interaction logic for ChessboardWindows.xaml
    /// </summary>
    public partial class ChessboardWindows : SurfaceWindow
    {
        // --------------------------------------------------------------------
        // ATTRIBUTES
        // --------------------------------------------------------------------
        #region Attributes

        /// <summary>
        /// Attributes
        /// </summary>
        public delegate void ChosenHandler(int row, int column);
        public event ChosenHandler Chosen;
        private ChessBoard _chessBoard;
        private MoveValidator _validator;
        private WindowBoard _windowBoard;
        //private colorPiece _playerColor;
        private ChessEngine _engine;
        private Thread _thread;
        private bool _isCpuGame;
        private bool isMoving;
        private BluetoothManager _bluetoothManager;
        private bool _areDevicesPaired;
        private LogHandler _log;                            //Log contenant la majorité des événements. Met le jour le TextBlock à gauche de l'application
        private int _nbPiece = 0;
        private typePiece[] _typePieceSet = new typePiece[] {typePiece.Pion, typePiece.Pion, typePiece.Pion, typePiece.Pion, 
                                                            typePiece.Pion, typePiece.Pion, typePiece.Pion, typePiece.Pion,
                                                            typePiece.Fou, typePiece.Fou, typePiece.Cavalier, typePiece.Cavalier, 
                                                            typePiece.Tour, typePiece.Tour, typePiece.Dame, typePiece.Roi };

        private enum AppMode { INITIALIZING,PLAYING  };
        private AppMode _mode = AppMode.INITIALIZING;

        private SpeechSynthesizer _speechSynthesizer;
        

        #endregion


        // --------------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------------
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ChessboardWindows()
        {
            // Default initialization
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            // Initialize tags
            InitializeTagDefinitions();

            // Initialize bluetooth
            _bluetoothManager = new BluetoothManager();
            _bluetoothManager.Connect();

            // Initialize the speech synthesizer
            _speechSynthesizer = new SpeechSynthesizer();
        }

        #endregion


        // --------------------------------------------------------------------
        // PIXELSENSE DEFAULT INTERACTION HANDLERS
        // --------------------------------------------------------------------
        #region PixelSense Default interaction Handlers

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Occurs directly after Close is called, and can be handled to cancel window closure.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_thread != null)
                _thread.Abort();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        #endregion


        // --------------------------------------------------------------------
        // BUTTONS HANDLERS
        // --------------------------------------------------------------------
        #region Buttons handlers

        /// <summary>
        /// Handler for the "New Game" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGameButton_TouchDown(object sender, TouchEventArgs e)
        {
            // Display the board
            grid1.Visibility = Visibility.Visible;

            // Initialize some attributes
            _chessBoard = new ChessBoard();
            _windowBoard = new WindowBoard();
            _log = new LogHandler();

            
            // Notify user
            string deviceName = _bluetoothManager.GetDeviceName();
            if (deviceName.Equals("NULL"))
            {
                string log = "THE DEVICES ARE NOT CONNECTED";
                _log.AddString(log);
                _speechSynthesizer.Speak(log);
                _areDevicesPaired = false;
            } else {
                _log.AddString("CONNECTED TO " + _bluetoothManager.GetDeviceName());
                _speechSynthesizer.Speak("Devices connected");
                _areDevicesPaired = true;
            }
         
            _log.AddString("\n");
            _log.AddString("PLEASE PUT THE WHITE CHESSPIECE ON THE BOARD, ONE BY ONE.");
            _speechSynthesizer.SpeakAsync("Initialization phase");
            EventTextBlock.Text = _log.getFullLog();

            //Send reset request via bluetooth. 
            if (_areDevicesPaired) 
                _bluetoothManager.Send("RESET");

            UpdateWindow();
        }


        /// <summary>
        /// Handler for the "Exit" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_TouchDown(object sender, TouchEventArgs e)
        {
            Application.Current.Shutdown();
        }


        /// <summary>
        /// Handler for the "Undo" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoButton_TouchDown(object sender, TouchEventArgs e)
        {
            if (_isCpuGame == false)
                _chessBoard = _chessBoard.LastBoard;
            else
                _chessBoard = _chessBoard.LastBoard.LastBoard;
            _validator = new MoveValidator(_chessBoard);
            _validator.Validate();
            UpdateWindow();
        }

        #endregion

        // --------------------------------------------------------------------
        // TAG HANDLERS
        // --------------------------------------------------------------------
        #region Tag handlers

        /// <summary>
        /// Handler when a tagged chesspiece is put on the table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            // Get the camera and the detected tag
            CameraVisualization camera = (CameraVisualization)e.TagVisualization;
            TagVisualizer tag = (TagVisualizer)sender;
            string name = tag.Name;

            Point visualizationCenter = e.TagVisualization.Center;

            double tagHeight = tag.ActualHeight;
            double tagWidth = tag.ActualWidth;
            double X = visualizationCenter.X;
            double Y = visualizationCenter.Y;

            int column = getCurrentAxisRowPosition(tagWidth, X);
            int row = getCurrentAxisRowPosition(tagHeight, Y);

            switch(this._mode)
            {
                case AppMode.INITIALIZING:
                    testValuePiece( tag, this._typePieceSet[camera.VisualizedTag.Value] );
                    
                    if (this._nbPiece == 16)
                    {
                        // Game can start : initialize rest of the attributes
                        _validator = new MoveValidator(_chessBoard);
                        _validator.Validate();
                        _windowBoard.Chosen += new WindowBoard.ChosenHandler(_windowBoard_Chosen_Cpu);
                        _engine = new ChessEngine(4);
                        _isCpuGame = true;
                        isMoving = false;

                        this._mode = AppMode.PLAYING;
                        _log.AddString("GAME START : Your move!]");
                        _speechSynthesizer.Speak("Game Start. Make your move buddy.");
                    }

                    break;
                case AppMode.PLAYING:
                    if (isMoving)
                    {
                        _windowBoard.WindowBoard_Chosen(row, column);
                        isMoving = false;
                    }
                    break;
            }

            // Log
            EventTextBlock.Text = _log.getFullLog();
        }

        /// <summary>
        /// Méthode pour obtenir la case correspondant à la position de l'objet sur l'axe
        /// </summary>
        /// <param name="tagVisualizerAxisPosition">Position du tag visualizer sur l'axe, égale à 840 en l'état</param>
        /// <param name="visualizationAxisPosition">Position de l'objet posé sur l'axe</param>
        /// <returns></returns>
        int getCurrentAxisRowPosition(double tagVisualizerAxisPosition, double visualizationAxisPosition) {
            int TagVisuliazerRow = (int)tagVisualizerAxisPosition / 8;
            return (int)visualizationAxisPosition / TagVisuliazerRow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {

            TagVisualizer tag = (TagVisualizer)sender;
            Point visualizationCenter = e.TagVisualization.Center;
            //Point visualizationCenter =
             //       tag.TouchesCaptured.ElementAt(this._nbPiece).GetCenterPosition(tag);
            double tagHeight = tag.ActualHeight;
            double tagWidth = tag.ActualWidth;
            double X = visualizationCenter.X;
            double Y = visualizationCenter.Y;

            int column = getCurrentAxisRowPosition(tagWidth, X);
            int row = getCurrentAxisRowPosition(tagHeight, Y);
            //int column = Grid.GetColumn(tag);
            //int row = Grid.GetRow(tag);

            WindowPiece[,] pieces = _windowBoard.Pieces;
            WindowPiece piece = pieces[row, column];
            Piece[] p = _chessBoard.Pieces;
            //Piece[] lastPieces = _chessBoard.LastBoard.Pieces;

            // Patch to handle piece removal during initialization
            if (this._mode == AppMode.INITIALIZING)
            {
                this._nbPiece--;
                Piece removed_piece = p[row * 16 + column];
                byte position = (byte)(row * 16 + column);
                _log.AddString(removed_piece.type + "_" + getMoveFromByte(position) + " supprimé. [" + this._nbPiece + "]");
                EventTextBlock.Text = _log.getFullLog();
                return;
            }

            Piece currentPiece = p[row * 16 + column];
            //Piece oldPiece = lastPieces[row * 16 + column];
            if (currentPiece.type != typePiece.Rien && currentPiece.color == colorPiece.Blanc)
            {
                _windowBoard.WindowBoard_Chosen(row, column);
                isMoving = true;
            }
            else
            {
                //_log.AddString(oldPiece.type+"_supprimé.");
                EventTextBlock.Text = _log.getFullLog();
            }
        }

        /// <summary>
        /// Association pièce Virtuelle / pièce Réelle
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="typePiece"></param>
        /// <returns></returns>
        private bool testValuePiece(TagVisualizer tag, typePiece typePiece)
        {
            Point visualizationCenter =
                    tag.TouchesCaptured.ElementAt(tag.TouchesCaptured.Count()-1).GetCenterPosition(tag);
            
            double tagHeight = tag.ActualHeight;
            double tagWidth = tag.ActualWidth;
            double X = visualizationCenter.X;
            double Y = visualizationCenter.Y;

            int column = getCurrentAxisRowPosition(tagWidth, X);
            int row = getCurrentAxisRowPosition(tagHeight, Y);
            //int column = Grid.GetColumn(tag);
            //int row = Grid.GetRow(tag);
            /*
            WindowPiece[,] pieces = _windowBoard.Pieces;

            WindowPiece piece = pieces[row, column];
            */
            Piece[] p = _chessBoard.Pieces;

            Piece piece = p[row * 16 + column];

            if (piece.type == typePiece)
            {
                this._nbPiece++;
                byte position = (byte)(row * 16 + column);
                _log.AddString( piece.type + "_" + getMoveFromByte(position) + " déposée. [" + this._nbPiece + "]");
                string pieceEnglishName = Piece.translatePieceNameFromFrenchToEnglish(piece.type);
                _speechSynthesizer.SpeakAsync(pieceEnglishName + " " + getMoveFromByte(position) + "placed");
                
            }
            else
            {
                _log.AddString(piece.type + "_Mauvaise position.");
                _speechSynthesizer.SpeakAsync("Wrong position");

            }
            EventTextBlock.Text = _log.getFullLog();
            return false;
        }



        #endregion


        // --------------------------------------------------------------------
        // INIT
        // --------------------------------------------------------------------
        #region Init

        /// <summary>
        /// Initialization des tags avec leurs valeurs
        /// </summary>
        private void InitializeTagDefinitions()
        {
            // Define tag for each of the sixteen chesspiece
            for (byte k = 0; k <= 15; k++)
            {
                TagVisualizationDefinition tagDef = new TagVisualizationDefinition();

                // The tag value that this definition will respond to.
                tagDef.Value = k;

                // The .xaml file for the UI
                tagDef.Source =
                    new Uri("CameraVisualization.xaml", UriKind.Relative);

                // The maximum number for this tag value.
                tagDef.MaxCount = 1;

                // The visualization stays for 2 seconds.
                tagDef.LostTagTimeout = 2000.0;

                // Orientation offset (default).
                //tagDef.OrientationOffsetFromTag = 0.0;

                // Physical offset (horizontal inches, vertical inches).
                //tagDef.PhysicalCenterOffsetFromTag = new Vector(2.0, 2.0);

                // Tag removal behavior (default).
                // https://msdn.microsoft.com/en-us/library/microsoft.surface.presentation.controls.tagremovedbehavior.aspx
                tagDef.TagRemovedBehavior = TagRemovedBehavior.Wait;

                // Orient UI to tag? (default).
                tagDef.UsesTagOrientation = false;

                // Add the definition to the collection.
                TagVisualizer.Definitions.Add(tagDef);
            }
        }

        #endregion

        // --------------------------------------------------------------------
        // UI UPDATE
        // --------------------------------------------------------------------
        #region UI update
        /// <summary>
        /// 
        /// </summary>
        private void UpdateWindow()
        {
            SetChessboardGrid();
            if (_isCpuGame == false && _chessBoard.LastBoard == null)
                undoButton1.IsEnabled = false;
            else if (_isCpuGame == true && (_chessBoard.LastBoard == null ||
                _chessBoard.LastBoard.LastBoard == null))
                undoButton1.IsEnabled = false;
            else
                undoButton1.IsEnabled = true;
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetChessboardGrid()
        {
            int i, j;

            _windowBoard.SetBoard(_chessBoard);
            ChessBoardGrid.Children.Clear();
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    ChessBoardGrid.Children.Add(_windowBoard.Pieces[i, j].DisplayControl);
                }
            }
        }

        #endregion


        // --------------------------------------------------------------------
        // CHESS MOVE
        // --------------------------------------------------------------------
        #region Chess move

        /// <summary>
        /// Handler of player chess move
        /// </summary>
        /// <param name="fromRow"></param>
        /// <param name="fromColumn"></param>
        /// <param name="toRow"></param>
        /// <param name="toColumn"></param>
        /// <returns></returns>
        bool _windowBoard_Chosen_Cpu(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            byte fromSquare = (byte)(fromRow * 16 + fromColumn);
            byte toSquare = (byte)(toRow * 16 + toColumn);

            _chessBoard.MovedFromSquare = fromSquare;
            _chessBoard.MovedToSquare = toSquare;
            _log.AddString("[Blanc] " + _chessBoard.Pieces[_chessBoard.MovedFromSquare].type + "_" + getMoveStr(_chessBoard.MovedFromSquare, _chessBoard.MovedToSquare));
            string pieceEnglishName = Piece.translatePieceNameFromFrenchToEnglish(_chessBoard.Pieces[_chessBoard.MovedFromSquare].type);
            _speechSynthesizer.Speak(pieceEnglishName +" "+ getMoveFromByte(_chessBoard.MovedToSquare));
            EventTextBlock.Text = _log.getFullLog();

            // Send kill request via bluetooth. 
            if (_areDevicesPaired) 
                _bluetoothManager.Send("KILL_" + getMoveFromByte(_chessBoard.MovedToSquare));

            if (CancelMove(fromSquare) == true)
                return false;

            if (_validator.IsMoveIn(fromSquare, toSquare) == false)
                return false;

            _chessBoard = _chessBoard.Move(fromSquare, toSquare);
            _validator = new MoveValidator(_chessBoard);
            _validator.Validate();
            if (_chessBoard.IsBoardValid() == false)
            {
                _chessBoard = _chessBoard.LastBoard;
                _validator = new MoveValidator(_chessBoard);
                _validator.Validate();
                return false;
            }
            Evaluator.Evaluate(_chessBoard, _validator);

            UpdateWindow();

            if (_chessBoard.IsDraw() == true)
            {
                MessageBox.Show("Draw");
                _windowBoard.Freeze();
                return true;
            }
            else if (_chessBoard.IsBlancMated == true || _chessBoard.IsNoirMated == true)
            {
                MessageBox.Show("Checkmate");
                _windowBoard.Freeze();
                return true;
            }

            CpuMove();

            return true;
        }


        /// <summary>
        /// CPU Move
        /// </summary>
        private void CpuMove()
        {
            Action action = new Action(delegate()
            {
                bool finished = false;

                Action refreshAction = new Action(delegate()
                {
                    SearchProgressBar.Value++;
                });

                _engine.ProgressChanged += delegate()
                {
                    Dispatcher.BeginInvoke(refreshAction);
                };

                _chessBoard = _engine.CpuMove(_chessBoard, _validator);

                _validator = new MoveValidator(_chessBoard, true);
                _validator.Validate();

                // Get CPU move as a string
                string move = getMoveStr(_chessBoard.CpuMovedFromSquare, _chessBoard.CpuMovedToSquare);
                
                // Send it via bluetooth
                if (_areDevicesPaired) 
                    _bluetoothManager.Send(move);

                // Update Log UI
                EventLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate()
                    {
                        _log.AddString("[Noir] " + _chessBoard.Pieces[_chessBoard.CpuMovedToSquare].type + "_" + move);
                        string pieceEnglishName = Piece.translatePieceNameFromFrenchToEnglish(_chessBoard.Pieces[_chessBoard.CpuMovedToSquare].type);
                        _speechSynthesizer.SpeakAsync(pieceEnglishName + " " + getMoveFromByte(_chessBoard.CpuMovedToSquare));
                        EventTextBlock.Text = _log.getFullLog();
                    }
                 ));


                if (_chessBoard.IsDraw() == true)
                {
                    MessageBox.Show("Draw");
                    finished = true;
                }
                else if (_chessBoard.IsBlancMated == true || _chessBoard.IsNoirMated == true)
                {
                    MessageBox.Show("Checkmate");
                    finished = true;
                }

                Action act = new Action(delegate()
                {
                    UpdateWindow();
                    if (finished == true)
                        _windowBoard.Freeze();
                });
                Dispatcher.BeginInvoke(act);



                _engine.RemoveHandler();

            });

            SearchProgressBar.Maximum = _validator.ValidMoves.Count;
            SearchProgressBar.Visibility = System.Windows.Visibility.Visible;
            SearchProgressBar.Value = 0;
            _windowBoard.Freeze();
            _thread = new Thread(new ThreadStart(action));
            _thread.Start();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromSquare"></param>
        /// <returns></returns>
        private bool CancelMove(byte fromSquare)
        {
            if (_chessBoard.Pieces[fromSquare].type == typePiece.Rien
                || _chessBoard.Pieces[fromSquare].color != _chessBoard.NowPlays)
                return true;

            return false;
        }

        #endregion


        // --------------------------------------------------------------------
        // LOG FUNCTIONS
        // --------------------------------------------------------------------
        #region Log functions

        /// <summary>
        /// Traduit un move entier (from/to) en langage "échec"
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public string getMoveStr(byte from, byte to)
        {
            string fromStr = getMoveFromByte(from);
            string toStr = getMoveFromByte(to);

            return fromStr + toStr;
        }


        /// <summary>
        /// Traduit un byte correspond à un move en langage des "échecs" Ex : 19 -> D3
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public string getMoveFromByte(byte move)
        {
            Dictionary<int, string> columnsDictionary = new Dictionary<int, string>() {
                {0, "A"},
                {1, "B"},
                {2, "C"},
                {3, "D"},
                {4, "E"},
                {5, "F"},
                {6, "G"},
                {7, "H"}
            };
            string str = "";
            int row = 8 - move / 16;
            int column = move % 16;

            columnsDictionary.TryGetValue(column, out str);

            return str = str + row;
        }

        #endregion

        private void UndoChosenMoveButton_TouchDown(object sender, RoutedEventArgs e)
        {
            if (this._mode.Equals(AppMode.PLAYING))
            {
                if (isMoving)
                {
                    _windowBoard.CancelCurrentMove();
                    isMoving = false;
                }
            }
        }

    } // End (class)
} // End (namespace)