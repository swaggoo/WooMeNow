﻿using AutoMapper;
using WooMeNow.API.Data.Repository;
using WooMeNow.API.Data.Repository.IRepository;

namespace WooMeNow.API.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UnitOfWork(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IUserRepository UserRepository => new UserRepository(_context, _mapper);

    public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

    public ILikesRepository LikesRepository => new LikesRepository(_context);

    public async Task<bool> CompleteAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}
