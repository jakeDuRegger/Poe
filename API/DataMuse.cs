using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using WeCantSpell.Hunspell;

namespace Poe.API;

public class DataMuse
{
    private static readonly HttpClient HttpClient = new HttpClient();

    public async Task<List<string>> GetRhymes(string word)
    {
        string url = $"https://api.datamuse.com/words?rel_rhy={word}";
        HttpResponseMessage response = await HttpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            // Probably should add try / catch block here later.
            List<DataMuseWord> rhymes = JsonConvert.DeserializeObject<List<DataMuseWord>>(jsonResponse);
            
            // Extract only the Word property from each DataMuseWord object
            List<string> rhymeWords = rhymes.Select(rhyme => rhyme.Word).ToList();
            
            return rhymeWords;
        }
        else
        {
            return new List<String>(); // Return an empty list if there's an error
        }
    }
    
    
    
    
    // Credit to https://www.datamuse.com/api/
    // todo Add ability to search for rhymes from a particular word!
    //todo Look into word suggestions (jja and jjb on DataMuse api
    
    
    // Syllable Counter!!!!
    // s	Syllable count
    // Produced in the numSyllables field of the result object. In certain cases the number of syllables may be ambiguous,
    // in which case the system's best guess is chosen based on the entire query.
    
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
