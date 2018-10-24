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

namespace CheckersEngine
{
    /// <summary>
    /// Interaction logic for CheckersUserControl.xaml
    /// </summary>
    public partial class CheckersUserControl : UserControl
    {
        private GameEngine m_GameEngine = new GameEngine();
        Timer m_Timer = new Timer(1000);
        bool m_IsPlaying = false;
        bool m_FinishRender = false;
        private Player m_PlayerTurn = Player.White;

        public CheckersUserControl()
        {
            InitializeComponent();

            m_Timer.Enabled = true;
            m_Timer.Elapsed += M_Timer_Elapsed;
            m_Timer.Start();
        }

        private void M_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!m_FinishRender)
            {
                this.Dispatcher.Invoke(() => InvalidateVisual());
                return;
            }

            if (m_IsPlaying)
                return;
            m_IsPlaying = true;
            var state = m_GameEngine.Play(m_PlayerTurn);
            var otherPlayer = m_PlayerTurn == Player.White ? Player.Black : Player.White;

            if (state == null)
            {
                m_Timer.Enabled = false;
                MessageBox.Show("Player win: " + otherPlayer.ToString());
            }

            m_PlayerTurn = otherPlayer;
            this.Dispatcher.Invoke(() => InvalidateVisual());
            m_IsPlaying = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
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

            base.OnRender(drawingContext);
        }

        private void AddButton(int row, int col, Piece[,] currBoard)
        {
            Button button = CreateButton(row, col);

            FillButtonWithImage(row, col, button, currBoard);

            button.PreviewMouseDown += Button_PreviewMouseDown;
            button.PreviewMouseUp += Button_PreviewMouseUp;

            this.GameCanvas.Children.Add(button);
        }

        private void RemoveOldButtons()
        {
            foreach (var currButton in GameCanvas.Children.OfType<Button>())
            {
                currButton.PreviewMouseDown += Button_PreviewMouseDown;
                currButton.PreviewMouseUp += Button_PreviewMouseUp;
            }
            this.GameCanvas.Children.Clear();
        }

        private static Button CreateButton(int row, int col)
        {
            Button button = new Button()
            {
                Height = 20,
                Width = 20,
                Margin = new Thickness(col * 20, row * 20, 0, 0)
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
                if (board[row, col]?.Player == Player.White)
                    FillButtonWithImage(button, Properties.Resources.BluePlayer);
                else if (board[row, col]?.Player == Player.Black)
                    FillButtonWithImage(button, Properties.Resources.RedPlayer);
            }
            else 
            {
                if (board[row, col]?.Player == Player.White)
                    FillButtonWithImage(button, Properties.Resources.BluePlayerQueen);
                else if (board[row, col]?.Player == Player.Black)
                    FillButtonWithImage(button, Properties.Resources.RedPlayerQueen);
            }
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
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
    }
}
