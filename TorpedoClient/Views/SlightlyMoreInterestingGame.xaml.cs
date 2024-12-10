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

namespace TorpedoClient.Views;
/// <summary>
/// Interaction logic for SlightlyMoreInterestingGame.xaml
/// </summary>
public partial class SlightlyMoreInterestingGame : Page
{
    public SlightlyMoreInterestingGame()
    {
        InitializeComponent();

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
                opp.Children.Add(btn);
                Grid.SetColumn(btn, i);
                Grid.SetRow(btn, j);
            }
        }
    }

    void Click(Button btn)
    {
        int x = Grid.GetColumn(btn);
        int y = Grid.GetRow(btn);

        //Marci majd itt varázsol valamit

        Border border = new()
        {
            Background = Brushes.Green,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
        };

        opp.Children.Add(border);
        Grid.SetColumn(border, x);
        Grid.SetRow(border, y);
        opp.Children.Remove(btn);
    }

    void RecieveBackShot(int x, int y)
    {
        player.Children
            .Cast<Border>()
            .First(G => Grid.GetColumn(G) == x && Grid.GetRow(G) == y)
            .Background = Brushes.Red;
    }
}
