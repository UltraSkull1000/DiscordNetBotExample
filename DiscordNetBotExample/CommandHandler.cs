using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Modules;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace DiscordBot;
public class CommandHandler
{
    private DiscordSocketClient? _client;
    private InteractionService _interactionService;
    private IServiceProvider _serviceProvider;
    
    public CommandHandler(DiscordSocketClient _client)
    {
        this._client = _client;
        _interactionService = new InteractionService(_client, new InteractionServiceConfig(){
            DefaultRunMode = Discord.Interactions.RunMode.Async,
            UseCompiledLambda = true
        });
        _serviceProvider = SetupServices();
        _client.Ready += OnReady;

        DiscordBot.Print("Initialized Command Handler!");
    }
    public async Task OnReady(){
        DiscordBot.Print($"Client is Ready, Preparing Services...");
        if(_client == null){
            throw new NullReferenceException(nameof(_client));
        }
        _client.Ready -= OnReady;
        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
        await _interactionService.RegisterCommandsGloballyAsync();
        _client.InteractionCreated += HandleInteraction;
        DiscordBot.Print("Finished Preparing Services.");
    }
    private async Task HandleInteraction(SocketInteraction interaction){
        var ctx = new SocketInteractionContext(_client, interaction);
        var result = await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
        if(!result.IsSuccess)
        {
            DiscordBot.Print(result.ErrorReason, ConsoleColor.Red);
        }
        else if(interaction.Type == InteractionType.ApplicationCommand)
            DiscordBot.Print($"{interaction.User.Username} >> Executed Application Command", ConsoleColor.Blue);
    }
    private IServiceProvider SetupServices()
    => new ServiceCollection()
    .AddSingleton(_interactionService)
    .AddSingleton(typeof(General))
    .BuildServiceProvider();
}