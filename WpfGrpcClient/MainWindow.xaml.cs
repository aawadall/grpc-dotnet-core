using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfGrpcClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GrpcClient Client { get; }
        public MainWindow()
        {
            InitializeComponent();
            Client = new GrpcClient();
            SslStatus.Content = Client.SslReady ? "SSL Success" : "SSL Failed";
            SslStatus.Foreground = Client.SslReady ? Brushes.Green : Brushes.Red;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Client.Connect();
            HandleConnectedEvent();
            
        }

        private void HandleConnectedEvent()
        {
            ConnectionStatus.Content = Client.Connected ? "Connected" : "Disconnected";
            ConnectionStatus.Foreground = Client.Connected ? Brushes.Green : Brushes.Red;
            Commands.IsEnabled = true;
            Arguments.IsEnabled = true;
        }

        private void GetByBadgeNumber_Click(object sender, RoutedEventArgs e)
        {
            if(Client.Connected)
            {
                try
                {
                    var badgeNumber = int.Parse(BadgeNumber.Text);
                    Client.GetByBadgeAsync(badgeNumber).Wait();
                    MessageBox.Show(Client.EResult.Employee.FirstName, "WPF");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Something went wrong {ex.Message}", "Uh Oh WPF");
                }
            }
        }
    }
}
