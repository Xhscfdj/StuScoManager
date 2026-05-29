using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StuScoManager.Models;
using SukiUI.Dialogs;
using System.Collections.Generic;
using System.IO;

namespace StuScoManager.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        public ISukiDialogManager _dialogManager = new SukiDialogManager();
        [ObservableProperty]
        public ObservableObject? _currentPage;
        public MainWindowViewModel()
        {
            Configs AllConfig = new();
            if (Configs.configs.Count != 2)
            {
                // 配置文件损坏，强制写入默认配置
                Configs.WriteAndSetDefaultConfig();
                Configs.WriteAndSetDefaultKey();
                // 重新读取配置文件
                Configs.ReadConfigsAndKey();

                CurrentPage = new InitViewModel();
            }
            if (Configs.inited == "true")
            {
                
                CurrentPage = new CertificationViewModel(_dialogManager);

            }
            else
            {
                CurrentPage = new InitViewModel();
            }
            //string decryptedPwd = AesGcmPasswordProtector.Decrypt(pwd, key);
        }
        [RelayCommand]
        public void SwitchToCertificationPage()
        {
            CurrentPage = new CertificationViewModel(_dialogManager);
        }
        [RelayCommand]
        public void SwitchToStudentManagePage()
        {
            CurrentPage = new StudentManageViewModel();
        }
        [RelayCommand]
        public void SwitchToInitPage()
        {
            CurrentPage = new InitViewModel();
        }
    }
}
