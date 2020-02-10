using System;
using System.Collections.Generic;
using System.Linq;
using DattingApp.API.Models;
using Newtonsoft.Json;

namespace DattingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context) {
            if (!context.Users.Any()) {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach(var user in users) {
                    byte[] passwordhash, passwordsalt;
                    CreatePasswordHash("password", out passwordhash, out passwordsalt);
                    user.PasswordHash = passwordhash;
                    user.PasswordSalt = passwordsalt;
                    user.Username = user.Username.ToLower();

                    context.Users.Add(user); 
                }

                context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordhash, out byte[] passwordsalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordsalt = hmac.Key;
                passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}