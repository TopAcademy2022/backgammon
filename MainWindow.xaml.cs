using System.Windows;

namespace BackGammon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameGrid _mainGrid;

        public MainWindow()
        {
            InitializeComponent();
            this._mainGrid = new GameGrid(this);
            this._mainGrid.RenderGameField();
        }
    }
}