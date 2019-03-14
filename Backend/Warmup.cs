using Backend.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Backend
{
    public class Warmup
    {
        public static void DoWork(IConfiguration configuration,IApplicationBuilder application)
        {
            LocalJwt.SecretKey = new SymmetricSecurityKey(Byte16String.Decode(configuration["jwt:SecretKey"]));
            var dbCtx=application.ApplicationServices.GetService<SQLiteContext>();
            dbCtx.Database.EnsureCreated();
        }


    }
}
