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
using TorpedoCommon.MessageTypes;

namespace TorpedoClient.Views
{
    /// <summary>
    /// Interaction logic for Lobby.xaml
    /// </summary>
    public partial class Lobby : Page
    {
        WebsocketClient client;

        public Lobby(WebsocketClient client) : this(client, []) {}

        public Lobby(WebsocketClient client, List<string> initList)
        {
            InitializeComponent();
            this.client = client;

            UpdateList(initList);
            client.onPlayerListRecieved += (list => UpdateList(list));
        }

        private void UpdateList(List<string> list) {
            lbPlayers.ItemsSource = list.Where(x => x != client.Username);
        }

        private async void btnInvite_Click(object sender, RoutedEventArgs e)
        {
            if (lbPlayers.SelectedIndex != -1) {
                await client.SendMessage(new StartGameMessage() { Player2Name = lbPlayers.SelectedItem.ToString()! });
            }
        }
    }
}
