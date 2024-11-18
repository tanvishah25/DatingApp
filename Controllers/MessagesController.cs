using DatingApp.BusinessLayer.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using DatingApp.Helpers;

namespace DatingApp.Controllers
{
    public class MessagesController :BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public MessagesController(IMessageRepository messageRepository, IUserRepository userRepository,
                    IMapper mapper) {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();

            if (username == createMessageDto.RecipientUsername) return BadRequest("You cannot message yourself");

            var sender = await _userRepository.GetUserByNameAsync(username);
            var recipient = await _userRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);

            if (sender == null || recipient == null) return BadRequest("Cannot send message at this time");

            var Message = new Message()
            {
                Sender = sender,
                Recipient = recipient,
                Content = createMessageDto.Content,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName
            };

            _messageRepository.AddMessage(Message);
            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(Message));

            return BadRequest("Failed to save messages");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();
            var messages = await _messageRepository.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(messages);

            return Ok(messages);
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread(string username)
        {
            var currentUsername = User.GetUserName();
            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();
            var message = await _messageRepository.GetMessage(id);
            if (message == null) return BadRequest("Cannot delete this message");
            if(message.SenderUsername != username && message.RecipientUsername != username) 
                return Forbid();

            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if(message is { SenderDeleted: true,RecipientDeleted:true })
            {
                _messageRepository.DeleteMessage(message);
            }

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}
