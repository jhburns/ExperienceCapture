namespace Carter.App.Export.JsonExtra
{
    using System.Collections.Generic;
    using System.Data;

    using Newtonsoft.Json.Linq;

    public class JsonExtra
    {
        // https://stackoverflow.com/a/36348017
        public static DataTable JsonToTable(string jsonContent)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonContent);
        }

        // https://stackoverflow.com/a/32800161
        public static Dictionary<string, object> DeserializeAndFlatten(string json)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            JToken token = JToken.Parse(json);
            FillDictionaryFromJToken(dict, token, string.Empty);
            return dict;
        }

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

        protected static string Join(string prefix, string name)
        {
            return string.IsNullOrEmpty(prefix) ? name : prefix + "." + name;
        }
    }
}