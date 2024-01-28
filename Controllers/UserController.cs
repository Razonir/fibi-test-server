using fibi_test_server.Models;
using fibi_test_server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace fibi_test_server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _UserService;
        
        public UserController(UserService UserService)
        {
            _UserService = UserService;
        }

        //Get All
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            try
            {
                var list = await _UserService.GetAllUsers();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

       

        //Get User
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
  
             var user = await _UserService.GetUser(id);
            if( user is null)
            {
                return NotFound();
            }
            return user;
        }


        // Delete User
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _UserService.GetUser(id);
            if  (user is null)
            {
                return NotFound();
            }
            await _UserService.RemoveUser(id);

            return NoContent();

        }

        // Update user
        [HttpPut]
        public async Task<IActionResult> UpadteUser(User updatedUser)
        {
            var user = await _UserService.GetUser(updatedUser.Id);
            if(user is null)
            {
                return NotFound();
            }
            await _UserService.UpdateUser(updatedUser.Id, updatedUser);
            return NoContent();
        }


        // Create New
        [HttpPost]
        public async Task<IActionResult> CreateNewUser(User newUser)
        {
            await _UserService.CreateNewUser(newUser);
               
            return CreatedAtAction(nameof(GetAllUsers), new { id = newUser }, newUser);
        }

        // Login User
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(User userAuth)
        {
            var user = await _UserService.Authentication(userAuth);

            if (user is null)
            {
                return Unauthorized("סיסמא או שם משתמש שגויים");
            }
            return Ok(user);

        }


        // Change User Role
        [HttpPut("{id}/changerole")]
        public async Task<IActionResult> ChangeUserRole(string id, string role)
        {
            try
            {
                var updateResult = await _UserService.ChangeUserRole(id, role);

                if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                {
                    return Ok("User role updated successfully.");
                }
                else
                {
                    return NotFound("User not found or role update failed.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }



    }
}
