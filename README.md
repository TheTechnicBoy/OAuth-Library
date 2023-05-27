
# OAuth Library




## What is OAuth and how it works?
OAuth makes it possible for third-party services to use the account information of end users.
In order to establish an identity between customers and service providers, OAuth uses authorization tokens rather than sharing password information.
## Open URL in Broswer

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
    var data_2 = dc.GetCurrentAuthorizationInformation(Data.AccessToken);

    Console.WriteLine("----------");
    Console.WriteLine($"State: {Data.State}");
    Console.WriteLine($"Code: {Data.AccessToken}");
    Console.WriteLine(data_2.user.username);
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
    DiscordOAuth.Scope.activities_read,
    DiscordOAuth.Scope.activities_write,
    DiscordOAuth.Scope.applications_builds_read,
    DiscordOAuth.Scope.applications_builds_upload,
    DiscordOAuth.Scope.applications_commands,
    DiscordOAuth.Scope.applications_commands_update,
    DiscordOAuth.Scope.applications_commands_permissions_update,
    DiscordOAuth.Scope.applications_entitlements,
    DiscordOAuth.Scope.applications_store_update,
    DiscordOAuth.Scope.bot,
    DiscordOAuth.Scope.connections,
    DiscordOAuth.Scope.dm_channels_read,
    DiscordOAuth.Scope.email,
    DiscordOAuth.Scope.gdm_join,
    DiscordOAuth.Scope.guilds,
    DiscordOAuth.Scope.guilds_join,
    DiscordOAuth.Scope.guilds_members_read,
    DiscordOAuth.Scope.identify,
    DiscordOAuth.Scope.messages_read,
    DiscordOAuth.Scope.relationships_read,
    DiscordOAuth.Scope.role_connections_write,
    DiscordOAuth.Scope.rpc,
    DiscordOAuth.Scope.rpc_activities_write,
    DiscordOAuth.Scope.rpc_notifications_read,
    DiscordOAuth.Scope.rpc_voice_read,
    DiscordOAuth.Scope.rpc_voice_write,
    DiscordOAuth.Scope.voice,
    DiscordOAuth.Scope.webhook_incoming,
}
            
```



## Authors

- [@TheTechnicBoy](https://www.github.com/TheTechnicBoy)

