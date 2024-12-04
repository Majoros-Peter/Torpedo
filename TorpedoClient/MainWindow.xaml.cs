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
using TorpedoClient.Views;

namespace TorpedoClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Page CurrentView { get; set; } = new Game();
        public static MainWindow Instance { get; private set; } = default!;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            Instance = this;
        }

        public static void ChangeView(Page page)
        {
            Instance.DataContext = null;
            Instance.CurrentView = page;
            Instance.DataContext = Instance;
        }
    }
}