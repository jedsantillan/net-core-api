using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace FarmHub.Domain.Services
{
    public class GoogleRecaptchaService : IGoogleRecaptchaService
    {
        private readonly GoogleRecaptchaConfig _config;
        private readonly HttpClient _httpClient;

        public GoogleRecaptchaService(IOptions<GoogleRecaptchaConfig> config)
        {
            _config = config.Value;
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.google.com")
            };
        }

        public async Task<ReCaptchaResponseModel> ValidateRecaptchaToken(string token)
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("secret", _config.SecretKey),
                new KeyValuePair<string, string>("response", token)
            };

            var responseMessage = await _httpClient.PostAsync("/recaptcha/api/siteverify", new FormUrlEncodedContent(formData));

            responseMessage.EnsureSuccessStatusCode();

            await using var responseStream = await responseMessage.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<ReCaptchaResponseModel>(responseStream);
        }
    }

    public interface IGoogleRecaptchaService
    {
        Task<ReCaptchaResponseModel> ValidateRecaptchaToken(string token);
    }
    
    public sealed class ReCaptchaResponseModel
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("challenge_ts")]
        public DateTime ChallengeTimestamp { get; set; }

        [JsonPropertyName("hostname")]
        public string HostName { get; set; }

        [JsonPropertyName("score")]
        public float Score { get; set; }

        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
    
}