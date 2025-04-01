using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(txt_cidade.Text))
                {
                    await DisplayAlert("Atenção", "Por favor, digite o nome de uma cidade.", "OK");
                    return;
                }

                
                lbl_res.Text = "Buscando dados...";
                ((Button)sender).IsEnabled = false; 

                
                Tempo? t = await DataService.GetPrevisao(txt_cidade.Text.Trim());

                if (t != null)
                {
                    string dados_previsao = $"Condição Atual: {t.main} ({t.description})\n" +
                                          $"Temperatura: {t.temp_min}°C ~ {t.temp_max}°C\n" +
                                          $"Vento: {t.speed} m/s\n" +
                                          $"Visibilidade: {t.visibility / 1000} km\n" +
                                          $"Nascer do Sol: {t.sunrise}\n" +
                                          $"Pôr do Sol: {t.sunset}\n" +
                                          $"Localização: Lat {t.lat}, Lon {t.lon}";

                    lbl_res.Text = dados_previsao;
                }
                else
                {
                    await DisplayAlert("Aviso", "Dados indisponíveis para esta cidade.", "OK");
                    lbl_res.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
                lbl_res.Text = string.Empty;
            }
            finally
            {
                ((Button)sender).IsEnabled = true; 
            }
        }
    }
}