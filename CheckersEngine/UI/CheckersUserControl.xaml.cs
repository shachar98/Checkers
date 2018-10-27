using CheckersEngine.GameLogic;
using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CheckersEngine
{
    /// <summary>
    /// Interaction logic for CheckersUserControl.xaml
    /// </summary>
    public partial class CheckersUserControl : UserControl
    {
        private const int CELL_SIZE = 30;
        private GameEngine m_GameEngine;
        private bool m_FinishRender = false;
        private Player m_HumanPlayer;
        private Button m_SelectedButton;
        private BoardCoordinate m_SelectedButtonPosition;
        private MovesHandler m_MovesHandler;
        private WinningChecker m_WinningChecker;
        private UserGameEngine m_UserGameEngine;

        public CheckersUserControl()
        {
            InitializeComponent();

            m_MovesHandler = new MovesHandler();
            m_WinningChecker = new WinningChecker(m_MovesHandler);
            m_UserGameEngine = new UserGameEngine(m_MovesHandler);

            this.PreviewMouseDown += CheckersUserControl_MouseDown;
            this.PreviewMouseMove += CheckersUserControl_MouseMove;
            this.PreviewMouseUp += CheckersUserControl_MouseUp;

            m_GameEngine = new GameEngine(Level.Medium);
            m_HumanPlayer = Player.Blue;
        }

        public event EventHandler GameFinished;
        public void StartPlay(Player player, Level level)
        {
            m_GameEngine = new GameEngine(level);
            m_HumanPlayer = player;

            InvalidateVisual();
        }

        private void CheckWinning(Player player)
        {
            if (!m_WinningChecker.IsLost(player.GetOtherPlayer(), m_GameEngine.MoveState.CurrState))
                return;

            Dispatcher.BeginInvoke(new Action(() => MessageBox.Show($"Player {player} wins!!")));

            this.PreviewMouseDown -= CheckersUserControl_MouseDown;
            this.PreviewMouseMove -= CheckersUserControl_MouseMove;
            this.PreviewMouseUp -= CheckersUserControl_MouseUp;

            GameFinished?.Invoke(this, EventArgs.Empty);
        }

        #region Render

        protected override void OnRender(DrawingContext drawingContext)
        {
            //if (!m_IsPlaying)
            //    return;

            Piece[,] currBoard = m_GameEngine.MoveState.MidStates.FirstOrDefault();
            m_FinishRender = currBoard == null;

            if (currBoard == null)
                currBoard = m_GameEngine.MoveState.CurrState;
            else
                m_GameEngine.MoveState.MidStates?.RemoveAt(0);

            RemoveOldButtons();

            for (int row = 0; row < currBoard.GetLength(0); row++)
            {
                for (int col = 0; col < currBoard.GetLength(1); col++)
                {
                    AddButton(row, col, currBoard);
                }
            }

            if (m_FinishRender)
                CheckWinning(m_HumanPlayer.GetOtherPlayer());
            else
            {
                Task.Factory.StartNew(ContinueRender);
            }

            base.OnRender(drawingContext);
        }

        private async Task ContinueRender()
        {
            await Task.Delay(1000);
            await Dispatcher.BeginInvoke(new Action(() => this.InvalidateVisual()));
        }

        private async Task MakeComputerTurn()
        {
            await Task.Delay(500);
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                m_GameEngine.Play(m_HumanPlayer.GetOtherPlayer());
                this.InvalidateVisual();
            }));
        }

        private void AddButton(int row, int col, Piece[,] currBoard)
        {
            Button button = CreateButton(row, col);

            FillButtonWithImage(row, col, button, currBoard);

            this.GameCanvas.Children.Add(button);
        }

        private void RemoveOldButtons()
        {
            this.GameCanvas.Children.Clear();
        }

        private static Button CreateButton(int row, int col)
        {
            Button button = new Button()
            {
                Height = CELL_SIZE,
                Width = CELL_SIZE,
                Margin = new Thickness(col * CELL_SIZE, row * CELL_SIZE, 0, 0)
            };

            if ((row + col) % 2 == 0)
                button.Background = new SolidColorBrush(Colors.Black);
            else
                button.Background = new SolidColorBrush(Colors.White);
            return button;
        }

        private void FillButtonWithImage(int row, int col, Button button, Piece[,] board)
        {
            if (board[row, col] == null)
                return;

            if (board[row, col].PieceType == PieceType.Regular)
            {
                if (board[row, col]?.Player == Player.Blue)
                    FillButtonWithImage(button, Properties.Resources.BluePlayer);
                else if (board[row, col]?.Player == Player.Red)
                    FillButtonWithImage(button, Properties.Resources.RedPlayer);
            }
            else 
            {
                if (board[row, col]?.Player == Player.Blue)
                    FillButtonWithImage(button, Properties.Resources.BluePlayerQueen);
                else if (board[row, col]?.Player == Player.Red)
                    FillButtonWithImage(button, Properties.Resources.RedPlayerQueen);
            }
        }

        private void FillButtonWithImage(Button button, Bitmap bitmap)
        {
            var imageSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            var image = new System.Windows.Controls.Image
            {
                Source = imageSource,
                VerticalAlignment = VerticalAlignment.Center
            };

            button.Content = image;
        }

        #endregion

        #region Mouse Events

        private void CheckersUserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_SelectedButtonPosition == null)
                return;

            var position = e.GetPosition(this);
            int col = (int)(position.X / CELL_SIZE);
            int row = (int)(position.Y / CELL_SIZE);
            var newPosition = new BoardCoordinate(row, col);

            var board = m_GameEngine.MoveState.CurrState;
            if (m_UserGameEngine.IsValidMove(m_SelectedButtonPosition, newPosition, board, false))
            {
                board[newPosition.Row, newPosition.Col] = board[m_SelectedButtonPosition.Row, m_SelectedButtonPosition.Col];
                board[m_SelectedButtonPosition.Row, m_SelectedButtonPosition.Col] = null;

                var distance = newPosition.Substract(m_SelectedButtonPosition);
                var direction = distance.Divide(Math.Abs(distance.Row));
                var betweenCoor = m_SelectedButtonPosition.Add(direction.Multiply(Math.Abs(distance.Row) - 1));

                m_MovesHandler.ChangeToQueenIfNeeded(board, newPosition);

                bool turnFinish = true;
                if (board[betweenCoor.Row, betweenCoor.Col] != null)
                {
                    board[betweenCoor.Row, betweenCoor.Col] = null;
                    turnFinish = !m_UserGameEngine.HaveMoreMoves(board, newPosition);
                }

                if (turnFinish)
                {
                    CheckWinning(m_HumanPlayer);
                    Task.Factory.StartNew(MakeComputerTurn);
                }
            }

            m_SelectedButton = null;
            m_SelectedButtonPosition = null;
            this.InvalidateVisual();
        }

        private void CheckersUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_SelectedButton == null)
                return;

            var position = e.GetPosition(this);
            m_SelectedButton.Margin = new Thickness(position.X - CELL_SIZE / 2, position.Y - CELL_SIZE / 2, 0, 0);
        }

        private void CheckersUserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!m_FinishRender)
                return;

            var position = e.GetPosition(this);
            int row = (int)(position.Y / CELL_SIZE);
            int col = (int)(position.X / CELL_SIZE);
            Piece piece = m_GameEngine.MoveState.CurrState[row, col];
            if (piece == null || piece.Player != m_HumanPlayer)
                return;

            m_SelectedButton = GetButton(position);
            m_SelectedButtonPosition = new BoardCoordinate(row, col);
        }

        private Button GetButton(System.Windows.Point point)
        {
            int x = (int)(point.X / CELL_SIZE);
            int y = (int)(point.Y / CELL_SIZE);
            foreach (var currButton in GameCanvas.Children.OfType<Button>())
            {
                if (currButton.Margin.Left == x * CELL_SIZE &&
                    currButton.Margin.Top == y * CELL_SIZE)
                    return currButton;
            }

            return null;
        }

        #endregion
    }
}
