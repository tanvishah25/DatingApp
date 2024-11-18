﻿using DatingApp.Entities;
using DatingApp.Helpers;
using DatingApp.DTOs;

namespace DatingApp.BusinessLayer.Interface
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUsername);
        Task<bool> SaveAllAsync();
    }
}