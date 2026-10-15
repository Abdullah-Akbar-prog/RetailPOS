using RetailPOS.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RetailPOS.Views
{
    /// <summary>
    /// Interaction logic for BrandView.xaml
    /// </summary>
    public partial class BrandView : UserControl
    {
        private readonly BrandViewModel _viewModel;
        public BrandView(BrandViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += BrandView_Loaded;
        }

        private async void BrandView_Loaded(
         object sender, RoutedEventArgs e)
        {
            Loaded -= BrandView_Loaded;
            await _viewModel.LoadAsync();

        }
    }
}
