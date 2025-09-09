using CommunityToolkit.Mvvm.ComponentModel;
using ShopAppVpd.Dtos;
using ShopAppVpd.Interfaces;
using ShopAppVpd.Services;
using ShopAppVpd.ViewModels;

namespace ShopAppVpd;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}