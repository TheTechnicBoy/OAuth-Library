# OAuth Library

## What is OAuth and how it works?
OAuth makes it possible for third-party services to use the account information of end users.
In order to establish an identity between customers and service providers, OAuth uses authorization tokens rather than sharing password information.

## Open URL in Broswer *[Optional]*
```csharp
public static void OpenBroswer(string URL)
{
    Process Browser = new Process();
    Browser.StartInfo.UseShellExecute = true;
    Browser.StartInfo.FileName = URL;
    Browser.Start();
}
```

## Discord OAuth
- How to create an DiscordOAuth Instance?
```csharp
using OAuth_Library;

//Create a new Instance of the DiscordOAuth
DiscordOAuth dc = new DiscordOAuth("[RedirectUrl]", "[ClientID]", "[ClientSecret]");

//This Event is triggered, when someone authorizes or denies Authorization.
dc.OAuth += OAuth;

//This Event is triggered, when someone denies Authorization.
dc.OnError += OnError;

//Get a Link, where someone can authorize
string URL = dc.GenerateOAuth(new List<DiscordOAuth.Scope>() { DiscordOAuth.Scope.identify})


//Example of how to get the authorization data
public static void OAuth(object sender, DiscordOAuth.OAuthData Data)
{
    var data_2 = dc.GetAuthorizationInformation(Data.AccessToken);
    
    Console.WriteLine("----------");
    Console.WriteLine($"State:          {Data.State}");
    Console.WriteLine($"AccessToken:    {Data.AccessToken}");
    Console.WriteLine($"Display Name:   {data_2.user.username}");
    Console.WriteLine($"E-Mail:         {data_2.user.email}");
    Console.WriteLine($"Scopes:");
    foreach ( var item in Data.Scopes) 
    {
        
        Console.WriteLine($"                {item}");
    }
    Console.WriteLine("----------");
    Console.WriteLine("");
}

//Example of how to print the Error in the Console
public static void OnError(object sender, DiscordOAuth.ErrorData args)
{
    Console.WriteLine(args.Message + "  -  " + args.State);
}
``` 

####

- Which Scopes are available?
  - *If you want to know more about the Scopes, please visit the [Discord Developer Portal](https://discord.com/developers/docs/topics/oauth2#shared-resources-oauth2-scopes)*
```csharp
var Scopes = new List<DiscordOAuth.Scope>() 
{
    DiscordOAuth.Scope.identify,
    DiscordOAuth.Scope.email,
    DiscordOAuth.Scope.connections,
    DiscordOAuth.Scope.guilds,
    DiscordOAuth.Scope.guilds_join,
    DiscordOAuth.Scope.guilds_members_read,
    DiscordOAuth.Scope.gdm_join,
    DiscordOAuth.Scope.rpc,
    DiscordOAuth.Scope.rpc_notifications_read,
    DiscordOAuth.Scope.rpc_voice_read,
    DiscordOAuth.Scope.rpc_voice_write,
    DiscordOAuth.Scope.rpc_video_read,
    DiscordOAuth.Scope.rpc_video_write,
    DiscordOAuth.Scope.rpc_screenshare_read,
    DiscordOAuth.Scope.rpc_screenshare_write,
    DiscordOAuth.Scope.rpc_activities_write,
    DiscordOAuth.Scope.bot,
    DiscordOAuth.Scope.webhook_incoming,
    DiscordOAuth.Scope.messages_read,
    DiscordOAuth.Scope.applications_builds_upload,
    DiscordOAuth.Scope.applications_builds_read,
    DiscordOAuth.Scope.applications_commands,
    DiscordOAuth.Scope.applications_store_update,
    DiscordOAuth.Scope.applications_entitlements,
    DiscordOAuth.Scope.activities_read,
    DiscordOAuth.Scope.activities_write,
    DiscordOAuth.Scope.relationships_read,
    DiscordOAuth.Scope.dm_channels_read,    
    DiscordOAuth.Scope.role_connections_write,
    DiscordOAuth.Scope.voice,           
}         
```

## Twitch OAuth
- How to create an TwitchOAuth Instance?
```csharp
using OAuth_Library;

//Create a new Instance of the TwitchOAuth
TwitchOAuth twitch = new TwitchOAuth("[RedirectUrl]", "[ClientID]", "[ClientSecret]");

//This Event is triggered, when someone authorizes or denies Authorization.
twitch.OAuth += OAuth;

//This Event is triggered, when someone denies Authorization.
twitch.OnError += OnError;

//Get a Link, where someone can authorize
string URL = twitch.GenerateOAuth(new List<TwitchOAuth.Scope>() { TwitchOAuth.Scope.user_read_email })


//Example of how to get the authorization data
public static void OAuth(object sender, TwitchOAuth.OAuthData Data)
{
    var data_2 = twitch.GetAuthorizationInformation(Data.AccessToken);
                
    Console.WriteLine("----------");
    Console.WriteLine($"State:          {Data.State}");
    Console.WriteLine($"AccessToken:    {Data.AccessToken}");
    Console.WriteLine($"Display Name:   {data_2.display_name}");
    Console.WriteLine($"E-Mail:         {data_2.email}");
    Console.WriteLine($"Scopes:");
    foreach (var item in Data.Scopes)
    {

        Console.WriteLine($"                {item}");
    }
    Console.WriteLine("----------");
    Console.WriteLine("");
}

//Example of how to print the Error in the Console
public static void OnError(object sender, TwitchOAuth.ErrorData args)
{
    Console.WriteLine(args.Message + "  -  " + args.State);
}
``` 

####

- Which Scopes are available?
  - *If you want to know more about the Scopes, please visit the [Twitch Docs](https://dev.twitch.tv/docs/authentication/scopes/)*
```csharp
var Scopes = new List<DiscordOAuth.Scope>() 
{
    //Twitch API Scopes
    TwitchOAuth.Scope.analytics_read_extensions,
    TwitchOAuth.Scope.analytics_read_games,
    TwitchOAuth.Scope.bits_read,
    TwitchOAuth.Scope.channel_manage_broadcast,
    TwitchOAuth.Scope.channel_read_charity,
    TwitchOAuth.Scope.channel_edit_commercial,
    TwitchOAuth.Scope.channel_read_editors,
    TwitchOAuth.Scope.channel_manage_extensions,
    TwitchOAuth.Scope.channel_read_goals,
    TwitchOAuth.Scope.channel_read_guest_star,
    TwitchOAuth.Scope.channel_manage_guest_star,
    TwitchOAuth.Scope.channel_read_hype_train,
    TwitchOAuth.Scope.channel_manage_moderators,
    TwitchOAuth.Scope.channel_read_polls,
    TwitchOAuth.Scope.channel_manage_polls,
    TwitchOAuth.Scope.channel_read_predictions,
    TwitchOAuth.Scope.channel_manage_predictions,
    TwitchOAuth.Scope.channel_manage_raids,
    TwitchOAuth.Scope.channel_read_redemptions,
    TwitchOAuth.Scope.channel_manage_redemptions,
    TwitchOAuth.Scope.channel_manage_schedule,
    TwitchOAuth.Scope.channel_read_stream_key,
    TwitchOAuth.Scope.channel_read_subscriptions,
    TwitchOAuth.Scope.channel_manage_videos,
    TwitchOAuth.Scope.channel_read_vips,
    TwitchOAuth.Scope.channel_manage_vips,
    TwitchOAuth.Scope.clips_edit,
    TwitchOAuth.Scope.moderation_read,
    TwitchOAuth.Scope.moderator_manage_announcements,
    TwitchOAuth.Scope.moderator_manage_automod,
    TwitchOAuth.Scope.moderator_read_automod_settings,
    TwitchOAuth.Scope.moderator_manage_automod_settings,
    TwitchOAuth.Scope.moderator_manage_banned_users,
    TwitchOAuth.Scope.moderator_read_blocked_terms,
    TwitchOAuth.Scope.moderator_manage_blocked_terms,
    TwitchOAuth.Scope.moderator_manage_chat_messages,
    TwitchOAuth.Scope.moderator_read_chat_settings,
    TwitchOAuth.Scope.moderator_manage_chat_settings,
    TwitchOAuth.Scope.moderator_read_chatters,
    TwitchOAuth.Scope.moderator_read_followers,
    TwitchOAuth.Scope.moderator_read_guest_star,
    TwitchOAuth.Scope.moderator_manage_guest_star,
    TwitchOAuth.Scope.moderator_read_shield_mode,
    TwitchOAuth.Scope.moderator_manage_shield_mode,
    TwitchOAuth.Scope.moderator_read_shoutouts,
    TwitchOAuth.Scope.moderator_manage_shoutouts,
    TwitchOAuth.Scope.user_edit,
    TwitchOAuth.Scope.user_edit_follows,
    TwitchOAuth.Scope.user_read_blocked_users,
    TwitchOAuth.Scope.user_manage_blocked_users,
    TwitchOAuth.Scope.user_read_broadcast,
    TwitchOAuth.Scope.user_manage_chat_color,
    TwitchOAuth.Scope.user_read_email,
    TwitchOAuth.Scope.user_read_follows,
    TwitchOAuth.Scope.user_read_subscriptions,
    TwitchOAuth.Scope.user_manage_whispers,   
    
    //Chat and PubSub Scopes
    TwitchOAuth.Scope.channel_moderate,
    TwitchOAuth.Scope.chat_edit,
    TwitchOAuth.Scope.chat_read,
    TwitchOAuth.Scope.whispers_read,
    TwitchOAuth.Scope.whispers_edit        
}         
``` 

## Authors

- [@TheTechnicBoy](https://www.github.com/TheTechnicBoy)

