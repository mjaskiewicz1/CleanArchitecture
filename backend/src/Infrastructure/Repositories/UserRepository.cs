using Domain.Entities;
using Domain.Repositories;

using Infrastructure.Database;
using Infrastructure.Repositories.Generic;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository;