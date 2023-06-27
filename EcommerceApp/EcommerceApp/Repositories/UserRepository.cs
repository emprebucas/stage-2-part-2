using Dapper;
using EcommerceApp.Data;
using EcommerceApp.Interfaces;
using EcommerceApp.Entities;
using EcommerceApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using SendGrid.Helpers.Errors.Model;

namespace EcommerceApp.Repositories
{
    /// <summary>
    /// UserRepository provides the implementation for retrieving and adding users.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ECommerceDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserRepository> _logger;

        /// <summary>
        /// The constructor takes the `ECommerceDbContext` and `IConfiguration` objects.
        /// `ECommerceDbContext`: used to interact with the database using Entity Framework Core.
        /// `IConfiguration`: used to retrieve the database connection string.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public UserRepository(ECommerceDbContext dbContext, IConfiguration configuration, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// GetUserByIdAsync retrieves a user by their ID from the database. 
        /// It uses Dapper to execute the SQL query and returns the retrieved user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task<UserModel> GetUserByIdAsync(Guid userId)
        {
            var connectionString = _configuration.GetConnectionString("ECommerceDb");
            using var connection = new MySqlConnection(connectionString);

            var query = "SELECT * FROM Users WHERE UserId = @UserId";
            var users = await connection.QueryAsync<UserModel>(query, new { UserId = userId });
            var user = users.FirstOrDefault();

            if (user == null)
            {
                _logger.LogError("Error retrieving user.");
                throw new BadRequestException("User does not exist.");
            }

            _logger.LogInformation("User retrieved successfully.");
            return user;
        }

        /// <summary>
        /// AddUserAsync uses Entity Framework Core to add a new user to the database.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public async Task AddUserAsync(UserEntity user)
        {
            var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == user.UserId);

            if (userExists)
            {
                _logger.LogError("Error adding user.");
                throw new BadRequestException("Cannot add user. User already exists.");
            }

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("User added successfully.");
        }
    }

}
