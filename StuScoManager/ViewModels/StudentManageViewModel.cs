using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StuScoManager.Models;
using SukiUI.Toasts;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
namespace StuScoManager.ViewModels
{
    public partial class StudentManageViewModel : ViewModelBase
    {
        public static ISukiToastManager ToastManager { get; } = new SukiToastManager();
        [ObservableProperty]
        public string _newStudentName;
        [ObservableProperty]
        public string _newStudentScore;
        public ObservableCollection<Student> Students { get; } = new();
        public StudentManageViewModel()
        {
            LoadFromFile();
        }
        [RelayCommand]
        public void AddStudent()
        {
            int integerOfNewStudentScore = 0;
            if ((int.TryParse(NewStudentScore, out integerOfNewStudentScore)) && NewStudentName != "")
            {
                Students.Add(new Student { Name = NewStudentName, Score = integerOfNewStudentScore });
                SaveToFile();
            } 
            else if(NewStudentName == "")
            {
                DisplayWarningToast("警告！", "学生姓名不得为空");
            }
            else
            {
                DisplayWarningToast("警告！", $"请勿在分数栏填写非数字字符“{NewStudentScore}”，请重新填写");
            }
        }
        public static void DisplayWarningToast(string title = "Message", string content = "Example content.")
        {
            ToastManager.CreateToast()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Warning)
                .WithTitle(title)
                .WithContent(content)
                .Dismiss().After(TimeSpan.FromSeconds(3))
                .Dismiss().ByClicking()
                .Queue();
        }
        private readonly string _saveFilePath = "students.json";

        [RelayCommand]
        private void DeleteStudent(Student student)
        {
            if (student != null && Students.Contains(student))
            {
                Students.Remove(student);
                SaveToFile();
            }
        }

        // 切换编辑模式
        [RelayCommand]
        private void ToggleEditMode(Student student)
        {
            if (student != null)
                student.IsEditing = !student.IsEditing;
        }

        // 保存当前正在编辑的学生（退出编辑模式）
        [RelayCommand]
        private void SaveEdit(Student student)
        {
            if (student != null && student.IsEditing)
            {
                student.IsEditing = false;
                SaveToFile();
            }
        }

        // 保存到 JSON 文件
        private void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(Students, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_saveFilePath, json);
            }
            catch (Exception ex)
            {
                // 实际项目可用日志记录
                System.Diagnostics.Debug.WriteLine($"保存失败: {ex.Message}");
            }
        }
        // 从 JSON 文件加载
        private void LoadFromFile()
        {
            if (!File.Exists(_saveFilePath)) return;

            try
            {
                var json = File.ReadAllText(_saveFilePath);
                var list = JsonSerializer.Deserialize<ObservableCollection<Student>>(json);
                if (list != null)
                {
                    Students.Clear();
                    foreach (var student in list)
                        Students.Add(student);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载失败: {ex.Message}");
            }
        }
    }
}