using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Poe.Models;

// In here we will have all of our code to analyze words

// I really want an idea where we are only allowed to use a certain lexicon (maybe a random distribution)
// But an idea where u can only use the most common 1000 words would be really fun!

// Let's highlight any word outside of our lexicon in bright yellow

public class Words
{
    private HashSet<string>? _lexicon; //todo don't know if ? should go here

    public Words(string lexiconFilePath)
    {
        LoadLexicon(lexiconFilePath); // Initialize lexicon from file at instantiation
    }

    private void LoadLexicon(string filePath)
    {
        // Ensure file exists to prevent exceptions
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Lexicon file not found at {filePath}");

        _lexicon = new HashSet<string>(File.ReadAllLines(filePath), StringComparer.OrdinalIgnoreCase);
    }

    // Analyze text and return positions and lengths of words not in lexicon
    public IEnumerable<(string Word, int StartIndex, int Length)> AnalyzeText(string inputText)
    {
        var matchesNotInLexicon = new List<(string, int, int)>();
        // Using regex to find word positions and lengths
        var matches = Regex.Matches(inputText, @"\w+");

        foreach (Match match in matches)
        {
            string word = match.Value;
            if (!_lexicon.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                matchesNotInLexicon.Add((word, match.Index, word.Length));
            }
        }

        return matchesNotInLexicon;
    }
}