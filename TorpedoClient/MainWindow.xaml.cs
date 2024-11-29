using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TorpedoCommon;
using TorpedoCommon.MessageTypes;

namespace TorpedoClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WebsocketClient client;

        public MainWindow()
        {
            InitializeComponent();
            Connect();
        }

        public async Task Connect() {
            client = new();
            await client.Connect();
            await client.SendMessage(new LoginRequest() { Username = "test", Password = "asdasd"});
        }
    }
}