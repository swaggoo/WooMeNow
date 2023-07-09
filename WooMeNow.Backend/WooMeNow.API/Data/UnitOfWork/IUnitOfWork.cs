using WooMeNow.API.Data.Repository;
using WooMeNow.API.Data.Repository.IRepository;

namespace WooMeNow.API.Data.UnitOfWork;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IMessageRepository MessageRepository { get; }
    ILikesRepository LikesRepository { get; }
    Task<bool> CompleteAsync();
    bool HasChanges();
}
