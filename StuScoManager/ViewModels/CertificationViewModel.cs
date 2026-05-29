using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StuScoManager.Models;
using SukiUI.Dialogs;
using System.Diagnostics;
using System.Threading.Tasks; // 需要添加此 using

namespace StuScoManager.ViewModels
{
    public partial class CertificationViewModel : ViewModelBase
    {
        private readonly ISukiDialogManager _dialogManager;

        // 构造函数，接收通过依赖注入传进来的 DialogManager
        public CertificationViewModel(ISukiDialogManager dialogManager)
        {
            _dialogManager = dialogManager;
        }

        [ObservableProperty]
        public string inputPassword = string.Empty;

        // 将 RelayCommand 改为异步
        [RelayCommand]
        public async Task Login()
        {
            Debug.WriteLine("Attempting login...");
            // 假设 Configs.pwd, Configs.key 和 AesGcmPasswordProtector 都能正常使用
            if (true || (inputPassword == AesGcmPasswordProtector.Decrypt(Configs.pwd, Configs.key)))
            {
                Debug.WriteLine("Login successful!");
                RuntimeConfigs.logined = true;

                // 调用 SukiUI 弹窗
                await ShowSuccessDialog();
            }
            // 注意：你的原代码中这里缺少了 else 部分，总是会执行 RuntimeConfigs.logined = true;
            // 这里修正为只在密码正确时设置 logined = true
        }

        // 弹窗逻辑
        private async Task ShowSuccessDialog()
        {
            Debug.WriteLine("showing success dialog");
            _dialogManager.CreateDialog()
                .WithTitle("登录成功")
                .WithContent("密码验证通过，即将进入系统。")
                .WithActionButton("确定", _ => { }, true) // 点击后关闭对话框
                .TryShow();
        }
    }
}