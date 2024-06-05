using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Avalonia.Controls;
using Dict.Models;
namespace Dict.Views;

public partial class MainWindow : Window
{
    private List<DictionaryOfTerms> _dictionaries = new();
    public MainWindow()
    {
        
        InitializeComponent();
        DictionaryOfTerms dict = new DictionaryOfTerms("Фрукты");
        dict.AddTermLinkPair("яблоко", "яблоко1");
        dict.AddTermLinkPair("банан", "банан2");
        dict.AddTermLinkPair("апельсин", "апельсин3");
        dict.AddTermLinkPair("лимон", "лимон4");
        dict.SaveDescriptionToBinaryFile("description.bin", "яблоко1#", dict.StringHashCode40("яблоко"));
        dict.SaveDescriptionToBinaryFile("description.bin", "банан2#", dict.StringHashCode40("банан"));
        dict.SaveDescriptionToBinaryFile("description.bin", "апельсин2#", dict.StringHashCode40("апельсин"));
        dict.SaveDescriptionToBinaryFile("description.bin", "лимон2#", dict.StringHashCode40("лимон"));
        dict.SaveToBinaryFile("terms.bin");
        _dictionaries.Add(dict);
        Console.WriteLine();
        var menuItemClose = new MenuItem { Header = "Закрыть словари" };
        YourMenu.Items.Add(menuItemClose);
        menuItemClose.Click += (sender, e) =>
        {
            WordListBox.Items.Clear();
            Termin.Text = "";
            Description.Text = "";
            Console.WriteLine("Словари закрыты");
        };
        FAQ.Click += (sender, e) =>
        {
            var dialog = new FAQ();
            dialog.Show();
        };
        foreach (var dictionaryName in _dictionaries)
        {
            var menuItem = new MenuItem { Header = dictionaryName.Name};
            //Добавляем обработчик события Click для каждого элемента
            menuItem.Click += (sender, e) =>
            {
                WordListBox.Items.Clear();
                Title = "Словарь терминов";
                // Найти выбранный словарь по имени
                DictionaryOfTerms selectedDictionary = _dictionaries.Find(d => d == dictionaryName);
                // Проверить, что словарь найден
                if (selectedDictionary != null)
                {
                    Title += $" {selectedDictionary.Name}";
                    Console.WriteLine($"{selectedDictionary.Name}");
                    UpdateList(dictionaryName);
                }
            };
            // Добавляем созданный MenuItem в ваше меню
            YourMenu.Items.Add(menuItem);
        }
        WordListBox.SelectionChanged += (sender, e) =>
        {
            if (WordListBox.SelectedItem != null)
            {
                Description.Text = "";
                var selectedWord = WordListBox.SelectedItem.ToString();
                Termin.Text = selectedWord;
                foreach (var dictionary in _dictionaries)
                {
                    var word = dictionary.FindTermDescription(selectedWord, "description.bin");
                    Console.WriteLine(word);
                    if (word != null)
                    {
                        Description.Text += word;
                        break; // Выходим из цикла, если нашли описание слова
                    }
                }
            }
            else
            {
                Termin.Text = "";
                Description.Text = "";
            }
        };
        AddTerminButton.Click += (sender, e) =>
        {
            var term = Termin.Text;
            var description = Description.Text;
            dict.AddTermLinkPair(term, description);
            dict.SaveDescriptionToBinaryFile("description.bin", description+'#', dict.StringHashCode40(term));
            dict.SaveToBinaryFile("terms.bin");
            WordListBox.Items.Clear();
            UpdateList(dict);
        };
        
        
        
    }
    private void UpdateList(DictionaryOfTerms dictionaryName)
    {
        var selectedDictionary = _dictionaries.Find(d => d == dictionaryName);
        //Получить все слова из выбранного словаря
        var words = selectedDictionary.Hashtable.Keys;
        foreach (var word in words)
        {
            if(selectedDictionary.Hashtable[word].ToString() != "System.Collections.Hashtable") Console.Write($"{word} ");
        }
        // Добавить все слова в ListBox
        foreach (var word in words)
        {
            if(selectedDictionary.Hashtable[word].ToString() != "System.Collections.Hashtable") WordListBox.Items.Add(word);
        }
    }
}