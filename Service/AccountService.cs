﻿using Model;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Service.Helpers;

namespace Service
{

    public interface IAccountService
    {
        Account Authenticate(string username, string password);
        IEnumerable<Account> GetAll();
    }

    public class AccountService : IAccountService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<Account> _users = new List<Account>
        {
            new Account { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
        };

        //public readonly AccountDbcontext _accountDbContext;
        //public AccountService(AccountDbcontext accountDbContext)
        //{
        //    _accountDbContext = accountDbContext;
        //}

        private readonly AppSettings _appSettings;

        public AccountService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

       
        //Authentication 
        public Account Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Password = null;

            return user;
        }

        //GetAllAccount
        public IEnumerable<Account> GetAll()
        {
            // return users without passwords
            return _users.ToList();
        }


        
    }
}

