using System.Diagnostics;
using System.Reflection;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.Base;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Handlers;

public class ModalSubmissionHandler : IPamelloService
{
    private readonly IServiceProvider _services;
    
    private readonly DiscordClientService _clients;
    
    private List<Type> _modalsTypes;
    
    public ModalSubmissionHandler(IServiceProvider services) {
        _services = services;
        
        _clients = services.GetRequiredService<DiscordClientService>();
        
        _modalsTypes = [];
    }

    public void Load() {
        var typeResolver = _services.GetRequiredService<IAssemblyTypeResolver>();

        _modalsTypes = typeResolver.GetInheritorsOf<DiscordModal>().ToList();
        
        _clients.Main.ModalSubmitted += OnModalSubmitted;
    }

    private async Task OnModalSubmitted(SocketModal socketModal) {
        var fullName = socketModal.Data.CustomId;
        var splitAt = fullName.IndexOf(':');
        var submittedName = fullName[..splitAt];
        var submittedArgs = fullName[(splitAt + 1)..];

        Console.WriteLine($"Modal \"{socketModal.Data.CustomId}\" submitted: {submittedName} | {submittedArgs}");

        Type? modalType = null;
        MethodInfo? submissionMethod = null;
        
        foreach (var type in _modalsTypes) {
            var methods = type.GetMethods();
            foreach (var method in methods) {
                if (method.GetCustomAttribute<ModalSubmissionAttribute>() is not { } modalSubmissionAttribute) continue;
                if (modalSubmissionAttribute.Name != submittedName) continue;
                
                submissionMethod = method;
                modalType = type;
                
                break;
            }
            
            if (modalType is not null) break;
        }
        
        if (modalType is null || submissionMethod is null) return;
        
        var modalField = modalType.GetField("Modal");
        var servicesField = modalType.GetField("Services");
        if (modalField is null || servicesField is null) throw new PamelloException($"Modal {modalType.FullName} should have both Modal and Services fields");

        var modal = Activator.CreateInstance(modalType) as DiscordModal;
        Debug.Assert(modal is not null, "Command cannot be null here");
        
        modalField.SetValue(modal, socketModal);
        servicesField.SetValue(modal, _services);

        try {
            await (Task)submissionMethod.Invoke(modal, [submittedArgs])!;
        }
        catch (Exception e) {
            Console.WriteLine(e);
            await modal.Modal.DeferAsync();
        }
    }
}
