using System.Collections.ObjectModel;
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
    public ObservableCollection<Category> Categories { get; } = new();

    [ObservableProperty]
    private Category selectedCategory;

    public MainViewModel(IProductService productService)
    {
        _productService = productService;

        Categories.Add(new Category { Name = "Buvette", Icon = "drink.png", Section = ProductSection.Bar});
        Categories.Add(new Category { Name = "Loto", Icon = "loto.png", Section = ProductSection.Bingo});
        Categories.Add(new Category { Name = "Livres", Icon = "book.png", Section = ProductSection.Book});

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