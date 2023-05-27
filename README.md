
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

//Subscribe to the Event. This Event is triggered, when someone authorizes or denies Authorization.
dc.OAuth += OAuth;

//Get a Link, where someone can authorize
string URL = dc.GenerateOAuth(new List<DiscordOAuth.Scope>() { DiscordOAuth.Scope.identify})

//Example of how to get the authorization data
static void OAuth(object sender, DiscordOAuth.OAuthData Data)
{
    Console.WriteLine($"State: {Data.State}");
    Console.WriteLine($"DateTime: {Data.DateTime}");
    Console.WriteLine($"Code: {Data.Code}");
    Console.WriteLine($"Error: {Data.Error}");
    Console.WriteLine($"Error Description: {Data.ErrorDescription}");

    //Check if Authorization worked. 
    if(Data.Error == null)
    {
        //Gets Data which contains the Bearer Token
        var BearerData = dc.ExchangeCodeForBearer(Data.Code);

        //Gets Data of The Authorization
        var AuthorizationData = dc.GetCurrentAuthorizationInformation(BearerData.AccessToken);
        Console.WriteLine($"Username: {AuthorizationData.username}");
    }
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

