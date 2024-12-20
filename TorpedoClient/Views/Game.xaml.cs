﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System.Threading;
using System.Text.Json.Serialization;
using TorpedoCommon.MessageTypes;

namespace TorpedoClient.Views
{
    public partial class Game : Page
    {
        private WebsocketClient _client;
        private TorpedoCommon.Game game;
        private bool inSetup = true;

        private string? _selectedShip;
        private readonly int[,] _gridState = new int[10, 10]; // 0 = empty, 1 = occupied
        private List<Border> highlightedBorders = new List<Border>();
        private List<Tuple<int, int>> placedShips = new List<Tuple<int, int>>();
        private static readonly Dictionary<string, int> ShipLength = new Dictionary<string, int>
        {
            { "Aircraft Carrier", 5 },
            { "Battleship", 4 },
            { "Submarine", 3 },
            { "Cruiser", 3 },
            { "Destroyer", 2 }
        };

        private bool _isVerticalPlacement = false; // Track if ship is placed vertically or horizontally
        private HashSet<string> placedShipTypes = new HashSet<string>(); // Track placed ship types

        public Game(WebsocketClient client, TorpedoCommon.Game game)
        {
            _client = client;
           this.game = game;

            InitializeComponent();

            // Initialize the grid
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Border border = new()
                    {
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Tag = Tuple.Create(i, j) // Store a Tuple<int, int> instead of Point
                    };
                    grid.Children.Add(border);
                    Grid.SetColumn(border, i);
                    Grid.SetRow(border, j);
                }
            }

            lblStatus.Content = "Place your ships";

            _client.onPlayerListRecieved += (List<string> players) =>
            {
                if (!players.Contains(game.Player1Name) || !players.Contains(game.Player2Name))
                {
                    MainWindow.ChangeView(new Views.Lobby(client, players));
                    MessageBox.Show("Other player disconnected!");
                }
            };

            _client.onGameStateUpdated += (GameStateUpdate update) =>
            {
                this.game = update.GameState;
                if (inSetup && !this.game.SetupPhase) {
                    inSetup = false;
                    MainWindow.ChangeView(new SlightlyMoreInterestingGame(_client, this.game));
                }
            };
        }

        // Handle ship button click to select ship type
        private void ShipButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string ship)
            {
                if (placedShipTypes.Contains(ship)) return; // Prevent placing the same ship type more than once

                _selectedShip = ship;
                SelectedShipLabel.Content = $"Selected ship: {_selectedShip}"; // Update UI with selected ship
            }
        }

        // Handle mouse click on grid to place ship
        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_selectedShip == null) return;

            var position = e.GetPosition(grid);
            int column = (int)(position.X / (grid.ActualWidth / grid.ColumnDefinitions.Count));
            int row = (int)(position.Y / (grid.ActualHeight / grid.RowDefinitions.Count));

            // Ensure the ship fits within the grid
            if ((_isVerticalPlacement ? row + ShipLength[_selectedShip] : column + ShipLength[_selectedShip]) > 10) return;

            // Check if the ship can be placed at the desired location
            if (CanPlaceShip(row, column))
            {
                // Place the ship and highlight the occupied cells
                PlaceShip(row, column);
                placedShipTypes.Add(_selectedShip); // Mark the ship type as placed
                shipSelectionPanel.Children
                    .OfType<Button>()
                    .FirstOrDefault(G => G.Content.ToString() == _selectedShip)
                    .IsEnabled = false;
                _selectedShip = null;
            }

            if(placedShipTypes.Count == ShipLength.Count)
                ReadyBtn.IsEnabled = true;
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Ensure there is a selected ship before proceeding
            if (_selectedShip == null)
                return;

            // Get the position where the right-click occurred on the grid
            var position = e.GetPosition(grid);
            int row = (int)(position.Y / (grid.ActualHeight / 10));  // 10 rows
            int column = (int)(position.X / (grid.ActualWidth / 10)); // 10 columns

            // Check if the ship can be placed at the current position
                // Rotate the ship
                _isVerticalPlacement = !_isVerticalPlacement;
                RotationLabel.Content = $"Current Orientation: {(_isVerticalPlacement ? "Vertical" : "Horizontal")}";

            if (CanPlaceShip(row, column))
            {
                // Highlight the valid placement for the ship after rotation
                HighlightCells(row, column, Brushes.Green);

                // Optionally, place the ship after rotation if it fits
                // PlaceShip(row, column, _selectedShip); // Uncomment this if you want to auto-place after rotation

            }
            else
            {
                HighlightCells(row, column, Brushes.Red);
            }

        }

        // Handle mouse move to show valid/invalid placement for ship
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_selectedShip == null) return;

            var position = e.GetPosition(grid);
            int column = (int)(position.X / (grid.ActualWidth / grid.ColumnDefinitions.Count));
            int row = (int)(position.Y / (grid.ActualHeight / grid.RowDefinitions.Count));

            // Clear previous highlights
            ClearCellHighlights();

            // Validate if the ship can be placed at the hovered position
            if (CanPlaceShip(row, column))
            {
                // Highlight the valid cells in green
                HighlightCells(row, column, Brushes.Green);
            }
            else
            {
                // Highlight the invalid cells in red
                HighlightCells(row, column, Brushes.Red);
            }
        }

        private void ClearCellHighlights()
        {
            // Reset only the borders that were previously highlighted
            foreach (var border in highlightedBorders)
            {
                int row = Grid.GetRow(border);
                int column = Grid.GetColumn(border);

                // Only reset the border's background if the cell is not part of a placed ship
                if (_gridState[row, column] != 1)
                {
                    border.Background = Brushes.Transparent; // Remove the background color
                }
            }

            // Clear the list of highlighted borders
            highlightedBorders.Clear();
        }

        private void HighlightCells(int row, int column, Brush color)
        {
            int shipLength = ShipLength[_selectedShip!];

            for (int i = 0; i < shipLength; i++)
            {
                int checkRow = _isVerticalPlacement ? row + i : row;
                int checkColumn = _isVerticalPlacement ? column : column + i;

                // Make sure it's within the grid bounds
                if (checkRow >= 10 || checkColumn >= 10)
                    break;

                // Skip already placed cells
                if (_gridState[checkRow, checkColumn] == 1)
                    continue;

                // Find the border element at this position
                Border border = grid.Children
                    .Cast<UIElement>()
                    .FirstOrDefault(child => Grid.GetRow(child) == checkRow && Grid.GetColumn(child) == checkColumn) as Border;

                if (border != null)
                {
                    // Set the background color for the hovered cell
                    border.Background = color;

                    // Add the border to the list of highlighted borders
                    if (!highlightedBorders.Contains(border))
                    {
                        highlightedBorders.Add(border);
                    }
                }
            }
        }

        // Place the ship on the grid
        private void PlaceShip(int row, int column)
        {
            if (_selectedShip == null)
                return;

            if(CanPlaceShip(row, column))
            {
                int shipLength = ShipLength[_selectedShip];
                List<Tuple<int, int>> shipCells = new List<Tuple<int, int>>();

                for (int i = 0; i < shipLength; i++)
                {
                    int checkRow = _isVerticalPlacement ? row + i : row;
                    int checkColumn = _isVerticalPlacement ? column : column + i;

                    // Ensure it is within bounds
                    if (checkRow >= 10 || checkColumn >= 10)
                        return;

                    // Mark the cell as occupied in the grid state
                    _gridState[checkRow, checkColumn] = 1;

                    // Add the cell to the list of ship cells
                    shipCells.Add(new Tuple<int, int>(checkColumn, checkRow));
                }

                // Color the cells grey after placing the ship
                foreach (var cell in shipCells)
                {
                    int rowIndex = cell.Item2;
                    int columnIndex = cell.Item1;

                    Border border = grid.Children
                        .Cast<UIElement>()
                        .FirstOrDefault(child => Grid.GetRow(child) == rowIndex && Grid.GetColumn(child) == columnIndex) as Border;

                    if (border != null)
                    {
                        border.Background = Brushes.Gray; // Set the placed ship color to grey
                    }
                }

                // Add the ship cells to placed ships list
                foreach (var cell in shipCells)
                {
                    placedShips.Add(cell);
                }

                
            }
        }

        private bool CanPlaceShip(int row, int column)
        {
            int shipLength = ShipLength[_selectedShip!];

            // Ensure that the ship fits in the grid without going off the edges
            if ((_isVerticalPlacement ? row + shipLength : column + shipLength) > 10) return false;

            for (int i = 0; i < shipLength; i++)
            {
                int checkRow = _isVerticalPlacement ? row + i : row;
                int checkColumn = _isVerticalPlacement ? column : column + i;

                // Check if the cell is within bounds and unoccupied
                if (checkRow >= 10 || checkColumn >= 10 || _gridState[checkRow, checkColumn] != 0)
                {
                    return false;
                }

                // Check surrounding cells to ensure they're not occupied
                for (int r = checkRow - 1; r <= checkRow + 1; r++)
                {
                    for (int c = checkColumn - 1; c <= checkColumn + 1; c++)
                    {
                        if (r >= 0 && r < 10 && c >= 0 && c < 10 && _gridState[r, c] != 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void ReadyBtn_Click(object sender, RoutedEventArgs e)
        {
            _client.SendMessage(new PlaceShipsMessage() { GameId = this.game.Id, Ships = placedShips });
            ReadyBtn.IsEnabled = false;
            lblStatus.Content = "Waiting for other player...";
        }
    }
}
