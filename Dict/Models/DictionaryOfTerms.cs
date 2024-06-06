using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dict.Models;

public class DictionaryOfTerms(string name)
{
    public string Name => name;
    
    public Hashtable Hashtable = new(100);

    public void UpdateHashTable(string term, string description)
    {
        if (Hashtable[term] == null)
        {
            Hashtable.Add(term, description);
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
    
}