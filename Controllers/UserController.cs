using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blogapi.Models;
using blogapi.Models.DTO;
using blogapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace blogapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        //inject the user service here and use it to handle user-related requests
        private readonly UserService _data;

        public UserController(UserService dataFromService)
        {
            _data = dataFromService;
        }

        //function to add our user type of CreateAccountDTO call UserToAdd this will return bool once our user is added.
        //Add user
        [HttpPost("AddUser")]
        public bool AddUser(CreateAccountDTO UserToAdd)
        {
            return _data.AddUser(UserToAdd);
        }

        //GetAllUsers
        [HttpGet("GetAllUsers")]
        
        public IEnumerable<UserModel> GetAllUsers()
        {
            return _data.GetAllUsers();
        }


        //GetUserByUsername
        [HttpGet("GetUserByUserName")]
        public UserIdDTO GetUserDTOUserName(string username)
        {
            return _data.GetUserDTOUserName(username);
        }

        //login endpoint
        [HttpPost("Login")]

        public IActionResult Login([FromBody] LoginDTO user)
        {
            return _data.Login(user);
        }
    }
}