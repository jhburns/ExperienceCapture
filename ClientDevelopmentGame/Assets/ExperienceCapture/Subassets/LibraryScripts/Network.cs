namespace Network
{
    using UnityEngine.Networking;
    using System.Collections;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using System.IO;

    class HTTPHelpers
    {

        static public IEnumerator post(string url, byte[] data, System.Action<byte[]> onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest request = UnityWebRequest.Put(url, data))
            {
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.SetRequestHeader("Content-Type", "application/bson");
                request.SetRequestHeader("Accept", "application/bson");
                request.timeout = 2;

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    onError(request.error);
                }
                else
                {
                    onSuccess(request.downloadHandler.data);
                }
            }
        }

    }

    class Serial
    {
        public static byte[] toBSON(object obj)
        {
            MemoryStream memStream = new MemoryStream();
            using (BsonWriter writer = new BsonWriter(memStream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
            }
            return memStream.ToArray();
        }

        public static T fromBSON<T>(MemoryStream memStream)
        {
            T obj;
            using (BsonReader reader = new BsonReader(memStream))
            {
                JsonSerializer serializer = new JsonSerializer();

                obj = serializer.Deserialize<T>(reader);
            }
            return obj;
        }
    }
}