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
using System.Windows.Shapes;

namespace CheckersEngine
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow(Level level, Player player)
        {
            InitializeComponent();

            if (level == Level.Easy)
            {
                EasyLevelButton.IsChecked = true;
            }
            else if (level == Level.Medium)
            {
                MediumLevelButton.IsChecked = true;
            }
            else 
            {
                HardLevelButton.IsChecked = true;
            }

            if (player == Player.Blue)
            {
                BluePlayerButton.IsChecked = true;
            }
            else
            {
                RedPlayerButton.IsChecked = true;
            }
        }

        public event EventHandler<OptionsEvetArgs> OkClicked;

        private void CancellButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            Level level;
            if (EasyLevelButton.IsChecked.Value)
            {
                level = Level.Easy;
            }
            else if (EasyLevelButton.IsChecked.Value)
            {
                level = Level.Medium;
            }
            else
            {
                level = Level.Hard;
            }

            Player player;
            if (BluePlayerButton.IsChecked.Value)
            {
                player = Player.Blue;
            }
            else
            {
                player = Player.Red;
            }

            OkClicked?.Invoke(this, new OptionsEvetArgs(level, player));
            this.Close();
        }
    }

    public class OptionsEvetArgs : EventArgs
    {
        public OptionsEvetArgs(Level level, Player player)
        {
            Level = level;
            Player = player;
        }

        public Level Level { get; }
        public Player Player { get; }
    }
}
