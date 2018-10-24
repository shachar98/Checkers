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

namespace StrategoEngine
{
    /// <summary>
    /// Interaction logic for StrategoUserControl.xaml
    /// </summary>
    public partial class StrategoUserControl : UserControl
    {
        private GameEngine m_GameEngine = new GameEngine();
        Timer m_Timer = new Timer(1000);
        bool m_IsPaint = false;
        private Player m_PlayerTurn = Player.White;

        public StrategoUserControl()
        {
            InitializeComponent();

            m_Timer.Enabled = true;
            m_Timer.Elapsed += M_Timer_Elapsed;
            m_Timer.Start();
        }

        private void M_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_IsPaint)
                return;
            m_IsPaint = true;
            m_GameEngine.Play(m_PlayerTurn);
            m_PlayerTurn = m_PlayerTurn == Player.White ? Player.Black : Player.White;
            this.Dispatcher.Invoke(() => InvalidateVisual());
            m_IsPaint = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Dictionary<PieceType, ImageSource> piecesImages = GetPiecesImages();
            for (int row = 0; row < m_GameEngine.Board.GetLength(0); row++)
            {
                for (int col = 0; col < m_GameEngine.Board.GetLength(1); col++)
                {
                    Rect rect = new Rect(col * 30, row * 30, 30, 30);
                    GameCell gameCell = m_GameEngine.Board[row, col];
                    if (gameCell.IsEmpty)
                        drawingContext.DrawImage(GetImageSourceFromBitMap(Properties.Resources.Empty), rect);
                    else if (gameCell.IsWater)
                        drawingContext.DrawImage(GetImageSourceFromBitMap(Properties.Resources.Water), rect);
                    else
                        drawingContext.DrawImage(piecesImages[gameCell.Piece.PieceType], rect);
                }
            }

            base.OnRender(drawingContext);
        }

        private Dictionary<PieceType, ImageSource> GetPiecesImages()
        {
            return new Dictionary<PieceType, ImageSource>()
            {
                { PieceType.Bomb, GetImageSourceFromBitMap(Properties.Resources.Bomb) },
                { PieceType.Flag, GetImageSourceFromBitMap(Properties.Resources.Flag) },
                { PieceType.One, GetImageSourceFromBitMap(Properties.Resources.One) },
                { PieceType.Two, GetImageSourceFromBitMap(Properties.Resources.Two) },
                { PieceType.Three, GetImageSourceFromBitMap(Properties.Resources.Three) },
                { PieceType.Four, GetImageSourceFromBitMap(Properties.Resources.Four) },
                { PieceType.Five, GetImageSourceFromBitMap(Properties.Resources.Five) },
                { PieceType.Six, GetImageSourceFromBitMap(Properties.Resources.Six) },
                { PieceType.Seven, GetImageSourceFromBitMap(Properties.Resources.Seven) },
                { PieceType.Eight, GetImageSourceFromBitMap(Properties.Resources.Eight) },
                { PieceType.Nine, GetImageSourceFromBitMap(Properties.Resources.Nine) },
                { PieceType.Ten, GetImageSourceFromBitMap(Properties.Resources.Ten) },
            };
        }

        private ImageSource GetImageSourceFromBitMap(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
