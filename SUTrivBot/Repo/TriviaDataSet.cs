using System.Collections.Generic;
using Newtonsoft.Json;
using SUTrivBot.Models;

namespace SUTrivBot.Repo
{
    public class TriviaDataSet
    {
        [JsonProperty("questions", Required = Required.Always)]
        public List<Question> Questions { get; set; }
    }
}