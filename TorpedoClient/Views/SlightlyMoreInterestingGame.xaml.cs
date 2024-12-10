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
using TorpedoCommon;
using TorpedoCommon.MessageTypes;

namespace TorpedoClient.Views;
/// <summary>
/// Interaction logic for SlightlyMoreInterestingGame.xaml
/// </summary>
public partial class SlightlyMoreInterestingGame : Page
{
    private WebsocketClient _client;
    private TorpedoCommon.Game game;

    private bool isPlayer1;

    public SlightlyMoreInterestingGame(WebsocketClient client, TorpedoCommon.Game game)
    {
        InitializeComponent();

        _client = client;
        this.game = game;

        isPlayer1 = client.Username == game.Player1Name;

        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                Border border = new()
                {
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Tag = Tuple.Create(i, j) // Store a Tuple<int, int> instead of Point
                };
                player.Children.Add(border);
                Grid.SetColumn(border, i);
                Grid.SetRow(border, j);

                Button btn = new()
                {
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Tag = Tuple.Create(i, j) // Store a Tuple<int, int> instead of Point
                };
                btn.Click += (_, _) => Click(btn);
                btn.IsEnabled = false;
                opp.Children.Add(btn);
                Grid.SetColumn(btn, i);
                Grid.SetRow(btn, j);
            }
        }

        UpdateState(game);

        _client.onGameStateUpdated += (upd => UpdateState(upd.GameState));

        _client.onGameOver += (winner) =>
        {
            MainWindow.ChangeView(new Lobby(client));
            MessageBox.Show($"Game over! {winner} won!");
        };
    }

    private void UpdateState(TorpedoCommon.Game newState) {
        game = newState;
        lblStatus.Content = game.isPlayer1Next == isPlayer1 ? "Your turn" : "Waiting for other player...";

        (isPlayer1 ? game.Player1Ships : game.Player2Ships).ForEach(s => ColorSelf(s.Item1, s.Item2, Brushes.LightBlue));

        (isPlayer1 ? game.Player2Shots : game.Player1Shots)
            .ForEach(s => ColorSelf(s.Item1, s.Item2, (isPlayer1 ? game.Player1Ships : game.Player2Ships).Contains(s) ? Brushes.Red : Brushes.Gray));

        (isPlayer1 ? game.Player1Shots : game.Player2Shots)
            .ForEach(s => ColorOpponent(s.Item1, s.Item2, (isPlayer1 ? game.Player2Ships : game.Player1Ships).Contains(s) ? Brushes.Red : Brushes.Gray));

        opp.Children.Cast<UIElement>().Where(x => x is Button).ToList().ForEach(x => x.IsEnabled = game.isPlayer1Next == isPlayer1);

        this.game = game;
    }

    async void Click(Button btn)
    {
        int x = Grid.GetColumn(btn);
        int y = Grid.GetRow(btn);

        await _client.SendMessage(new ShootMessage() {GameId = game.Id, X = x, Y = y});
    }

    void ColorSelf(int x, int y, Brush color)
    {
        player.Children
            .Cast<Border>()
            .First(G => Grid.GetColumn(G) == x && Grid.GetRow(G) == y)
            .Background = color;
    }

    void ColorOpponent(int x, int y, Brush color)
    {
        UIElement el = opp.Children.Cast<UIElement>().First(G => Grid.GetColumn(G) == x && Grid.GetRow(G) == y);
        if (el is not Button) {
            return;
        }

        Border border = new()
        {
            Background = color,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
        };

        opp.Children.Add(border);
        Grid.SetColumn(border, x);
        Grid.SetRow(border, y);
        opp.Children.Remove(el);
    }
}
