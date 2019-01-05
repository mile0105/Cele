using System;
using System.Collections.Generic;
using System.Data.Common;
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

        private static Dictionary<string, HashSet<PositionsInFile>> index = new Dictionary<string, HashSet<PositionsInFile>>();
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
                    int position = 1;
                    foreach (string word in words)
                    {
                        string lowerCase = word.ToLowerInvariant();
                        if (!index.ContainsKey(lowerCase))
                        {
                            index.Add(lowerCase, new HashSet<PositionsInFile>());
                        }
                        HashSet<PositionsInFile> positions = index[lowerCase];
                        if (!positions.Any(m => m.FileName.Equals(file)))
                        {
                            index[lowerCase].Add(
                                new PositionsInFile
                                {
                                    FileName = file,
                                    Positions = new HashSet<int>(new [] {position})
                                });
                        }
                        else
                        {
                            try
                            {
                                index[lowerCase].First(m => m.FileName.Equals(file)).Positions.Add(position);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        position++;
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
            HashSet<PositionsInFile> result = new HashSet<PositionsInFile>();
            if (!index.ContainsKey(words[0].ToLowerInvariant()))
            {
                Console.WriteLine("Not found!");
                return;
            }
            result = index[words[0].ToLowerInvariant()];
            for (int i = 1; i < words.Length; i++)
            {
                string previousWord = words[i - 1].ToLowerInvariant();
                string currentWord = words[i].ToLowerInvariant();
                if (!index.ContainsKey(currentWord))
                {
                    Console.WriteLine("Not found!");
                    return;
                }
                HashSet<PositionsInFile> filesAndPositionsInCurrentWord = new HashSet<PositionsInFile>(index[currentWord]);
                HashSet<PositionsInFile> filesAndPositionsInPreviousWord = new HashSet<PositionsInFile>(index[previousWord]);
                var files = filesAndPositionsInCurrentWord.Select(m => m.FileName);
                result.RemoveWhere(match => !files.Contains(match.FileName));
                foreach (var res in filesAndPositionsInCurrentWord)
                {
                    HashSet<int> currentPositions = res.Positions;
                    var file = res.FileName;
                    foreach (var prev in filesAndPositionsInPreviousWord)
                    {
                        if (prev.FileName.Equals(file))
                        {
                            HashSet<int> previousPositions = prev.Positions;
                            previousPositions.RemoveWhere(match => !currentPositions.Contains(match + 1));
                            currentPositions.RemoveWhere(match => !previousPositions.Contains(match - 1));
                        }
                    }
                }
                result.RemoveWhere(match => !match.Positions.Any());
            }
            if (result.Count == 0)
            {
                Console.WriteLine("Not found!");
            }
            else
            {
                foreach (var pos in result)
                {
                    Console.WriteLine("Found in file: " + pos.FileName + " in positions: " + string.Join(", ", pos.Positions));
                }
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
