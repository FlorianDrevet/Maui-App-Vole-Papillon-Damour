using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShopAppVpd.Apis.Enums;
using ShopAppVpd.Dtos;
using ShopAppVpd.Interfaces;

namespace ShopAppVpd.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IProductService _productService;

    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<Product> FilteredProducts { get; } = new();

    public List<Category> Categories { get; } = Constants.Categories.All;

    [ObservableProperty] private Category _selectedCategory = Constants.Categories.Buvette;

    public MainViewModel(IProductService productService)
    {
        _productService = productService;

        LoadProductsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadProductsAsync(CancellationToken cancellationToken)
    {
        var products = await _productService.GetProductsAsync(cancellationToken);

        Products.Clear();
        foreach (var p in products)
            Products.Add(p);

        ApplyFilter(ProductSection.Bar);
    }
    
    [RelayCommand]
    private void SetSelectedCategory(Category category)
    {
        SelectedCategory = category;
    }

    partial void OnSelectedCategoryChanged(Category value)
    {
        ApplyFilter(value.Section);
    }

    private void ApplyFilter(ProductSection section)
    {
        FilteredProducts.Clear();

        var filtered = Products.Where(p => p.ProductSection == section.ToString());

        foreach (var p in filtered)
            FilteredProducts.Add(p);
    }
}