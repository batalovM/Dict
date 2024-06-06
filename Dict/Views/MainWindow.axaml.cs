using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalonia.Controls;
using Dict.Models;


namespace Dict.Views;

public partial class MainWindow : Window
{
    private List<DictionaryOfTerms> _dictionaries = new();
    static void WriteWordsToBinaryFile(string[] words, string fileName)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
        {
            foreach (string word in words)
            {
                byte[] wordBytes = Encoding.UTF8.GetBytes(word); // Преобразование слова в байты с использованием UTF-8
                writer.Write(wordBytes); // Запись самого слова в бинарном формате
            }
        }
    }
    static string[] ReadWordsFromBinaryFile(string filePath)
    {
        string[] words;
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                long length = fileStream.Length;
                byte[] buffer = new byte[length];
                binaryReader.Read(buffer, 0, buffer.Length);

                string data = System.Text.Encoding.UTF8.GetString(buffer);
               // words = data.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
               words = data.Split(' ');
            }
        }

        return words;
    }
    static int StringHashCode40(string value)
    {
        int num = 5381;
        int num2 = num;
        for (int i = 0; i < value.Length; i += 2)
        {
            num = (((num << 5) + num) ^ value[i]);

            if (i + 1 < value.Length)
                num2 = (((num2 << 5) + num2) ^ value[i + 1]);
        }
        return Math.Abs(num + num2 * 1566083941);
    }
    static string[] ReadWordsFromBinaryFileForDescription(string filePath, string[] terms)
    {
        List<string> listOfDescription = new ();
        foreach (var term in terms)
        {
            var hash = StringHashCode40(term);
            char endChar = '#';
            var arr = new char[100];
            using (StreamReader reader = new StreamReader(filePath))
            {
                reader.BaseStream.Seek(hash, SeekOrigin.Begin); // устанавливаем позицию чтения

                int i = 0;
                int currentChar;
                do
                {
                    currentChar = reader.Read();
                    if (currentChar != -1 && currentChar != endChar)
                    {
                        char character = (char)currentChar;
                        arr[i] = character;
                        i++;
                    }
                } while (currentChar != -1 && currentChar != endChar && i < arr.Length);
            }
            var s = new string(arr).Trim('\0');
            listOfDescription.Add(s);
        }

        return listOfDescription.ToArray();
    }
    public MainWindow()
    {
        InitializeComponent();
        DictionaryOfTerms dict = new DictionaryOfTerms("Фрукты");
        string[] names = { dict.Name+'#' };
        WriteWordsToBinaryFile(names, "Словари.bin");
        
        search.Click += (sender, e) =>
        {
            var textForSearch = Termin.Text;
            
            if (textForSearch is not null)
            {
                if (YourMenu.Header != "Словари")
                {
                    Description.Text = dict.FindTermDescription(textForSearch, $"{YourMenu.Header}description.bin");
                }
            }
        };
        // dict.AddTermLinkPair("яблоко", "яблоко1");
        // dict.AddTermLinkPair("банан", "банан2");
        // dict.AddTermLinkPair("апельсин", "апельсин3");
        // dict.AddTermLinkPair("лимон", "лимон4");
        // dict.SaveDescriptionToBinaryFile($"{dict.Name}description.bin", "яблоко1#", dict.StringHashCode40("яблоко"));
        // dict.SaveDescriptionToBinaryFile($"{dict.Name}description.bin", "банан2#", dict.StringHashCode40("банан"));
        // dict.SaveDescriptionToBinaryFile($"{dict.Name}description.bin", "апельсин2#", dict.StringHashCode40("апельсин"));
        // dict.SaveDescriptionToBinaryFile($"{dict.Name}description.bin", "лимон2#", dict.StringHashCode40("лимон"));
        // dict.SaveToBinaryFile($"{dict.Name}terms.bin");
        // _dictionaries.Add(dict);
        //////////////////
        // foreach (var VARIABLE in terms)
        // {
        //     Console.Write($"{VARIABLE} ");
        // }
        // foreach (var VARIABLE in descriptions)
        // {
        //     Console.Write($"{VARIABLE} " );
        // }
        _dictionaries.Add(dict);
        var menuItemClose = new MenuItem { Header = "Закрыть словарь" };
        YourMenu.Items.Add(menuItemClose);
        
        menuItemClose.Click += (sender, e) =>
        {
            WordListBox.Items.Clear();
            Termin.Text = "";
            Description.Text = "";
            Console.WriteLine("Словари закрыты");
            YourMenu.Header = "Словари";
        };
        AddDict.Click += (sender, args) =>
        {
            var dialog = new AddDictionary();
            dialog.Show();
        };
        FAQ.Click += (sender, e) =>
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
                DictionaryOfTerms selectedDictionary = _dictionaries.Find(d => d == dictionaryName);
                // Проверить, что словарь найден
                if (selectedDictionary != null)
                {
                    YourMenu.Header = selectedDictionary.Name;
                    Console.WriteLine($"{selectedDictionary.Name}");
                    UpdateList(dictionaryName);
                    string[] terms = ReadWordsFromBinaryFile("Фруктыterms.bin");
                    string[] descriptions = ReadWordsFromBinaryFileForDescription("Фруктыdescription.bin", terms);
                    
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
                    var word = dictionary.FindTermDescription(selectedWord, $"{dictionary.Name}description.bin");
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
            string name;

            if (YourMenu.Header != null && YourMenu.Name != "Словари")
            {
                foreach (var dictionary in _dictionaries)
                {
                    if (term != null && (description != null) && (dictionary.Name == (string)YourMenu.Header))
                    {
                        
                        
                        name = dictionary.Name;// string[] descriptions = ReadWordsFromBinaryFileForDescription("Фруктыdescription.bin", terms);
                        dict.AddTermLinkPair(term, description);
                        dict.SaveDescriptionToBinaryFile($"{name}description.bin", description+'#', dict.StringHashCode40(term));
                        dict.SaveToBinaryFile($"{name}terms.bin");
                    }
                    else
                    {
                        Console.WriteLine("ПОля пустые");
                        return;
                    }
                }
                WordListBox.Items.Clear();
                UpdateList(dict);
            }
        };
    }
    private void UpdateList(DictionaryOfTerms dictionaryName)
    {
        
        var selectedDictionary = _dictionaries.Find(d => d == dictionaryName);
        string[] terms = ReadWordsFromBinaryFile($"{dictionaryName.Name}terms.bin");
        foreach (var name in terms)
        {
            dictionaryName.UpdateHashTable(name, name+1);
        }
        //Получить все слова из выбранного словаря
        var words = selectedDictionary.Hashtable.Keys;
        // Добавить все слова в ListBox
        foreach (var word in words)
        {
            if (selectedDictionary.Hashtable[word].ToString() != "System.Collections.Hashtable" && selectedDictionary.Hashtable[word].ToString() != null )
            {
                WordListBox.Items.Add(word);
            }
        }
    }
}