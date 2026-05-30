using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StuScoManager.Models;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Diagnostics;
using System.Threading.Tasks; // 需要添加此 using

namespace StuScoManager.ViewModels
{
    // 消息类，可以携带登录状态
    public class LoginStatusChangedMessage(bool isLoggedIn)
    {
        public bool IsLoggedIn { get; } = isLoggedIn;
    }
    public partial class CertificationViewModel : ViewModelBase
    {
        private readonly ISukiDialogManager _dialogManager;
        public CertificationViewModel(ISukiDialogManager dialogManager)
        {
            _dialogManager = dialogManager;
        }

        [ObservableProperty]
        public string inputPassword = string.Empty;

        [RelayCommand]
        public async Task Login()
        {
            Debug.WriteLine("Attempting login...");
            if (InputPassword == AesGcmPasswordProtector.Decrypt(Configs.pwd, Configs.key))
            {
                Debug.WriteLine("Login successful!");
                RuntimeConfigs.logined = true;

                WeakReferenceMessenger.Default.Send(new LoginStatusChangedMessage(true));
                // 调用 SukiUI 对话框显示登录成功消息
                await ShowSuccessToast("成功！", "密码验证通过，学生管理通道已打开~");
            }
        }

        private static async Task ShowSuccessToast(string title = "Success", string content = "Default content.")
        {
            Debug.WriteLine("showing success dialog");
            StudentManageViewModel.ToastManager.CreateToast()
                .WithTitle(title)
                .OfType(Avalonia.Controls.Notifications.NotificationType.Success)
                .WithContent(content)
                .Dismiss().After(TimeSpan.FromSeconds(3))
                .Dismiss().ByClicking()
                .Queue();
        }
    }
}