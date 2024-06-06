using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dict.Models;

public class BinaryOperation
{
    public string FindTermDescription(string term, string filePath)
    {
        var hash = StringHashCode40(term);
        char endChar = '#';
        var arr = new char[100];
        using (StreamReader reader = new StreamReader(filePath))
        {
            reader.BaseStream.Seek(hash+1, SeekOrigin.Begin); // устанавливаем позицию чтения

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
        return s;
    }
    public void SaveToBinaryFile(string filePath, Hashtable Hashtable)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            foreach (DictionaryEntry obj in Hashtable)
            {
                if (obj.Value.ToString() != "System.Collections.Hashtable")
                {
                    char[] s = (obj.Key.ToString()+' ').ToCharArray();
                    writer.Write(s);
                }
            }
        }
    }
    public void SaveDescriptionToBinaryFile(string filePath, string description, int seek)
    {
        using (var writter = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate)))
        {
            writter.Seek(seek, SeekOrigin.Begin);
            writter.Write(description);
        }
    }
  
    public int StringHashCode40(string input)
    {
        int hash = 0;
        foreach (char c in input)
        {
            hash = (hash * 31 + c) % 10000;
        }
        return hash;
    }
    public void WriteWordsToBinaryFile(string words, string fileName)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Append)))
        {
            byte[] wordBytes = Encoding.UTF8.GetBytes(words); // Преобразование слова в байты с использованием UTF-8
            writer.Write(wordBytes); // Запись самого слова в бинарном формате
            
        }
    }
    public string[] ReadWordsFromBinaryFile(string filePath)
    {
        string[] words;
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                long length = fileStream.Length;
                byte[] buffer = new byte[length];
                binaryReader.Read(buffer, 0, buffer.Length);

                string data = Encoding.UTF8.GetString(buffer);
                words = data.Split(' ');
            }
        }

        return words;
    }
    public string[] ReadWordsFromBinaryFileForDescription(string filePath, string[] terms)
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

    public void DeleteDescription(string filePath, int seek)
    {
        byte[] data;
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);

            int i = seek+1;
            while (i <= data.Length && data[i] != (byte)'#')
            {
                data[i] = (byte)'\0'; // Заменяем символы на '\0'
                if (data[i] == (byte)'#')
                {
                    data[i] = (byte)'\0';
                    break;
                }
                i++;
                
            }

            // Перемещаем указатель в начало файла и записываем измененные данные
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Write(data, 0, data.Length);
        }
        
    }
}