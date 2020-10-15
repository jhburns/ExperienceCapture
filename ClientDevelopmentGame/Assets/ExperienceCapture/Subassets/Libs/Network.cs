namespace Capture.Internal.Network
{
    using System;

    using UnityEngine.Networking;
    using System.Collections;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using System.IO;

    using UnityEngine;

    /// <summary>
    /// Wrappers methods for UnityWebRequest.
    /// </summary>
    class HTTPHelpers
    {
        /// <summary>
        /// Make a HTTP POST request
        /// </summary>
        /// <param name="url">A URL to post data.</param>
        /// <param name="data">A BSON encoded object.</param>
        /// <param name="token">An access token.</param>
        /// <param name="onSuccess">Callback, called when status is in the 200 range.</param>
        /// <param name="onError">Callback, called when the request has an error.</param>
        /// <returns>An IEnumerator.</returns>
        static public IEnumerator post(
            string url,
            byte[] data,
            string token,
            Action<byte[]> onSuccess,
            Action<string> onError)
        {
            using (UnityWebRequest request = UnityWebRequest.Put(url, data))
            {
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.SetRequestHeader("Content-Type", "application/bson");
                request.SetRequestHeader("Accept", "application/bson");
                request.SetRequestHeader("Cookie", "ExperienceCapture-Access-Token=" + token);
                request.timeout = 3; // 3 seconds

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("Error Code: " + request.responseCode);
                    onError(request.error);
                }
                else
                {
                    onSuccess(request.downloadHandler.data);
                }
            }
        }

        /// <summary>
        /// Make a HTTP GET request with a claim token.
        /// Repeats Until the status code is 200 exactly.
        /// </summary>
        /// <param name="url">A URL to get data.</param>
        /// <param name="token">An access token.</param>
        /// <param name="onSuccess">Callback, called when status code is 200 exactly.</param>
        /// <param name="onError">Callback, called when the request has an error.</param>
        /// <returns>An IEnumerator.</returns>
        static public IEnumerator pollGet(string url,
            string token,
            Action<byte[]> onSuccess,
            Action<string> onError)
        {
            bool isNotReady = true;
            
            while(isNotReady)
            {
                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    request.SetRequestHeader("Accept", "application/text");
                    request.SetRequestHeader("Cookie", "ExperienceCapture-Claim-Token=" + token);
                    request.timeout = 1; // 1 second

                    yield return request.SendWebRequest();

                    if (request.isNetworkError || request.isHttpError)
                    {
                        // Should continue to poll even after error
                        Debug.Log(request.responseCode);
                        onError(request.error);
                        yield break;
                    }
                    else
                    {
                        if (request.responseCode == 200) {
                            onSuccess(request.downloadHandler.data);
                            isNotReady = false;
                        }

                        yield return new WaitForSeconds(3);
                    }
                }
            }
        }

        /// <summary>
        /// Make a HTTP DELETE request.
        /// </summary>
        /// <param name="url">A URL to post data.</param>
        /// <param name="token">An access token.</param>
        /// <param name="onSuccess">Callback, called when status is in the 200 range.</param>
        /// <param name="onError">Callback, called when the request has an error.</param>
        /// <returns>An IEnumerator.</returns>
        static public IEnumerator delete(string url,
            string token,
            Action onSuccess,
            Action<string> onError)
        {
            using (UnityWebRequest request = UnityWebRequest.Delete(url))
            {
                request.method = UnityWebRequest.kHttpVerbDELETE;
                request.SetRequestHeader("Accept", "application/text");
                request.SetRequestHeader("Cookie", "ExperienceCapture-Access-Token=" + token);
                request.timeout = 3; // 3 seconds

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("Error Code: " + request.responseCode);
                    onError(request.error);
                }
                else
                {
                    onSuccess();
                }
            }
        }
    }

    /// <summary>
    /// Helpers for serializing objects.
    /// </summary>
    class Serial
    {
        /// <summary>
        /// Serialize an object.
        /// </summary>
        /// <param name="obj">An object to be serialized</param>
        /// <returns>An object, BSON encoded.</returns>
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

        /// <summary>
        /// Deserialize to an object.
        /// </summary>
        /// <typeparam name="T">Of the returned object, required.</typeparam>
        /// <param name="data">BSON encoded.</param>
        /// <returns>Deserialized data.</returns>
        public static T fromBSON<T>(byte[] data)
        {
            T obj;
            MemoryStream memStream = new MemoryStream(data);

            using (BsonReader reader = new BsonReader(memStream))
            {
                JsonSerializer serializer = new JsonSerializer();

                obj = serializer.Deserialize<T>(reader);
            }
 
            return obj;
        }
    }

    /// <summary>
    /// A wrapper to make secrets easier to store.
    /// </summary>
    public class SecretStorage
    {
        /// <summary>
        /// A token needed to authenticate with the server.
        /// </summary>
        public string accessToken { get; private set; }

        /// <summary>
        /// Construct a SecretStorage.
        /// </summary>
        /// <param name="a">An access token</param>
        public SecretStorage(string a)
        {
            accessToken = a;
        }
    }
}