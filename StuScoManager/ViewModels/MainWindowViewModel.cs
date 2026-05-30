using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StuScoManager.Models;
using SukiUI.Dialogs;

namespace StuScoManager.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        public bool _isStudentManagePageVisible = false;
        public static ISukiDialogManager MainDialogManager { get; } = new SukiDialogManager();
        [ObservableProperty]
        public ObservableObject? _currentPage;
        public MainWindowViewModel()
        {
            WeakReferenceMessenger.Default.Register<LoginStatusChangedMessage>(this, (_, message) =>
            {
                IsStudentManagePageVisible = message.IsLoggedIn;
            });
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
                
                CurrentPage = new CertificationViewModel(MainDialogManager);

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
            CurrentPage = new CertificationViewModel(MainDialogManager);
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
