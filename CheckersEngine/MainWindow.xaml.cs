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
        private Level m_Level;
        private Player m_Player;

        public MainWindow()
        {
            InitializeComponent();

            m_Player = Player.Blue;
            m_Level = Level.Medium;
        }

        private void RulesButton_Click(object sender, RoutedEventArgs e)
        {
            RulesWindow rulesWindow = new RulesWindow();
            rulesWindow.Show();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            CheckersUserControl.Visibility = Visibility.Visible;

            CheckersUserControl.StartPlay(m_Player, m_Level);
        }

        private void GameOptinsClick(object sender, RoutedEventArgs e)
        {
            OptionsWindow optionsWindow = new OptionsWindow(m_Level, m_Player);
            optionsWindow.OkClicked += OptionsWindow_OkClicked;
            optionsWindow.Closed += OptionsWindow_Closed;
            optionsWindow.Show();
            this.IsEnabled = false;
        }

        private void OptionsWindow_Closed(object sender, EventArgs e)
        {
            OptionsWindow optionsWindow = sender as OptionsWindow;
            if (optionsWindow == null)
                return;

            optionsWindow.OkClicked -= OptionsWindow_OkClicked;
            optionsWindow.Closed -= OptionsWindow_Closed;
            this.IsEnabled = true;
        }

        private void OptionsWindow_OkClicked(object sender, OptionsEvetArgs e)
        {
            m_Player = e.Player;
            m_Level = e.Level;
        }
    }
}
