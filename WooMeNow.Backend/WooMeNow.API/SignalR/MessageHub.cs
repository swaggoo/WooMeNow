﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WooMeNow.API.Data.UnitOfWork;
using WooMeNow.API.Entities;
using WooMeNow.API.Extensions;
using WooMeNow.API.Models;
using WooMeNow.API.Models.DTOs;

namespace WooMeNow.API.SignalR;

[Authorize]
public class MessageHub : Hub
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IHubContext<PresenceHub> _presenceHub;

    public MessageHub(
        IUnitOfWork uow,
        IMapper mapper,
        IHubContext<PresenceHub> presenceHub)
    {
        _uow = uow;
        _mapper = mapper;
        _presenceHub = presenceHub;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUsername = httpContext.Request.Query["user"];
        var groupName = GetGroupName(Context.User.GetUsername(), otherUsername);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddConnectionToGroupAsync(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await _uow.MessageRepository
            .GetMessageThreadAsync(Context.User.GetUsername(), otherUsername);

        if (_uow.HasChanges()) await _uow.CompleteAsync();

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var group = await RemoveConnectionFromGroupAsync();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User.GetUsername();

        if (username == createMessageDto.RecipientUsername)
            throw new HubException("You cannot send messages to yourself");

        var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) throw new HubException("Recipient is not found");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await _uow.MessageRepository.GetMessageGroupAsync(groupName);

        if(group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);

            if(connections != null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", 
                    new {username = sender.UserName, knownAs = sender.KnownAs });
            }
        }

        _uow.MessageRepository.AddMessage(message);

        if (await _uow.CompleteAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
        else
        {
            throw new HubException("Failed to send message");
        }
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<Group> AddConnectionToGroupAsync(string groupName)
    {
        var group = await _uow.MessageRepository.GetMessageGroupAsync(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        if (group == null)
        {
            group = new Group(groupName);
            _uow.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        if (await _uow.CompleteAsync()) return group;

        throw new HubException("Failed to add to group");
    }

    private async Task<Group> RemoveConnectionFromGroupAsync()
    {
        var group = await _uow.MessageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _uow.MessageRepository.RemoveConnection(connection);

        if (await _uow.CompleteAsync()) return group;

        throw new HubException("Failed to remove from group");
    }
}
