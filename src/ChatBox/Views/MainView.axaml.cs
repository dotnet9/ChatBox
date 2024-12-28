using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ChatBox.Desktop;
using ChatBox.Pages;
using ChatBox.Service;
using ChatBox.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBox.Views;

public partial class MainView : UserControl
{
    private readonly SettingService _settingService;
    private WindowNotificationManager _notificationManager;

    public MainView()
    {
        InitializeComponent();

        DataContext = HostApplication.Services.GetRequiredService<MainViewModel>();
        _settingService = HostApplication.Services.GetRequiredService<SettingService>();
    }

    private MainViewModel ViewModel => (MainViewModel)DataContext;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _notificationManager = new WindowNotificationManager(HostApplication.Services.GetRequiredService<MainWindow>())
        {
            Position = NotificationPosition.TopRight,
            MaxItems = 4,
            Margin = new Thickness(0, 0, 15, 40)
        };

        var setting = _settingService.LoadSetting();

        ViewModel.IsLogin = !string.IsNullOrEmpty(setting.ApiKey);
    }

    private void NvSample_OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem is NavigationViewItem item)
        {
            if (item.Tag.ToString().Equals("设置", StringComparison.OrdinalIgnoreCase))
            {
                ViewModel.OnNavigation(ViewModel, MenuKeys.MenuKeySetting);
                return;
            }

            ViewModel.OnNavigation(ViewModel, item.Tag.ToString());
        }
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        CrossPlatformCustomProtocolHelper.OpenCustomProtocolUrl();

        _settingService.FileChange(() =>
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                // 登录成功
                _notificationManager?.Show(
                    new Notification("登录成功", "登录成功", NotificationType.Success));

                ViewModel.IsLogin = true;
            });
        });
    }
}