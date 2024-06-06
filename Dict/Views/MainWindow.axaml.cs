using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Dict.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;


namespace Dict.Views;

public partial class MainWindow : Window
{
    private List<DictionaryOfTerms> _dictionaries = new();
    private BinaryOperation operation = new();
   
    public MainWindow()
    {
        InitializeComponent();
       var s = operation.ReadWordsFromBinaryFile("Словари.bin");
       string[] news = s[0].Split('#');
       foreach (var VARIABLE in news)
       {
           var newDict = new DictionaryOfTerms(VARIABLE);
           if (!File.Exists($"{VARIABLE}terms.bin") && !File.Exists($"{VARIABLE}description.bin"))
           {
               using (FileStream stream1 = new FileStream($"{VARIABLE}terms.bin", FileMode.Create)) ;
               using (FileStream stream2 = new FileStream($"{VARIABLE}description.bin", FileMode.Create)) ;
           }
           _dictionaries.Add(newDict);
           Console.Write($"{newDict.Name} ");
       }
       
       
        var menuItemClose = new MenuItem { Header = "Закрыть словарь" };
        YourMenu.Items.Add(menuItemClose);
        
        menuItemClose.Click += (_, _) =>
        {
            WordListBox.Items.Clear();
            Termin.Text = "";
            Description.Text = "";
            Console.WriteLine("Словари закрыты");
            YourMenu.Header = "Словари";
        };
        AddDict.Click += (_, _) =>
        {
            var dialog = new AddDictionary();
            dialog.Show();
            Close();
            
        };
        FAQ.Click += (_, _) =>
        {
            var dialog = new FAQ();
            dialog.Show();
        };
        
        foreach (var dictionaryName in _dictionaries)///// добавляем словари
        {
            var menuItem = new MenuItem { Header = dictionaryName.Name};
            //Добавляем обработчик события Click для каждого элемента
            menuItem.Click += (sender, e) =>
            {
                WordListBox.Items.Clear();
                Title = "Словарь терминов";
                // Найти выбранный словарь по имени
                var selectedDictionary = _dictionaries.Find(d => d == dictionaryName);
                // Проверить, что словарь найден
                if (selectedDictionary != null)
                {
                    YourMenu.Header = selectedDictionary.Name;
                    Console.WriteLine($"{selectedDictionary.Name}");
                    UpdateList(dictionaryName);
                    string[] terms = operation.ReadWordsFromBinaryFile($"{YourMenu.Header}terms.bin");
                    string[] descriptions = operation.ReadWordsFromBinaryFileForDescription($"{YourMenu.Header}description.bin", terms);
                    
                }
                
            };
            // Добавляем созданный MenuItem в ваше меню
            YourMenu.Items.Add(menuItem);
        }
        
        WordListBox.SelectionChanged += (_, _) =>
        {
            if (WordListBox.SelectedItem != null)
            {
                Description.Text = "";
                var selectedWord = WordListBox.SelectedItem.ToString();
                Termin.Text = selectedWord;
                foreach (var dictionary in _dictionaries)
                {
                    if (dictionary.Name == (string)YourMenu.Header)
                    {
                        var word = operation.FindTermDescription(selectedWord, $"{dictionary.Name}description.bin");
                        Console.WriteLine(word);
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
        RefreshButton.Click += (_, _) =>
        {
            Termin.Text = "";
            Description.Text = "";
        };
        SearchTermin.Click += (_, _) =>
        {
            var textForSearch = Termin.Text;
            
            if (textForSearch is not null)
            {
                if (YourMenu.Header != "Словари")
                {
                    Description.Text =
                        operation.FindTermDescription(textForSearch, $"{YourMenu.Header}description.bin");
                }
            }
        };
        SaveButton.Click += (_, _) =>
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Сохрание", "Сохранение прошло успешно");
            box.ShowAsync();
        };
        AddTerminButton.Click += (_, _) =>
        {
            var term = Termin.Text;
            var description = Description.Text;
            string name;

            if (YourMenu.Header != null && YourMenu.Name != "Словари")
            {
                foreach (var dictionary in _dictionaries)
                {
                    if (dictionary.Name == (string)YourMenu.Header)
                    {
                        if (term != null && description != null)
                        {
                            name = dictionary.Name;
                            Console.WriteLine(name);
                            dictionary.UpdateHashTable(term, description);
                            operation.SaveDescriptionToBinaryFile($"{name}description.bin", description+'#', dictionary.StringHashCode40(term));
                            operation.SaveToBinaryFile($"{name}terms.bin", dictionary.Hashtable);
                            WordListBox.Items.Clear();
                            UpdateList(dictionary);
                        }
                        else
                        {
                            Console.WriteLine("ПОля пустые");
                            return;
                        }
                    }
                }
                
            }
        };
        DeleteButton.Click += (_, _) =>
        {
            var term = Termin.Text;
            foreach (var dictionary in _dictionaries)
            {
                if (dictionary.Name == (string)YourMenu.Header)
                {
                    operation.DeleteDescription($"{dictionary.Name}description.bin", dictionary.StringHashCode40(term));
                    var termlist = operation.ReadWordsFromBinaryFile($"{dictionary.Name}description.bin");
                    string[] news = termlist[0].Split('#');
                    dictionary.Hashtable.Remove(term);
                    operation.SaveToBinaryFile($"{dictionary.Name}terms.bin", dictionary.Hashtable);
                    WordListBox.Items.Clear();
                    Termin.Text = "";
                    Description.Text = "";
                    UpdateList(dictionary);
                }
            }

           
        };
    }
    private void UpdateList(DictionaryOfTerms dictionaryName)
    {
        var selectedDictionary = _dictionaries.Find(d => d == dictionaryName);
        string[] terms = operation.ReadWordsFromBinaryFile($"{dictionaryName.Name}terms.bin");
        foreach (var name in terms)
        {
            if(Regex.IsMatch(name, "[а-яА-Я0-9]"))
                dictionaryName.UpdateHashTable(name, name+1);
        }
        //Получить все слова из выбранного словаря
        var words = selectedDictionary.Hashtable.Keys;
        // Добавить все слова в ListBox
        foreach (var word in words)
        {
            if (selectedDictionary.Hashtable[word]?.ToString() != "System.Collections.Hashtable" && selectedDictionary.Hashtable[word]?.ToString() != null )
            {
                WordListBox.Items.Add(word);
            }
        }
    }
}