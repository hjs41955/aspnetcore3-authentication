using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ApiOne.Controllers
{
    public class SecretController : Controller
    {
        [Route("/secret")]
        [Authorize] //user or caller who is authenticated with open-id token can call this method
        public string Index()
        {
            var claims = User.Claims.ToList();
            return "secret message from ApiOne";
        }
    }
}
