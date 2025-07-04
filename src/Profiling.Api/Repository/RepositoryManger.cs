using System;
using LoggerService;
using Profiling.Api.Context;
using Profiling.Api.Contracts;

namespace Profiling.Api.Repository;

public class RepositoryManger : IRepositoryManager
{
    private readonly Lazy<IModuleRepository> _moduleRepository;
    private readonly Lazy<IResourceRepository> _resourceRepository;
    private readonly Lazy<IUserRepository> _userRepository;

    protected DataContext _context;
    private ILoggerManager _loggerManager;
    private readonly IHttpContextAccessor _httpContext;

    public RepositoryManger(DataContext context, ILoggerManager loggerManager, IHttpContextAccessor httpContext)
    {
        _context = context;
        _loggerManager = loggerManager;
        _httpContext = httpContext;
        _moduleRepository = new Lazy<IModuleRepository>(() => new ModuleRepository(_context, _loggerManager));
        _resourceRepository = new Lazy<IResourceRepository>(() => new ResourceRepository(_context, _loggerManager, _httpContext));
        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(_context, _loggerManager));
    }

    public IModuleRepository ModuleRepository => _moduleRepository.Value;

    public IResourceRepository ResourceRepository => _resourceRepository.Value;

    public IUserRepository UserRepository => _userRepository.Value;
}
