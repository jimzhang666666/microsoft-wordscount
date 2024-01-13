using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

class Program
{
    static void Main()
    {
        string url = "https://en.wikipedia.org/wiki/Microsoft";
        string sectionId = "History";
        int numberOfWordsToReturn = 10;
      
        // Add words to exclude
        var excludedWords = new HashSet<string> { "this", "a", "is", "to", "on", "at" };
        var result = GetMostCommonWords(url, sectionId, numberOfWordsToReturn, excludedWords);

        Console.WriteLine("# of occurrences");
        foreach (var entry in result)
        {
            Console.WriteLine($"{entry.Key,-15}{entry.Value}");
        }
    }

    static Dictionary<string, int> GetMostCommonWords(string url, string sectionId, int topN, HashSet<string> excludedWords)
    {
        var web = new HtmlWeb();
        var doc = web.Load(url);

        var sectionNode = doc.DocumentNode.SelectSingleNode($"//span[@id='{sectionId}']");
        var text = sectionNode?.InnerText ?? string.Empty;

        var words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(word => word.ToLower())
                        .Where(word => !excludedWords.Contains(word))
                        .GroupBy(word => word)
                        .ToDictionary(group => group.Key, group => group.Count());

        return words.OrderByDescending(pair => pair.Value)
                    .Take(topN)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}
