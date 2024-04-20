using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json;

namespace Poe.Models.API;

/**
 * Credits to https://www.datamuse.com/api/
 *
 * https://api.datamuse.com/words
 *
 * 
 * /words endpoint
 * 
 * rel_[code]	Related word constraints: require that the results, when paired with the word in this parameter,
 * are in a predefined lexical relation indicated by [code]. Any number of these parameters may
 * be specified any number of times. An assortment of semantic, phonetic, and corpus-statistics-based relations
 * are available. .
 *
 *      [code]	Description	                                                         Example
 *      jja	Popular nouns modified by the given adjective, per Google Books Ngrams	gradual → increase
        jjb	Popular adjectives used to modify the given noun, per Google Books Ngrams	beach → sandy
        syn	Synonyms (words contained within the same WordNet synset)	ocean → sea
        trg	"Triggers" (words that are statistically associated with the query word in the same piece of text.)	cow → milking
        ant	Antonyms (per WordNet)	late → early
        spc	"Kind of" (direct hypernyms, per WordNet)	gondola → boat
        gen	"More general than" (direct hyponyms, per WordNet)	boat → gondola
        com	"Comprises" (direct holonyms, per WordNet)	car → accelerator
        par	"Part of" (direct meronyms, per WordNet)	trunk → tree
        bga	Frequent followers (w′ such that P(w′|w) ≥ 0.001, per Google Books Ngrams)	wreak → havoc
        bgb	Frequent predecessors (w′ such that P(w|w′) ≥ 0.001, per Google Books Ngrams)	havoc → wreak
        hom	Homophones (sound-alike words)	course → coarse
        cns	Consonant match	sample → simple
        
        md	Metadata flags: A list of single-letter codes (no delimiter) requesting that extra lexical knowledge 
        be included with the results. The available metadata codes are as follows:
        
        Letter	Description	Implementation notes
        
Letter	Description	Implementation notes
d	Definitions	Produced in the defs field of the result object. The definitions are from Wiktionary and WordNet. If the word is an inflected form (such as the plural of a noun or a conjugated form of a verb), then an additional defHeadword field will be added indicating the base form from which the definitions are drawn.
p	Parts of speech	One or more part-of-speech codes will be added to the tags field of the result object. "n" means noun, "v" means verb, "adj" means adjective, "adv" means adverb, and "u" means that the part of speech is none of these or cannot be determined. Multiple entries will be added when the word's part of speech is ambiguous, with the most popular part of speech listed first. This field is derived from an analysis of Google Books Ngrams data.
s	Syllable count	Produced in the numSyllables field of the result object. In certain cases the number of syllables may be ambiguous, in which case the system's best guess is chosen based on the entire query.
r	Pronunciation	Produced in the tags field of the result object, prefixed by "pron:". This is the system's best guess for the pronunciation of the word or phrase. The format of the pronunication is a space-delimited list of Arpabet phoneme codes. If you add "&ipa=1" to your API query, the pronunciation string will instead use the International Phonetic Alphabet. Note that for terms that are very rare or outside of the vocabulary, the pronunciation will be guessed based on the spelling. In certain cases the pronunciation may be ambiguous, in which case the system's best guess is chosen based on the entire query.
f	Word frequency	Produced in the tags field of the result object, prefixed by "f:". The value is the number of times the word (or multi-word phrase) occurs per million words of English text according to Google Books Ngrams.
        
 */
public class DataMuse
{
    private static readonly HttpClient HttpClient = new HttpClient();
    
    /**
     * Collection of relation codes
     */
    private const string Rhyme = "rhy";
    private const string PopularNounsModifiedByAdjective = "jja";
    private const string PopularAdjectivesUsedToModifyNoun = "jjb";
    private const string Synonyms = "syn";
    private const string Triggers = "trg";
    private const string Antonyms = "ant";
    private const string KindOf = "spc";
    private const string MoreGeneralThan = "gen";
    private const string Comprises = "com";
    private const string PartOf = "par";
    private const string FrequentFollowers = "bga";
    private const string FrequentPredecessors = "bgb";
    private const string Homophones = "hom";
    private const string ConsonantMatch = "cns";

    /**
     * Collection of metadata flags
     */
    private const string Definitions = "d";
    private const string PartsOfSpeech = "p";
    private const string SyllableCount = "s";
    private const string Pronunciation = "r";
    private const string WordFrequency = "f";


    async Task<string> GetRelation(string code, string word)
    {
        string url = $"https://api.datamuse.com/words?rel_{code}={word}";
        HttpResponseMessage response = await HttpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        return "";
    }
    
    public async Task<List<string>> GetRhymes(string word)
    {
        string jsonResponse = await GetRelation(Rhyme, word);
        if (!String.IsNullOrEmpty(jsonResponse))
        {
            List<DataMuseWord>? rhymes = JsonConvert.DeserializeObject<List<DataMuseWord>>(jsonResponse);

            // Extract only the Word property from each DataMuseWord object
            List<string> rhymeWords = rhymes.Select(rhyme => rhyme.Word).ToList();

            return rhymeWords;
        }
        return new List<String>(); // Return an empty list if there's an error
    }

    public async Task<List<string>> GetDefinition(string word)
    {
        var jsonResponse = await GetMetaData(Definitions, word);
        
        if (string.IsNullOrEmpty(jsonResponse)) return []; // Return an empty list if there's an error
        
        var definitionList = JsonConvert.DeserializeObject<List<DataMuseWord>>(jsonResponse);

        if (definitionList == null) return []; // Return an empty list if there's an error
            
        // For every DataMuseWord obj in definitionList get its definition
        var definitions = definitionList.SelectMany(w => w.Definitions ?? []).ToList();

        return definitions;
    }
    
    public async Task<List<string>> GetSynonym(string word)
    {
        var jsonResponse = await GetRelation(Synonyms, word);
        
        if (string.IsNullOrEmpty(jsonResponse)) return []; // Return an empty list if there's an error
        
        var wordList = JsonConvert.DeserializeObject<List<DataMuseWord>>(jsonResponse);

        if (wordList == null) return []; // Return an empty list if there's an error
            
        // For every DataMuseWord obj in list get its synonym
        var synonyms = wordList.Select(w => w.Word).ToList();
        
        return synonyms;
    }
    
    public async Task<List<string>> GetAntonyms(string word)
    {
        var jsonResponse = await GetRelation(Antonyms, word);
    
        if (string.IsNullOrEmpty(jsonResponse)) return new List<string>(); // Return an empty list if there's an error
    
        var wordList = JsonConvert.DeserializeObject<List<DataMuseWord>>(jsonResponse);

        if (wordList == null) return new List<string>(); // Return an empty list if there's an error
        
        // For every DataMuseWord obj in list get its word
        var antonyms = wordList.Select(w => w.Word).ToList();

        return antonyms;
    }

    
    
    private async Task<string> GetMetaData(string flag, string word)
    {
        string url = $"https://api.datamuse.com/words?md={flag}&sp={word}";
        HttpResponseMessage response = await HttpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        return "";
    }
    
    /**
     * !Deprecated! From a list of words it finds rhymes between words. Returns a dictionary of a word and the rhymes found.
     */
    /*private async Task<Dictionary<string, List<string>>> GetRhymesFromTextOld(List<string> text)
    { 
      // Use a HashSet to avoid duplicate API calls for the same word.
      var uniqueWords = new HashSet<string>(text);
      
      // Fill dictionary keys with our unique words
      var rhymeDictionary = new Dictionary<string, HashSet<string>>();

      // We really want to loop through the words we pass in, that's all we care about!
      foreach (var word in uniqueWords)
      {
          // Get the list of rhymes for each word
          var rhymes = await GetRhymes(word);
          
          // Cull all entries not in our uniqueWord set
          var filteredRhymes = new HashSet<string>(rhymes.Where(r => uniqueWords.Contains(r)));

          // Search through rhyme list for current word in the unique word list
          foreach (var rhyme in filteredRhymes)
          {
              // Sort the word and its rhyme in alphabetical order to avoid mix ups.
              string key = String.CompareOrdinal(word, rhyme) < 0 ? word : rhyme;
              string value = String.CompareOrdinal(word, rhyme) < 0 ? rhyme : word;
              
              
              if (!rhymeDictionary.ContainsKey(key))
              {
                  rhymeDictionary[key] = new HashSet<string>();
              }

              // Avoid adding the reverse direction of an already existing pair
              if (!rhymeDictionary[key].Contains(value))
              {
                  rhymeDictionary[key].Add(value);
              }
          }
      }
      // Convert to list -- todo this is probably not right
      var commonRhymes = rhymeDictionary.ToDictionary(pair => pair.Key, pair => pair.Value.ToList());
      
      return commonRhymes;
    }*/
    
        
    /*
     * Lookup from first word all its relatives (rhymes). If any words that rhyme exist, add them all to the dictionary
     * then remove them from the list.
     * Repeat this step until you have a full dictionary.
     */
    private async Task<Dictionary<string, List<string>>> GetRhymesFromText(List<string> words)
    {
        // Initialize your rhyme dictionary
        var rhymeDictionary = new Dictionary<string, List<string>>();
        var uniqueWords = new HashSet<string>(words);

        // As long as there are ungrouped words, keep forming groups
        while (uniqueWords.Any())
        {
            var word = uniqueWords.First(); // Take the first word
            var rhymes = await GetRhymes(word); // Get rhymes for that word

            // Filter rhymes to only include those that are in the unique words set
            var rhymingWords = rhymes.Where(r => uniqueWords.Contains(r)).ToList();

            if (rhymingWords.Count > 0)
            {
                // Add the first word and its rhymes to the dictionary
                rhymeDictionary[word] = rhymingWords;

                // Remove the rhyming words from the set of unique words
                rhymingWords.ForEach(rw => uniqueWords.Remove(rw));
            }
            // Also remove the original word whether it had rhymes or not
            uniqueWords.Remove(word);
        }

        return rhymeDictionary;
    }


    
    
    

    /**
     * Assigns rhyme scheme from dictionary of words and their respective rhymes.
     */
    private Dictionary<char, List<string>> AssignRhymeScheme(Dictionary<string, List<string>>? commonRhymeDictionary)
    {
        // Dictionary mapping letters to rhymes ("A" => ["cat", "bat"])
        var rhymeScheme = new Dictionary<char, List<string>>();

        var c = 'A';

        // Add the words and their respective rhymes to a place within the rhymeScheme dictionary
        foreach (var word in commonRhymeDictionary)
        {
            if (!rhymeScheme.ContainsKey(c))
            {
                rhymeScheme[c] = new List<string>();
            }
            
            // Add word to rhyme scheme.
            rhymeScheme[c].Add(word.Key);
        
            // Add each rhyme from that word to rhyme scheme.
            foreach (var rhyme in word.Value)
            {
                rhymeScheme[c].Add(rhyme);

            }
            c++;
        }
        
        return rhymeScheme;
    }

    private void OrderRhymeGroups()
    {
        
    }

    /**
     * Gets the complete rhyme scheme after analyzing and assigning letters to each rhyme.
     */
    public async Task<Dictionary<char, List<string>>> GetRhymeScheme(List<string> text)
    {
        // Todo: Figure out more logic on what to do with the rhyme scheme.
        var commonRhymesDictionary= await GetRhymesFromText(text);
        var rhymeScheme = AssignRhymeScheme(commonRhymesDictionary);
        return rhymeScheme;
    }
    
    

    


}

public class DataMuseWord
{
    [JsonProperty("word")]
    public string Word { get; set; }

    [JsonProperty("score")]
    public int Score { get; set; }

    [JsonProperty("numSyllables")]
    public int? NumSyllables { get; set; }

    [JsonProperty("tags")]
    public List<string> Tags { get; set; }

    [JsonProperty("defs")]
    public List<string> Definitions { get; set; }

    // Additional properties
    [JsonProperty("lc")]
    public string LeftContext { get; set; }

    [JsonProperty("rc")]
    public string RightContext { get; set; }

    [JsonProperty("topics")]
    public List<string> Topics { get; set; }

    /*
     *
     * This are some interesting extra parts of the DataMuse api which could be helpful later!
     *
     * topics   Topic words: An optional hint to the system about the theme of the document being written.
     * Results will be skewed toward these topics. At most 5 words can be specified.
     * Space or comma delimited. Nouns work best.
     * lc	Left context: An optional hint to the system about the word that appears immediately to the left of the
     * target word in a sentence. (At this time, only a single word may be specified.)
     * rc	Right context: An optional hint to the system about the word that appears immediately to the right of the
     * target word in a sentence. (At this time, only a single word may be specified.)
     *
     */
}