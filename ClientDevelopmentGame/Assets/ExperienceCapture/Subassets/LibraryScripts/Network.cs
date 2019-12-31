namespace Network
{
    using UnityEngine.Networking;
    using System.Collections;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using System.IO;

    using UnityEngine;

    class HTTPHelpers
    {
        static public IEnumerator post(string url, byte[] data, string token, System.Action<byte[]> onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest request = UnityWebRequest.Put(url, data))
            {
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.SetRequestHeader("Content-Type", "application/bson");
                request.SetRequestHeader("Accept", "application/bson");
                request.SetRequestHeader("Cookie", "ExperienceCapture-Access-Token=" + token);

                request.timeout = 3;

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

        static public IEnumerator post(string url, string data, System.Action<string> onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest request = UnityWebRequest.Put(url, data))
            {
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.SetRequestHeader("Content-Type", "application/bson");
                request.SetRequestHeader("Accept", "application/bson");
                request.timeout = 3;

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    onError(request.error);
                }
                else
                {
                    onSuccess(request.downloadHandler.text);
                }
            }
        }

        static public IEnumerator pollGet(string url, string token, System.Action<string> onSuccess, System.Action<string> onError)
        {
            bool isNotReady = true;
            while(isNotReady)
            {
                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    request.SetRequestHeader("Accept", "application/text");
                    request.SetRequestHeader("Cookie", "ExperienceCapture-Claim-Token=" + token);
                    request.timeout = 3;

                    yield return request.SendWebRequest();

                    if (request.isNetworkError || request.isHttpError)
                    {
                        onError(request.error);
                    }
                    else
                    {
                        if (request.responseCode == 200) {
                            onSuccess(request.downloadHandler.text);
                            isNotReady = false;
                        }
                    }
                    
                    yield return new WaitForSeconds(3);
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

    public class SecretStorage
    {
        public string accessToken { get; private set; }
        public SecretStorage(string a)
        {
            accessToken = a;
        }
    }
}