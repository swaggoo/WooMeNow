﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WooMeNow.API.Helpers;
using WooMeNow.API.Models;
using WooMeNow.API.Models.DTOs;

namespace WooMeNow.API.Data.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<MemberDto> GetMemberAsync(string username)
    {
        return await _db.Users
            .Where(u => u.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var query = _db.Users.AsQueryable();

        query = query.Where(u => u.UserName != userParams.CurrentUsername);
        query = query.Where(u => u.Gender == userParams.Gender);

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(u => u.DateOfBirth <= maxDob &&  u.DateOfBirth >= minDob);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActive)
        };


        return await PagedList<MemberDto>.CreateAsync(
            query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider), 
            userParams.PageNumber, 
            userParams.PageSize);
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _db.Users
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(user => user.UserName == username);
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _db.Users
            .Include(p => p.Photos)
            .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }

    public void Update(User user)
    {
        _db.Entry(user).State = EntityState.Modified;
    }
}
