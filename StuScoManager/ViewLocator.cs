using Avalonia.Controls;
using Avalonia.Controls.Templates;
using StuScoManager.ViewModels;
using System;

namespace StuScoManager;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null) return null;

        // 避免处理字符串等非 ViewModel 对象
        if (param is not ViewModelBase)
            return new TextBlock { Text = $"Invalid DataContext type: {param.GetType()}" };

        var typeName = param.GetType().FullName!
            .Replace("ViewModels", "Views")
            .Replace("ViewModel", "View");

        var type = Type.GetType(typeName);

        if (type is not null)
        {
            try
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            catch (Exception ex)
            {
                return new TextBlock { Text = $"View creation failed: {ex.Message}" };
            }
        }

        return new TextBlock { Text = $"View not found: {typeName}" };
    }

    public bool Match(object? data) => data is ViewModelBase;
}