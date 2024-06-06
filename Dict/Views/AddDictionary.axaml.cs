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

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (textDict.Text is not null)
        {
            WriteWordsToBinaryFile(textDict.Text+'#',"Словари.bin");
        }
    }
    static void WriteWordsToBinaryFile(string words, string fileName)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
        {
            byte[] wordBytes = Encoding.UTF8.GetBytes(words); // Преобразование слова в байты с использованием UTF-8
            writer.Write(wordBytes); // Запись самого слова в бинарном формате
            
        }
    }
}