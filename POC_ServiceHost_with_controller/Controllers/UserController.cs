using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POC_Services.DataTransferObjects;
using System.Linq;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace POC_Services.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserController()
        {
        }
        // GET: api/<User>
        [HttpGet]
        public User Get()
        {
            var user = new User();

            var claims = User.Identities.SelectMany(x => x.Claims) as IEnumerable<Claim>;

            user.FullName = claims.FirstOrDefault(x => x.Type == "name").Value;
            user.ProfilePic = claims.FirstOrDefault(x => x.Type == "picture").Value;
            user.Email = claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            return user;
        }

        // GET api/<User>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<User>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<User>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<User>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
