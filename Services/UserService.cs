using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using blogapi.Models;
using blogapi.Models.DTO;
using blogapi.Services.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace blogapi.Services
{
    public class UserService : ControllerBase
    {
    private readonly DataContext _context;
        public UserService(DataContext context)
        {
            _context = context;
        }

        // We need a helper method to check if user exists in our database
public bool DoesUserExist(string username)
    {
        // Check our tables to see if the user exists
        return _context.UserInfo.SingleOrDefault(user => user.Username == username) != null;
    }
    public bool AddUser(CreateAccountDTO userToAdd)
    {
      bool result = false;

    if(userToAdd.Username != null && !DoesUserExist(userToAdd.Username))
            {
                UserModel newUser = new UserModel();

               var hashedPassword = HashPassword(userToAdd.Password);
               newUser.Id = userToAdd.Id;
               newUser.Username = userToAdd.Username;
               newUser.Salt = hashedPassword.Salt;
               newUser.Hash = hashedPassword.Hash;

               _context.Add(newUser);
               result = _context.SaveChanges() != 0;
               
            }
            return result;

// we are going to need a Hash helper function help us hash our password 
// we need to set our newUser.Id = UserToAdd.Id
// Username 
// Salt
// Hash

// then we add it to our DataContext 
// save our changes
// return a bool to return true or false 
    }

    //function that will help hash our password
    public PasswordDTO HashPassword(string? password)
        {
            PasswordDTO newHashedPassword = new PasswordDTO();

            byte[] SaltBytes = new byte[64];

            var provider = RandomNumberGenerator.Create();
            provider.GetNonZeroBytes(SaltBytes);
            var Salt = Convert.ToBase64String(SaltBytes);

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password ?? "", SaltBytes, 10000, HashAlgorithmName.SHA512);

            var Hash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            newHashedPassword.Salt = Salt;
            newHashedPassword.Hash = Hash;
            return newHashedPassword;
        }

        //Helper function to verify password
        public bool VerifyUserPassword(string? Password, string? StoredHash, string? StoredSalt)
        {
            if(StoredSalt == null)
            {
                return false;
            }

            var SaltBytes = Convert.FromBase64String(StoredSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(Password ?? "", SaltBytes, 10000, HashAlgorithmName.SHA256);
            var newHash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
            return newHash == StoredHash;
        }

        public IEnumerable<UserModel> GetAllUsers()
        {
            return _context.UserInfo;
        }

        public IActionResult Login(LoginDTO user)
        {
                IActionResult result = Unauthorized();
            //check if user exists
            if (DoesUserExist(user.Username))
            {
                //create a secrete key used to sign the jwt token
                //this key should be stored securely (not hardcoded in production)
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersupersupersuperdupersecretkey@34456789"));
                //create signing credentials using the secret key and HMACSHA256 algorithm
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256); //this ensures the token cant be tampered with.

                //build the JWT token with metadata

                var tokeOptions = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signingCredentials
                );

                //Convert the token object into string that can be sent to the client
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                //return the token as JSON to client
                result = Ok(new { Token = tokenString });
            }
            //return either the token (if user exists) or unauthorized (if user does not exist)
            return result;
        }

        public UserIdDTO GetUserDTOUserName(string username)
        {
            throw new NotImplementedException();
        }
    }
    }
