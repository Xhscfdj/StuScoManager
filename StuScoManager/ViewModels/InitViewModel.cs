using Avalonia.DesignerSupport;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StuScoManager.Models;
using System.Diagnostics;

namespace StuScoManager.ViewModels
{
    public partial class InitViewModel : ViewModelBase
    {
        [ObservableProperty]
        public string _password = string.Empty;
        [ObservableProperty]
        public string _pwd = string.Empty;
        public InitViewModel()
        {

        }
        [RelayCommand]
        public void SetPwd()
        {
            Debug.WriteLine("button on click");
            if ((Password == Pwd) && (Password.Length == 12))
            {
                Debug.WriteLine("Password validated.");
                Debug.WriteLine("before:");
                Debug.WriteLine(Configs.pwd);
                string encryptedPassword = AesGcmPasswordProtector.Encrypt(Password, Configs.key);

                Configs.pwd = encryptedPassword;
                Configs.inited = "true";
                Configs.WriteConfigs();

                Debug.WriteLine("After:");
                Debug.WriteLine(Configs.pwd);
                Debug.WriteLine("Decrypted:");
                Debug.WriteLine(AesGcmPasswordProtector.Decrypt(Configs.pwd, Configs.key));
            }
        }
        
    }
}