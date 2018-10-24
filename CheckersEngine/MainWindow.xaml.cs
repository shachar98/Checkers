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
            MenuGrid.Visibility = Visibility.Hidden;

            Player player = RedPlayer.IsChecked.Value ? Player.Black : Player.White;
            Level level = Easy.IsChecked.Value ? Level.Easy :
                          Medium.IsChecked.Value ? Level.Medium : Level.High;


            CheckersUserControl.StartPlay(player, level);
            CheckersUserControl.GameFinished += CheckersUserControl_GameFinished;
        }

        private void CheckersUserControl_GameFinished(object sender, EventArgs e)
        {
            CheckersUserControl.Visibility = Visibility.Hidden;
            MenuGrid.Visibility = Visibility.Visible;

            CheckersUserControl.GameFinished -= CheckersUserControl_GameFinished;
        }
    }
}
