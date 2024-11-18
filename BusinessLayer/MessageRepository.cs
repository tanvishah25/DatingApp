using DatingApp.BusinessLayer.Interface;
using DatingApp.Data;
using DatingApp.Entities;
using DatingApp.Helpers;
using DatingApp.DTOs;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DatingApp.BusinessLayer
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext dataContext, IMapper mapper) 
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _dataContext.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _dataContext.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _dataContext.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _dataContext.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();
            var message = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username && x.RecipientDeleted == false),
                "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username && x.SenderDeleted == false),
                _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DateRead == null && x.RecipientDeleted == false)
            };

            var messages = message.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }
        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUsername)
        {
            var messages = await _dataContext.Messages
                                .Include(x => x.Sender).ThenInclude(x => x.Photos)
                                .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                                .Where(x => x.Recipient.UserName == recipientUsername 
                                                        && x.RecipientDeleted == false 
                                                        && x.Sender.UserName == currentUserName
                                       || x.Recipient.UserName == currentUserName 
                                                        && x.SenderDeleted == false 
                                                        && x.Sender.UserName == recipientUsername)
                                .OrderBy(x => x.MessageSent).ToListAsync();
            var unreadMessages = messages.Where(x=>x.DateRead == null).ToList();
            if(unreadMessages.Count != 0)
            {
                unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
                await _dataContext.SaveChangesAsync();
            }
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }
        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
