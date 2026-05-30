
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.IO;

namespace StuScoManager.Models
{
    public class StudentManage
    {
        List<Student> students = new();
        string[] studentFilePaths = Directory.GetFiles("./students");
        public StudentManage()
        {
            foreach (string path in studentFilePaths)
            {
                students.Add(ReadStudent(path));
            }
        }
        public static Student NewStudent(string stuName)
        {
            Student student = new();
            student._name = stuName;
            File.WriteAllLines($"./students/student_{stuName}.txt", [student._name, student._score.ToString()]);
            return student;
        }
        public static Student ReadStudent(string stuName)
        {
            List<string> stu = [..File.ReadAllLines($"./students/student_{stuName}.txt")];
            Student student = new()
            {
                _name = stuName,
                _score = int.Parse(stu[1])
            };
            return student;
        }
    }
    public partial class Student : ObservableObject
    {
        [ObservableProperty]
        public int _score = 0;
        [ObservableProperty]
        public string _name = string.Empty;
        [ObservableProperty]
        public bool _isEditing = false;

    }
}