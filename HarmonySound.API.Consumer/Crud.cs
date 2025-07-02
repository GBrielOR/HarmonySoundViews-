using Newtonsoft.Json;
using System.Text;

namespace HarmonySound.API.Consumer
{
    public static class Crud<T>
    {
        public static string EndPoint { get; set; }

        public static List<T> GetAll()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(EndPoint).Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<T>>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static T GetById(int id)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"{EndPoint}/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static T Create(T item)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(
                        EndPoint,
                        new StringContent(
                            JsonConvert.SerializeObject(item),
                            Encoding.UTF8,
                            "application/json"
                        )
                    ).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static bool Update(int id, T item)
        {
            using (var client = new HttpClient())
            {
                var response = client.PutAsync(
                        $"{EndPoint}/{id}",
                        new StringContent(
                            JsonConvert.SerializeObject(item),
                            Encoding.UTF8,
                            "application/json"
                        )
                    ).Result;

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static bool Delete(int id)
        {
            using (var client = new HttpClient())
            {
                var response = client.DeleteAsync($"{EndPoint}/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }
    }
}
