using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Dict.Models;

namespace Dict.Views;

public partial class AddDictionary : Window
{
    public AddDictionary()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs routedEventArgs)
    {
        var operation = new BinaryOperation();
        if (textDict.Text is not null)
        {
            operation.WriteWordsToBinaryFile(textDict.Text+'#',"Словари.bin");
        }
        var wn = new MainWindow();
        wn.Show();
        Close();
    }
}