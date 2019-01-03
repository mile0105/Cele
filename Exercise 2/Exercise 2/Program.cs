using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Exercise_2
{
    class Program
    {
        private static string rootFilePath = "C:\\Users\\Mile\\Desktop\\Cele\\Exercise 2\\"; //smeni go ova.
        private static List<string> files = new List<string>
        {
            "t1.txt",
            "t2.txt",
            "t3.txt"
        };// klaj gi text files vo ova

        private static Dictionary<string, HashSet<string>> index = new Dictionary<string, HashSet<string>>();
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter your search query: ");
            string query = Console.ReadLine();
            CreateIndex();
            Search(query);
            Console.ReadKey();
        }

        private static void CreateIndex()
        {
            foreach (var file in files)
            {
                try
                {
                    string filePath = rootFilePath + file;
                    string[] lines = File.ReadAllLines(filePath);
                    List<string> words = ExtractWords(lines);
                    foreach (string word in words)
                    {
                        string lowerCase = word.ToLowerInvariant();
                        if (!index.ContainsKey(lowerCase))
                        {
                            index.Add(lowerCase, new HashSet<string>());
                        }
                        index[lowerCase].Add(file);
                    }
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("File " + file + " not found.");
                }
            }
            Console.WriteLine("Index created with: " + index.Count + " items.");
        }
        private static void Search(string query)
        {
            string[] words = Regex.Split(query, @"\W+");
            HashSet<string> result = new HashSet<string>();
            if (!index.ContainsKey(words[0].ToLowerInvariant()))
            {
                Console.WriteLine("Not found!");
                return;
            }
            result = index[words[0].ToLowerInvariant()];
            foreach (var word in words)
            {
                if (!index.ContainsKey(word.ToLowerInvariant()))
                {
                    Console.WriteLine("Not found!");
                    return;
                }
                result.IntersectWith(index[word.ToLowerInvariant()]);
            }
            if (result.Count == 0)
            {
                Console.WriteLine("Not found!");
            }
            else
            {
                Console.WriteLine("Found in files: " + String.Join(", ", result));
            }            
        }
        private static List<string> ExtractWords(string[] lines)
        {
            List<string> words = new List<string>();
            foreach (var line in lines)
            {
                //splits the words
                words.AddRange(Regex.Split(line, @"\W+"));
            }
            return words;
        }
            
    }
}
