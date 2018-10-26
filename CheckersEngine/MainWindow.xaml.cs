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

namespace CheckersEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RulesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            CheckersUserControl.Visibility = Visibility.Visible;

            Player player = Player.Blue;
            Level level = Level.Medium;

            CheckersUserControl.StartPlay(player, level);
        }

        private void CheckersUserControl_GameFinished(object sender, EventArgs e)
        {
        }

        private void GameOptinsClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
