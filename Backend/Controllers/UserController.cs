using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Model.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SQLiteContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IConfiguration _configuration;

        public UserController(SQLiteContext context, ILogger<UserController> logger, IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _connectionMultiplexer = connectionMultiplexer;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public IActionResult Verify([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(model.PWD) || string.IsNullOrWhiteSpace(model.PhoneNumber))
                return BadRequest();

            IQueryable<mUser> query = _context.sUser.Where(x => x.PhoneNumber == model.PhoneNumber).Where(x => x.Password == model.PWD).Take(1);
            var result = query.ToList();
            if (result.Count == 0) return NotFound();

            return Ok(new { Jwt = LocalJwt.Regist(_connectionMultiplexer.GetDatabase(), result[0].PhoneNumber.ToString(), _configuration["jwt:Issuer"], int.Parse(_configuration["jwt:Overtime"])) });
        }

        [HttpPost("Regist")]
        public async Task<IActionResult> RegistAsync([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(model.PWD) || string.IsNullOrWhiteSpace(model.PhoneNumber))
                return BadRequest();

            IQueryable<mUser> query = _context.sUser.Where(x => x.PhoneNumber == model.PhoneNumber).Take(1);
            if (query.Count() > 0) return BadRequest();


            var user = new mUser()
            {
                Password = model.PWD,
                PhoneNumber = model.PhoneNumber
            };
            _context.sUser.Attach(user);
            await _context.SaveChangesAsync();
            return Ok(new { Jwt = LocalJwt.Regist(_connectionMultiplexer.GetDatabase(), user.ID.ToString(), _configuration["jwt:Issuer"], int.Parse(_configuration["jwt:Overtime"])) });
        }

        [Authorize]
        [HttpPost("GetDuties")]
        public List<mDuty> GetDuties() => _context.sDuty.ToList();

        [Authorize]
        [HttpPost("AddDuty")]
        public IActionResult AddDuty([FromBody]DutyModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int parentid = 0;
            var dbModel = new mDuty()
            {
                Name = model.Name,
                Role = model.Role,
                Parent = int.TryParse(model.ParentId, out parentid) ? _context.sDuty.Find(parentid) : null
            };

            return Ok();
        }

        public class LoginModel
        {
            public string PWD { get; set; }

            public string PhoneNumber { get; set; }
        }

        public class DutyModel
        {
            public string Name { get; set; }

            public string Role { get; set; }

            public string ParentId { get; set; }
        }


    }
}