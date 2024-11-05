using Discord.Interactions;

namespace DiscordBot.Modules;

public class General() : InteractionModuleBase
{
    [SlashCommand("uptime", "Checks the uptime of the current shard")]
    public async Task Uptime()
    {
        await RespondAsync($"The current shard has been up for {(DateTime.Now - DiscordBot.startTime).ToString()}.", ephemeral: true);
    }
    
    [SlashCommand("ping", "Checks the ping to the current shard")]
    public async Task Ping()
    {
        await RespondAsync($"*Pong! {(DateTime.Now - Context.Interaction.CreatedAt).ToString("fff")}ms*", ephemeral: true);
    }

}