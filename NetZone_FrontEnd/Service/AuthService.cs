using NetZone_FrontEnd.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Docker.DotNet.Models;


namespace NetZone_FrontEnd.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthStateProvider _authStateProvider;

        public AuthService(
            HttpClient http,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider)    // Blazor DI sẽ cung cấp CustomAuthStateProvider
        {
            _http = http;
            _localStorage = localStorage;
            _authStateProvider = (CustomAuthStateProvider)authStateProvider;
        }

        public async Task<bool> RegisterAsync(RegisterModel model)
        { Console.WriteLine("RegisterAsync called"); // Debug log
            var response = await _http.PostAsJsonAsync("api/auth/register", model);
            return response.IsSuccessStatusCode;
        }
   
        public async Task<bool> LoginAsync(LoginModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", model);
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<Models.AuthResponse>();

            if (result?.Token != null)
            {
                // 1. Lưu token vào localStorage
                await _localStorage.SetItemAsync("authToken", result.Token);

                // 2. Thông báo tới AuthStateProvider để cập nhật trạng thái đăng nhập
                _authStateProvider.NotifyUserAuthentication(result.Token);

                return true;
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            // Xóa token
            await _localStorage.RemoveItemAsync("authToken");
            // Thông báo logout
            _authStateProvider.NotifyUserLogout();
        }
        //public async Task<UserProfileModel?> GetUserProfileAsync()
        //{
        //    Console.WriteLine("GetProfileAsync called");
        //    var token = await _localStorage.GetItemAsync<string>("authToken");
        //    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    Console.WriteLine($"Authorization header: {_http.DefaultRequestHeaders.Authorization}");

        //    var result = await _http.GetFromJsonAsync<UserProfileModel>("api/auth/GetProfile");
        //    return result;
        //}

        //public async Task<bool> UpdateUserProfileAsync(UserProfileModel model)
        //{
        //    Console.WriteLine("update called");
        //    var token = await _localStorage.GetItemAsync<string>("authToken");
        //    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var response = await _http.PutAsJsonAsync("api/auth/UpdateProfile", model);
        //    return response.IsSuccessStatusCode;

        //}
        //public async Task<(bool IsSuccess, string? ErrorMessage)> ChangePasswordAsync(ChangePasswordModel model)
        //{
        //    var response = await _http.PostAsJsonAsync("api/auth/changepassword", model);
        //    if (response.IsSuccessStatusCode)
        //        return (true, null);

        //    var error = await response.Content.ReadAsStringAsync();
        //    return (false, error);
        //}

     

    }
}
