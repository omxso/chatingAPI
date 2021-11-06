using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository {get;}
        IMessageRepository MessageRepository {get;}
        ILikesRepository LikesRepository {get;}
        IPhotoRepository PhotoRepository {get;}
        Task<bool> Complete(); // the message to save all changes
        bool HasChanges(); // a helper method to see if EF has tracking or any changes
    }
}