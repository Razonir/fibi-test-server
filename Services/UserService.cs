using fibi_test_server.Models;
using MongoDB.Driver;

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace fibi_test_server.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserService(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration["Mongo"]);
            var mongoDataBase = mongoClient.GetDatabase("fibi-test-db");
            _userCollection = mongoDataBase.GetCollection<User>("Users");
        }

        // Get All
        public async Task<List<User>> GetAllUsers()
        {
            return await _userCollection.Find(_ => true).ToListAsync();
        }

        // Get User
        public async Task<User> GetUser(string id)
        {
            return await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        // Remove User
        public async Task<DeleteResult> RemoveUser(string id)
        {
            return await _userCollection.DeleteOneAsync(x => x.Id == id);
        }



        // Upadte User
        public async Task<ReplaceOneResult> UpdateUser(string id,User updatedUser)
        {
            return await _userCollection.ReplaceOneAsync(x => x.Id == id,updatedUser);
        }


        // Create New
        public async Task CreateNewUser(User newUser)
        {
            var existingUser = await _userCollection.Find(x => x.UserName == newUser.UserName).FirstOrDefaultAsync();

            if (existingUser == null)
            {
                newUser.Role = "User";
                await _userCollection.InsertOneAsync(newUser);
            }
        }



        // Login Authentication
        public async Task<User> Authentication(User userAuth)
        {
            return await _userCollection.Find(u => u.UserName == userAuth.UserName && u.PasswordHash == userAuth.PasswordHash).FirstOrDefaultAsync();
        }



        // Change User Role
        public async Task<UpdateResult> ChangeUserRole(string id, string newRole)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            var update = Builders<User>.Update.Set(x => x.Role, newRole);

            return await _userCollection.UpdateOneAsync(filter, update);
        }

    }

}
