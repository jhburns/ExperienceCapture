namespace Carter.App.Export.JsonExtra
{
    using System.Collections.Generic;
    using System.Data;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Helpers for doing JSON conversions.
    /// </summary>
    public class JsonExtra
    {
        // https://stackoverflow.com/a/36348017
        /// <summary>
        /// Converts JSON to a DataTable.
        /// </summary>
        /// <returns>
        /// A data table.
        /// </returns>
        /// <param name="jsonContent">Data in JSON form to be converted.</param>
        public static DataTable JsonToTable(string jsonContent)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonContent);
        }

        // https://stackoverflow.com/a/32800161
        /// <summary>
        /// Converts JSON into a flat, dictionary form.
        /// </summary>
        /// <returns>
        /// A flat dictionary.
        /// </returns>
        /// <param name="json">Data in JSON form to be flattened.</param>
        public static Dictionary<string, object> DeserializeAndFlatten(string json)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            JToken token = JToken.Parse(json);
            FillDictionaryFromJToken(dict, token, string.Empty);
            return dict;
        }

        /// <summary>
        /// An recursive aide in flattening JSON.
        /// </summary>
        /// <param name="dict">An empty dictionary, returned by pointer.</param>
        /// <param name="token">Current position in the JSON.</param>
        /// <param name="prefix">Current nesting in string form.</param>
        protected static void FillDictionaryFromJToken(Dictionary<string, object> dict, JToken token, string prefix)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        FillDictionaryFromJToken(dict, prop.Value, Join(prefix, prop.Name));
                    }

                    break;
                case JTokenType.Array:
                    int index = 0;
                    foreach (JToken value in token.Children())
                    {
                        FillDictionaryFromJToken(dict, value, Join(prefix, index.ToString()));
                        index++;
                    }

                    break;
                default:
                    dict.Add(prefix, ((JValue)token).Value);
                    break;
            }
        }

        /// <summary>
        /// An aide in flattening JSON.
        /// </summary>
        /// <param name="prefix">A start of a key.</param>
        /// <param name="name">A current key.</param>
        protected static string Join(string prefix, string name)
        {
            return string.IsNullOrEmpty(prefix) ? name : prefix + "." + name;
        }
    }
}