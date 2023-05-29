using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;


namespace OAuth_Library
{
    public class DiscordOAuth
    {
        #region Variables
        private HttpListener HttpListener = new HttpListener();

        public string RedirectUrl { get; private set; }
        public string ClientId { get; private set; }
        public string ClientSecret {get; private set;}
        public EventHandler<OAuthData> OAuth;
        public EventHandler<ErrorData> OnError;
        #endregion

        /// <summary>
        /// Generates a new Instance.
        /// </summary>
        /// <param name="_RedirectURL">A Redirect URL corresponding to one of the redirects in the Discord Application Panel</param>
        /// <param name="_ClientId">The Client ID of the Application</param>
        /// <param name="_ClientSecret">The Client Secret of the Application</param>
        public DiscordOAuth(string _RedirectURL, string _ClientId, string _ClientSecret)
        {
            RedirectUrl = _RedirectURL;
            ClientId = _ClientId;
            ClientSecret = _ClientSecret;

            HttpListener.Prefixes.Add(RedirectUrl);
            HttpListener.Start();
            StartListenerAsync();
        }

        /// <summary>
        /// If you want to know more about the Scopes, please visit the <see href="https://discord.com/developers/docs/topics/oauth2#shared-resources-oauth2-scopes">Discord Developer Portal</see>
        /// </summary>
        public enum Scope
        {
            identify,
            email,
            connections,

            guilds,
            guilds_join,
            guilds_members_read,
            gdm_join,

            rpc,
            rpc_notifications_read,
            rpc_voice_read,
            rpc_voice_write,
            rpc_video_read,
            rpc_video_write,
            rpc_screenshare_read,
            rpc_screenshare_write,
            rpc_activities_write,

            bot,
            webhook_incoming,
            messages_read,

            applications_builds_upload,
            applications_builds_read,
            applications_commands,
            applications_store_update,
            applications_entitlements,

            activities_read,
            activities_write,

            relationships_read,

            [Display(Name = "dm_channels.read")]
            dm_channels_read,
            [Display(Name = "role_connections.write")]
            role_connections_write,

            voice,
            
        }

        #region JsonClasses
        public class ErrorData
        {
            public string Message { get; set; }
            public string State { get; set; }
        }
        public class OAuthData
        {
            public string State { get; set; }
            public string AccessToken { get; set; }
            public string TokenType { get; set; }
            public DateTime Expires { get; set; }
            public string RefreshToken { get; set; }
            public List<Scope> Scopes { get; set; }
        }
        private class RawExchangeData
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
        }
        private class RawAuthorizationInformation
        {
            public Application application { get; set; }
            public List<string> scopes { get; set; }
            public DateTime expires { get; set; }
            public User user { get; set; }
        }
        public class AuthorizationInformation
        {
            public Application application { get; set; }
            public List<Scope> scopes { get; set; }
            public DateTime expires { get; set; }
            public User user { get; set; }
        }
        public class User
        {
            public string id { get; set; }
            public string username { get; set; }
            public string discriminator { get; set; }
            public string avatar { get; set; }
            public bool? bot { get; set; }
            public bool? system { get; set; }
            public bool? mfa_enabled { get; set; }
            public string? banner { get; set; }
            public int? accent_color { get; set; }
            public string? locale { get; set; }
            public bool? verified { get; set; }
            public string? email { get; set; }
            public int flags { get; set; }
            public int premium_type { get; set; }
            public int public_flags { get; set; }
        }
        public class Application
        {
            public string id { get; set; }
            public string name { get; set; }
            public string icon { get; set; }
            public string description { get; set; }
            public string summary { get; set; }
            public object type { get; set; }
            public string cover_image { get; set; }
            public bool hook { get; set; }
            public string guild_id { get; set; }
            public bool bot_public { get; set; }
            public bool bot_require_code_grant { get; set; }
            public string verify_key { get; set; }
            public int flags { get; set; }
        }
        #endregion

        #region Functions
        private async Task StartListenerAsync()
        {
            while (true)
            {

                HttpListenerContext context = await HttpListener.GetContextAsync();
                string ResponseString = "<script>function Close(){window.close();}</script><body onload=\"Close()\"></body>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ResponseString);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();

                string RawURL = context.Request.RawUrl;
                if (RawURL != "/favicon.ico")
                {
                    RawURL = RawURL.Replace("/?", "");
                    string[] RawData = RawURL.Split("&");
                    Dictionary<string, string> Data = new Dictionary<string, string>();
                    foreach (string data in RawData)
                    {
                        Data.Add(data.Split("=")[0].ToLower(), HttpUtility.UrlDecode(data.Split("=")[1]));
                    }

                    if (Data.ContainsKey("error"))
                    {
                        OnError?.Invoke(this, new ErrorData() { Message = $"Error: " + Data["error_description"], State = Data["state"] });
                    }
                    else
                    {
                        var _RawExchangeData = ExchangeCodeForBearer(Data["code"]);
                        var _Scopes = new List<Scope>();
                        if (_RawExchangeData.scope != null)
                        {
                            foreach (string scope in _RawExchangeData.scope.Split(" "))
                            {
                                bool DisplayNameFound = false;
                                foreach(var name in Enum.GetNames(typeof(Scope)).ToList())
                                {
                                    var DisplayName = typeof(Scope)?.GetMember(name)?.First()?.GetCustomAttribute<DisplayAttribute>()?.Name;
                                    if(DisplayName != null)
                                    {
                                        if(DisplayName == scope)
                                        {
                                            DisplayNameFound = true;
                                            _Scopes.Add(Enum.Parse<Scope>(name));
                                        }
                                    } 

                                }
                                if (!DisplayNameFound)
                                {
                                    _Scopes.Add(Enum.Parse<Scope>(scope));
                                }

                            }
                        }
                        
                        OAuth?.Invoke(this, new OAuthData() {AccessToken = _RawExchangeData.access_token, Expires = DateTime.Now.AddSeconds(_RawExchangeData.expires_in), RefreshToken = _RawExchangeData.refresh_token, Scopes = _Scopes, State = Data["state"] });
                    }
                }
            }
        }

        private RawExchangeData ExchangeCodeForBearer(string _Code)
        {

            string url = $"https://discord.com/api/oauth2/token";
            HttpClient client = new HttpClient();
            var requestData = new Dictionary<string, string>();
            requestData.Add("client_id", ClientId);
            requestData.Add("grant_type", "authorization_code");
            requestData.Add("code", _Code);
            requestData.Add("redirect_uri", RedirectUrl);
            requestData.Add("client_secret", ClientSecret);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new FormUrlEncodedContent(requestData)
            };

            var response = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            RawExchangeData _RawExchangeData = JsonConvert.DeserializeObject<RawExchangeData>(response);
            return _RawExchangeData;
        }

        /// <summary>
        /// Generatas an OAuth Link
        /// </summary>
        /// <param name="_Scopes">The scopes to which the application should have access</param>
        /// <param name="_State">A number that determines the ID of the OAuth. If set to -1 the System is going to pick a random number.</param>
        /// <returns>The URL of the OAuth</returns>
        public string GenerateOAuth(List<Scope> _Scopes, int _State = -1)
        {
            string FormatedScopes;
            if (_Scopes.Count > 0)
            {
                FormatedScopes = "";
                foreach (Scope _Scope in _Scopes) 
                {
                    var tmpScope = _Scope.GetType()?.GetMember(_Scope.ToString())?.First()?.GetCustomAttribute<DisplayAttribute>()?.Name;
                    if(tmpScope != null)
                    {
                        FormatedScopes += tmpScope + "%20";
                    }
                    else
                    {
                        FormatedScopes += _Scope.ToString().Replace("_", ".") + "%20";
                    }
                }
                FormatedScopes = FormatedScopes.Remove(FormatedScopes.Length - 3);
            }
            else
            {
                FormatedScopes = null;
            }
            

            if (_State <= 0)
            {
                Random rnd = new Random();
                _State = rnd.Next();
            }
            string url = $"https://discord.com/oauth2/authorize?response_type=code&client_id={ClientId}&state={_State}&redirect_uri={HttpUtility.UrlEncode(RedirectUrl)}&scope={FormatedScopes}";
            return url;
        }

        /// <summary>
        /// Obtain the AuthorizationInformation
        /// </summary>
        /// <param name="BearerToken">The Bearer Token, which had been exchanged previously</param>
        /// <returns>An Object with the informations</returns>
        public AuthorizationInformation GetAuthorizationInformation(string BearerToken)
        {
            string url = $"https://discord.com/api/v10/oauth2/@me";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer  {BearerToken}");
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };

            var response = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            var _RawAuthorizationInformation = JsonConvert.DeserializeObject<RawAuthorizationInformation>(response);
            var _Scopes = new List<Scope>();
            foreach (string scope in _RawAuthorizationInformation.scopes)
            {
                bool DisplayNameFound = false;
                foreach (var name in Enum.GetNames(typeof(Scope)).ToList())
                {
                    var DisplayName = typeof(Scope)?.GetMember(name)?.First()?.GetCustomAttribute<DisplayAttribute>()?.Name;
                    if (DisplayName != null)
                    {
                        if (DisplayName == scope)
                        {
                            DisplayNameFound = true;
                            _Scopes.Add(Enum.Parse<Scope>(name));
                        }
                    }

                }
                if (!DisplayNameFound)
                {
                    _Scopes.Add(Enum.Parse<Scope>(scope));
                }

            }
            
            return new AuthorizationInformation() { application = _RawAuthorizationInformation.application, expires = _RawAuthorizationInformation.expires, scopes = _Scopes, user = _RawAuthorizationInformation.user };
        }
        #endregion
    }
}
