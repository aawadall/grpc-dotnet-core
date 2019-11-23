using System.Drawing;
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
            ConnectionStatus.Content = Client.Connected ? "Connected" : "Disconnected";
            ConnectionStatus.Foreground = Client.Connected ? Brushes.Green : Brushes.Red;
        }
    }
}
