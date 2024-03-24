using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Poe.API
{
    public class MerriamWebsterAPI
    {
        // Constants for the API keys
        private const string DictionaryApiKey = "25673efd-c1ff-4f0d-a791-81176916ff38";
        private const string ThesaurusApiKey = "eab0d19a-c42e-4c9b-99d9-7017258a59e2";

        // HttpClient instance for making web requests
        private readonly HttpClient _httpClient = new HttpClient();

        // Method to get dictionary definitions
        public async Task<string> GetDictionaryDefinition(string word)
        {
            try
            {
                string url =
                    $"https://www.dictionaryapi.com/api/v3/references/collegiate/json/{word}?key={DictionaryApiKey}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                // Parse JSON and extract detailed information
                JArray jsonArray = JArray.Parse(json);
                JObject firstEntry = (JObject)jsonArray[0];

                // Extract the word and its pronunciation
                string formattedWord = firstEntry["hwi"]["hw"].ToString().Replace("*", "·");
                string pronunciation = firstEntry["hwi"]["prs"][0]["mw"].ToString();

                // Extract the part of speech
                string partOfSpeech = firstEntry["fl"].ToString();

                // Extract definitions
                StringBuilder definitions = new StringBuilder();
                foreach (JArray sseq in firstEntry["def"][0]["sseq"])
                {
                    foreach (JArray senseArray in sseq)
                    {
                        string def = senseArray[1]["dt"][0][1].ToString();
                        definitions.AppendLine(def);
                    }
                }
                
                // Format the final output
                string formattedDefinition =
                    $"{formattedWord}\n/{pronunciation}/\n{partOfSpeech}\n{definitions}\n";

                return formattedDefinition;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return "Definition not found.";
            }
        }


        // Method to get thesaurus synonyms
        public async Task<(string Synonyms, string Antonyms)> GetThesaurus(string word)
        {
            try
            {
                string url = $"https://www.dictionaryapi.com/api/v3/references/thesaurus/json/{word}?key={ThesaurusApiKey}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                // Deserialize JSON to dynamic object
                dynamic data = JsonConvert.DeserializeObject(json);
                
                // Initialize StringBuilder for synonyms and antonyms
                StringBuilder synonyms = new StringBuilder();
                StringBuilder antonyms = new StringBuilder();

                // Extract synonyms
                if (data[0]["meta"]["syns"] != null)
                {
                    foreach (var synGroup in data[0]["meta"]["syns"])
                    {
                        foreach (string synonym in synGroup)
                        {
                            synonyms.AppendLine(synonym);
                        }
                    }
                }

                // Extract antonyms
                if (data[0]["meta"]["ants"] != null)
                {
                    foreach (var antGroup in data[0]["meta"]["ants"])
                    {
                        foreach (string antonym in antGroup)
                        {
                            antonyms.AppendLine(antonym);
                        }
                    }
                }

                return (synonyms.ToString(), antonyms.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return ("Synonyms not found.", "Antonyms not found.");
            }
        }

    }
}