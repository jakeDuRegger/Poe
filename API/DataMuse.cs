using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Poe.API;

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