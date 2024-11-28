using DatingApp.BusinessLayer.Interface;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Entities;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using System;

namespace DatingApp.SignalR
{
    public class MessageHub : Hub
    {
        IMessageRepository _messageRepository;
        IUserRepository _userRepository;
        IMapper _mapper;
        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request.Query["username"];
            if (Context.User == null || string.IsNullOrEmpty(otherUser))
                throw new HubException("Cannot join group");
            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
            //Without adding users to groups, SignalR would be forced to send messages to all connected clients (using Clients.All.SendAsync())
            //which is not efficient and doesn't allow targeted communication.
            //When a user is added to a group, messages can be sent to just those in the group(using Clients.Group(groupName).SendAsync()).
            //This is useful when you only want certain users to receive messages—for example,
            //in a private chat between two users or in a team chat within a collaborative app.
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);
            var messages = await _messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser!);
            await Clients.Group(groupName).SendAsync("ReceivedMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User?.GetUserName() ?? throw new Exception("could not get user");

            if (username == createMessageDto.RecipientUsername) throw new HubException("You cannot message yourself");

            var sender = await _userRepository.GetUserByNameAsync(username);
            var recipient = await _userRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);

            if (sender == null || recipient == null) throw new HubException("Cannot send message at this time");

            var message = new Message()
            {
                Sender = sender,
                Recipient = recipient,
                Content = createMessageDto.Content,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _messageRepository.GetMessageGroup(groupName);

            if(group != null && group.Connections.Any(x=>x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }

            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }

        }

        private async Task<bool> AddToGroup(string groupName)
        {
            var username = Context.User?.GetUserName() ?? throw new Exception("Cannot get username");
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connections = new Connection { ConnectionId = Context.ConnectionId, Username = username };
            if (group == null)
            {
                group = new Group { Groupname = groupName };
                _messageRepository.AddGroup(group);
            }
            group.Connections.Add(connections);
            return await _messageRepository.SaveAllAsync();
        }

        private async Task RemoveFromMessageGroup()
        {
            var connection = await _messageRepository.GetConnection(Context.ConnectionId);
            if (connection != null)
            {
                _messageRepository.RemoveConnection(connection);
                await _messageRepository.SaveAllAsync();
            }
        }

        private string GetGroupName(string caller, string? receiver)
        {
            var stringCompare = string.CompareOrdinal(caller, receiver) < 0;
            return stringCompare ? $"{caller} - {receiver}" : $"{receiver} - {caller}";
        }
    }
}
