using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork // creating istance of the Repo and pass it what it has in its constructer
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public IUserRepository UserRepository => new UserRepository(_context,_mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_context);
        public IPhotoRepository PhotoRepository => new PhotoRepository(_context);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0; // make sure we have changes when saving
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges(); // any thing EF tracking if it has something then it will return true
        }
    }
}