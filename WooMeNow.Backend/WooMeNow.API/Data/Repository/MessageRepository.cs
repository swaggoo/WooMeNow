using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WooMeNow.API.Data.Repository.IRepository;
using WooMeNow.API.Helpers;
using WooMeNow.API.Models;
using WooMeNow.API.Models.DTOs;

namespace WooMeNow.API.Data.Repository;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public MessageRepository(
        ApplicationDbContext db,
        IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public void AddMessage(Message message)
    {
        _db.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _db.Messages.Remove(message);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams)
    {
        var query = _db.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(m => m.RecipientUsername == messageParams.Username && 
                !m.RecipientDeleted),
            "Outbox" => query.Where(m => m.SenderUsername == messageParams.Username && 
                !m.SenderDeleted),
            _ => query.Where(m => m.RecipientUsername == messageParams.Username && 
                !m.RecipientDeleted && 
                m.DateRead == null)
        };

        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);


        return await PagedList<MessageDto>
            .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _db.Messages.FindAsync(id);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
    {
        var messages = await _db.Messages
            .Include(message => message.Sender).ThenInclude(user => user.Photos)
            .Include(message => message.Recipient).ThenInclude(user => user.Photos)
            .Where(message => message.SenderUsername == currentUserName &&
                        !message.RecipientDeleted &&    
                        message.RecipientUsername == recipientUserName ||
                        message.RecipientUsername == currentUserName &&
                        !message.SenderDeleted &&
                        message.SenderUsername == recipientUserName)
            .OrderBy(m => m.MessageSent)
            .ToListAsync();

        var unreadMessages = messages.Where(message => message.DateRead == null && 
            message.RecipientUsername == currentUserName).ToList();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }
}
