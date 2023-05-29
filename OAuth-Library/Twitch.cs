using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;


namespace OAuth_Library
{
    public class TwitchOAuth
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
        /// <param name="_RedirectURL">A Redirect URL corresponding to one of the redirects in the Twitch Developers Panel</param>
        /// <param name="_ClientId">The Client ID of the Application</param>
        /// <param name="_ClientSecret">The Client Secret of the Application</param>
        public TwitchOAuth(string _RedirectURL, string _ClientId, string _ClientSecret)
        {
            RedirectUrl = _RedirectURL;
            ClientId = _ClientId;
            ClientSecret = _ClientSecret;

            HttpListener.Prefixes.Add(RedirectUrl);
            HttpListener.Start();
            StartListenerAsync();
        }
        
        /// <summary>
        /// If you want to know more about the Scopes, please visit the <see href="https://dev.twitch.tv/docs/authentication/scopes/">Twitch Docs</see>
        /// </summary>
        public enum Scope
        {
            #region Twitch API scopes
            analytics_read_extensions,
            analytics_read_games,
            bits_read,
            channel_manage_broadcast,
            channel_read_charity,
            channel_edit_commercial,
            channel_read_editors,
            channel_manage_extensions,
            channel_read_goals,
            channel_read_guest_star,
            channel_manage_guest_star,
            channel_read_hype_train,
            channel_manage_moderators,
            channel_read_polls,
            channel_manage_polls,
            channel_read_predictions,
            channel_manage_predictions,
            channel_manage_raids,
            channel_read_redemptions,
            channel_manage_redemptions,
            channel_manage_schedule,
            channel_read_stream_key,
            channel_read_subscriptions,
            channel_manage_videos,
            channel_read_vips,
            channel_manage_vips,
            clips_edit,
            moderation_read,
            moderator_manage_announcements,
            moderator_manage_automod,
            moderator_read_automod_settings,
            moderator_manage_automod_settings,
            moderator_manage_banned_users,
            moderator_read_blocked_terms,
            moderator_manage_blocked_terms,
            moderator_manage_chat_messages,
            moderator_read_chat_settings,
            moderator_manage_chat_settings,
            moderator_read_chatters,
            moderator_read_followers,
            moderator_read_guest_star,
            moderator_manage_guest_star,
            moderator_read_shield_mode,
            moderator_manage_shield_mode,
            moderator_read_shoutouts,
            moderator_manage_shoutouts,
            user_edit,
            user_edit_follows,
            user_read_blocked_users,
            user_manage_blocked_users,
            user_read_broadcast,
            user_manage_chat_color,
            user_read_email,
            user_read_follows,
            user_read_subscriptions,
            user_manage_whispers,
            #endregion

            #region Chat and PubSub scopes
            channel_moderate,
            chat_edit,
            chat_read,
            whispers_read,
            whispers_edit
            #endregion
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
            public List<string> scope { get; set; }
        }
        private class RawAuthorizationInformation
        {
            public List<AuthorizationInformation> data { get; set; }
        }
        public class AuthorizationInformation
        {
            public string id { get; set; }
            public string login { get; set; }
            public string display_name { get; set; }
            public string type { get; set; }
            public string broadcaster_type { get; set; }
            public string description { get; set; }
            public string profile_image_url { get; set; }
            public string offline_image_url { get; set; }
            public int view_count { get; set; }
            public string email { get; set; }
            public DateTime created_at { get; set; }
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
                        OnError?.Invoke(this, new ErrorData() { Message = Data["error_description"], State = Data["state"] });
                    }
                    else
                    {
                        var _RawExchangeData = ExchangeCodeForBearer(Data["code"]);
                        var _Scopes = new List<Scope>();
                        if(_RawExchangeData.scope != null)
                        {
                            foreach (string scope in _RawExchangeData.scope)
                            {
                                _Scopes.Add(Enum.Parse<Scope>(scope.Replace(":", "_")));
                            }
                        }
                        
                        OAuth?.Invoke(this, new OAuthData() { AccessToken = _RawExchangeData.access_token, Expires = DateTime.Now.AddSeconds(_RawExchangeData.expires_in), RefreshToken = _RawExchangeData.refresh_token, Scopes = _Scopes, State = Data["state"] });
                    }
                    
                }
            }
        }

        private RawExchangeData ExchangeCodeForBearer(string _Code)
        {

            string url = $"https://id.twitch.tv/oauth2/token";
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
                    FormatedScopes += _Scope.ToString().Replace("_", ":") + "%20";
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
            string url = $"https://id.twitch.tv/oauth2/authorize?response_type=code&client_id={ClientId}&state={_State}&redirect_uri={HttpUtility.UrlEncode(RedirectUrl)}&scope={FormatedScopes}";
            return url;
        }

        /// <summary>
        /// Obtain the AuthorizationInformation
        /// </summary>
        /// <param name="BearerToken">The Bearer Token, which had been exchanged previously</param>
        /// <returns>An Object with the informations</returns>
        public AuthorizationInformation GetAuthorizationInformation(string BearerToken)
        {
            string url = $"https://api.twitch.tv/helix/users";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer  {BearerToken}");
            client.DefaultRequestHeaders.Add("Client-Id", $"{ClientId}");
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };

            var response = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<RawAuthorizationInformation>(response).data[0];
        }
        #endregion
    }
}
