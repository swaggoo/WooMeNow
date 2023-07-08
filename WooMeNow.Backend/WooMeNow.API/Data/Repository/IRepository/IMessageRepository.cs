using System.Collections;
using WooMeNow.API.Entities;
using WooMeNow.API.Helpers;
using WooMeNow.API.Models;
using WooMeNow.API.Models.DTOs;

namespace WooMeNow.API.Data.Repository.IRepository;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessageAsync(int id);
    Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUserName, string recipientUserName);
    Task<bool> SaveAllAsync();
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection> GetConnectionAsync(string connectionId);
    Task<Group> GetMessageGroupAsync(string groupname);
    Task<Group> GetGroupForConnection(string connectionId);
}

