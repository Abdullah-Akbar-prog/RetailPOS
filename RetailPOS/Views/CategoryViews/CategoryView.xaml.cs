using RetailPOS.ViewModels;
using System.Windows.Controls;

namespace RetailPOS.Views
{
    /// <summary>
    /// Interaction logic for CategoryView.xaml
    /// </summary>
    public partial class CategoryView : UserControl
    {
        private readonly CategoryViewModel _viewModel;

        public CategoryView(CategoryViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += CategoryView_Loaded;
        }

        private async void CategoryView_Loaded(
            object sender,
            System.Windows.RoutedEventArgs e)
        {
            Loaded -= CategoryView_Loaded;
            await _viewModel.LoadAsync();
        }
    }
}
