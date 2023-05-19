using Interfaces;
using Entities;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
        private RepositoryContext _repoContext;
        
        private ILoggerManager _logger;
        
        public ILoggerManager logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new LoggerService.LoggerManager();
                }
                return _logger;
            }
        }

       
    }
}
