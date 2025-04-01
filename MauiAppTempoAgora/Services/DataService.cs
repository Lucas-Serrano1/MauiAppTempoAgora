using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;
            string chave = "ac8e55bf1f4282d5e5f0b84e25ec2f1d";
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cidade}&units=metric&appid={chave}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Configura timeout para 10 segundos
                    client.Timeout = TimeSpan.FromSeconds(10);

                    HttpResponseMessage resp = await client.GetAsync(url);

                    if (resp.IsSuccessStatusCode)
                    {
                        string json = await resp.Content.ReadAsStringAsync();
                        var rascunho = JObject.Parse(json);

                        DateTime time = new();
                        DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                        DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                        t = new()
                        {
                            lat = (double)rascunho["coord"]["lat"],
                            lon = (double)rascunho["coord"]["lon"],
                            description = (string)rascunho["weather"][0]["description"],
                            main = (string)rascunho["weather"][0]["main"],
                            temp_min = (double)rascunho["main"]["temp_min"],
                            temp_max = (double)rascunho["main"]["temp_max"],
                            speed = (double)rascunho["wind"]["speed"],
                            visibility = (int)rascunho["visibility"],
                            sunrise = sunrise.ToString("HH:mm"),
                            sunset = sunset.ToString("HH:mm"),
                        };
                    }
                    else if (resp.StatusCode == HttpStatusCode.NotFound) // 404
                    {
                        throw new Exception($"Cidade '{cidade}' não encontrada. Verifique o nome e tente novamente.");
                    }
                    else if (resp.StatusCode == HttpStatusCode.Unauthorized) // 401
                    {
                        throw new Exception("Erro de autenticação com a API. Chave inválida.");
                    }
                    else
                    {
                        throw new Exception($"Erro na API: {resp.StatusCode} - {resp.ReasonPhrase}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Erro de conexão: Verifique sua internet e tente novamente.");
            }
            catch (TaskCanceledException)
            {
                throw new Exception("A requisição demorou muito. Verifique sua conexão.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter previsão: {ex.Message}");
            }

            return t;
        }
    }
}