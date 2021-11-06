using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))] // all controller will be making use of this action filter cuse its inside thr base controller 
    [ApiController] //this class of type ApiCntroller
    [Route("api/[controller]")]//route for the api con and all are api cont start with api/
    
    public class BaseApiController : ControllerBase
    {
        
    }
}