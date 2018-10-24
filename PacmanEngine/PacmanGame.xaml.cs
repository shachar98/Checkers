using GameEnginesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacmanEngine
{
    /// <summary>
    /// Interaction logic for PacmanGame.xaml
    /// </summary>
    public partial class PacmanGame : UserControl
    {
        PacmanGameState m_Game = new PacmanGameState();
        Timer m_Timer = new Timer(500);
        bool m_IsPaint = false;
        public PacmanGame()
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
            m_Game = new AlphaBetaAlgorithem(11).GetNextMove(m_Game) as PacmanGameState;
            this.Dispatcher.Invoke(() => InvalidateVisual());
            m_IsPaint = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var foodImage = new BitmapImage(new Uri(@"C:\MyProjects\GamesEngine\PacmanEngine\bin\Debug\Resources\Food.png"));
            var ghostImage = new BitmapImage(new Uri(@"C:\MyProjects\GamesEngine\PacmanEngine\bin\Debug\Resources\ghost.png"));
            var wallmage = new BitmapImage(new Uri(@"C:\MyProjects\GamesEngine\PacmanEngine\bin\Debug\Resources\wall.png"));
            var PacmanImage = new BitmapImage(new Uri(@"C:\MyProjects\GamesEngine\PacmanEngine\bin\Debug\Resources\pcaman.png"));
            var emptyImage = new BitmapImage(new Uri(@"C:\MyProjects\GamesEngine\PacmanEngine\bin\Debug\Resources\Empty.png"));

            for (int row = 0; row < m_Game.Board.GetLength(0); row++)
            {
                for (int col = 0; col < m_Game.Board.GetLength(1); col++)
                {
                    Rect rect = new Rect(col * 30, row * 30, 30, 30);
                    switch (m_Game.Board[row, col])
                    {
                        case PacmanCell.Food:
                            drawingContext.DrawImage(foodImage, rect);
                            break;
                        case PacmanCell.Ghost:
                            drawingContext.DrawImage(ghostImage, rect);
                            break;
                        case PacmanCell.Pacman:
                            drawingContext.DrawImage(PacmanImage, rect);
                            break;
                        case PacmanCell.Wall:
                            drawingContext.DrawImage(wallmage, rect);
                            break;
                        case PacmanCell.Empty:
                            drawingContext.DrawImage(emptyImage, rect);
                            break;
                        default:
                            throw new Exception();
                    }
                }
            }

            base.OnRender(drawingContext);
        }
    }
}
