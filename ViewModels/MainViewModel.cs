﻿using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShopAppVpd.Apis.Enums;
using ShopAppVpd.Dtos;
using ShopAppVpd.Interfaces;
using ShopAppVpd.Models;

namespace ShopAppVpd.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IProductService _productService;

    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<Product> FilteredProducts { get; } = new();

    public List<Category> Categories { get; } = Constants.Categories.All;
    
    public List<FoodSection> FoodSections { get; } = Constants.FoodSections.All;

    [ObservableProperty] private Category _selectedCategory = Constants.Categories.Buvette;
    
    [ObservableProperty] private FoodSection _selectedFoodSection = Constants.FoodSections.Salt;
    public bool IsFoodSectionVisible => SelectedCategory.Section == ProductSection.Bar;

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

        ApplyFilter(SelectedCategory.Section, SelectedFoodSection.Section);
    }
    
    [RelayCommand]
    private void SetSelectedCategory(Category category)
    {
        SelectedCategory = category;
        OnPropertyChanged(nameof(IsFoodSectionVisible));
    }
    
    [RelayCommand]
    private void SetSelectedFoodSection(FoodSection foodSection)
    {
        SelectedFoodSection = foodSection;
    }

    partial void OnSelectedCategoryChanged(Category value)
    {
        ApplyFilter(value.Section, value.Section == ProductSection.Bar ? SelectedFoodSection.Section : null);
    }
    
    partial void OnSelectedFoodSectionChanged(FoodSection value)
    {
        ApplyFilter(ProductSection.Bar, value.Section);
    }

    private void ApplyFilter(ProductSection section, ProductCategory? foodSection = null)
    {
        FilteredProducts.Clear();

        var filtered = Products
            .Where(p => p.ProductSection == section.ToString())
            .Where(p => foodSection is null || p.ProductCategory == foodSection.ToString());

        foreach (var p in filtered)
            FilteredProducts.Add(p);
    }
}