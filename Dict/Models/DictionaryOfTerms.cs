using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dict.Models;

public class DictionaryOfTerms
{
    private string _name;
    public DictionaryOfTerms(string name)
    {
        _name = name;
    }
    public string Name => _name;
    
    public Hashtable Hashtable = new(100);

    public void UpdateHashTable(string term, string description)
    {
        if (Hashtable[term] == null)
        {
            Hashtable.Add(term, description);
        }
    }
    public void AddTermLinkPair(string term, string description)
    {
        if (Hashtable[term] == null)
        {
            Hashtable.Add(term, description);
        }
    }
    public string FindTermDescription(string term, string filePath)
    {
        var hash = StringHashCode40(term);
        char endChar = '#';
        var arr = new char[100];
        using (StreamReader reader = new StreamReader(filePath))
        {
            reader.BaseStream.Seek(hash+1, SeekOrigin.Begin); // устанавливаем позицию чтения
            
            int currentChar = 0;
            int i = 0;
            do
            {
                char character = (char)currentChar;
                if(character != '\0')arr[i] = character;
                i++;
        
            } while ((currentChar = reader.Read()) != -1 && currentChar != endChar);
        }

        var s = new string(arr).Trim('\0');
        return s;
    }
    
    public void SaveToBinaryFile(string filePath)
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
    public int StringHashCode40(string value)
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
    
}