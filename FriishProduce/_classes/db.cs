﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace FriishProduce
{
    public class Database
    {
        public string CurrentFolder(Platforms platform) => Paths.Database + platform.ToString().ToLower() + "\\";

        private static JToken dbReader;

        internal int Selected;

        public Database(int platform = -1)
        {
            dbReader = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(Paths.Database + "database.json"));
            if (platform >= 0)
            {
                dbReader = dbReader[$"{((Platforms)platform).ToString().ToLower()}"];
                Selected = platform;
            }
        }

        public JEnumerable<JToken> GetList()
        {
            try
            {
                return dbReader.Children<JToken>();
            }
            catch (System.NullReferenceException)
            {
                dbReader = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(Paths.Database + "database.json"));
                dbReader = dbReader[Platforms.Flash.ToString().ToLower()];
                Selected = (int)Platforms.Flash;
                return dbReader.Children<JToken>();
            }
        }

        /// <summary>
        /// Searches for an entry with the title parameter (game name & region) and if found, returns its upper ID, otherwise null
        /// </summary>
        public string SearchID(string title)
        {
            foreach (JObject entry in dbReader.Children<JToken>())
                if (entry["title"].ToString() == title)
                    return entry["id"].ToString().ToUpper();

            return null;
        }
    }
}
