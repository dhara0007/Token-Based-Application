using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using TokenAuthenticationWEBAPI.Models;

namespace TokenAuthenticationWEBAPI.Controllers
{
    // [EnableCors("*", "*", "*")]
    public class UserController : ApiController
    {
        //This resource is For all types of role
        [Authorize]
        [HttpGet]
        [Route("api/user/GetDetails")]
        public IHttpActionResult GetDetails()
        {
            IEnumerable<User> identity;
            using (TokenAuthenticationDBEntities entities = new TokenAuthenticationDBEntities())
            {

                identity = entities.Users.ToList();
            }


            return Ok(identity.ToList());
        }
        [HttpGet]
        [Route("api/user/GetRoles")]
        public IHttpActionResult GetRoles()
        {
            IEnumerable<Role> identity;
            using (TokenAuthenticationDBEntities entities = new TokenAuthenticationDBEntities())
            {

                identity = entities.Roles.ToList();
            }


            return Ok(identity.ToList());
        }
        [HttpPost]
        [Route("api/user/SubmitDetails")]
        public IHttpActionResult SubmitDetails(UserMaster user)
        {
            if (!ModelState.IsValid || user.UserName == null || user.UserPassword == null || user.UserRoleID == null || user.UserEmail == null)
                return BadRequest("Invalid data.");

            using (var ctx = new TokenAuthenticationDBEntities())
            {
                ctx.UserMasters.Add(new UserMaster()
                {
                    UserName = user.UserName,
                    UserPassword = user.UserPassword,
                    UserRoleID = user.UserRoleID,
                    UserEmail = user.UserEmail
                });

                ctx.SaveChanges();
            }

            return Ok();
        }
        [Authorize]
        [HttpGet]
        [Route("api/user/GetUserById/{id}")]
        
            public IHttpActionResult GetUserById(int id)
            {
                User user = null;

                using (var ctx = new TokenAuthenticationDBEntities())
                {
                user = ctx.Users.Where(s => s.ID == id).FirstOrDefault<User>();
                    //.Select(s => new User()
                    //{
                    //    ID = s.ID,
                    //    FirstName = s.FirstName,
                    //    LastName = s.LastName,
                    //    Gender = s.Gender
                    //}).FirstOrDefault<User>();
            }

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
        [Authorize]
        [HttpPut]
        [Route("api/user/EditDetail")]
        public IHttpActionResult EditDetail(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            using (var ctx = new TokenAuthenticationDBEntities())
            {
                var existingUser = ctx.Users.Where(s => s.ID == user.ID)
                                                        .FirstOrDefault<User>();

                if (existingUser != null)
                {
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.GenderID = user.GenderID;

                    ctx.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok();
        }
        [Authorize]
        [HttpDelete]
        [Route("api/user/DeleteDetail/{id}")]
        public IHttpActionResult DeleteDetail(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid id");

            using (var ctx = new TokenAuthenticationDBEntities())
            {
                var user = ctx.Users
                    .Where(s => s.ID == id)
                    .FirstOrDefault();

                ctx.Entry(user).State = System.Data.Entity.EntityState.Deleted;


                var userMaster = ctx.UserMasters
                    .Where(s => s.UserID == id)
                    .FirstOrDefault();

                ctx.Entry(userMaster).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }

            return Ok();
        }
    }
}

    

    //This resource is only For Admin and SuperAdmin role
    //[Authorize(Roles = "SuperAdmin, Admin")]
    //[HttpGet]
    //[Route("api/test/resource2")]
    //public IHttpActionResult GetResource2()
    //{
    //    var identity = (ClaimsIdentity)User.Identity;
    //    var Email = identity.Claims
    //              .FirstOrDefault(c => c.Type == "Email").Value;
    //    var UserName = identity.Name;

    //    return Ok("Hello " + UserName + ", Your Email ID is :" + Email);
    //}
    ////This resource is only For SuperAdmin role
    //[Authorize(Roles = "SuperAdmin")]
    //[HttpGet]
    //[Route("api/test/resource3")]
    //public IHttpActionResult GetResource3()
    //{
    //    var identity = (ClaimsIdentity)User.Identity;
    //    var roles = identity.Claims
    //                .Where(c => c.Type == ClaimTypes.Role)
    //                .Select(c => c.Value);
    //    return Ok("Hello " + identity.Name + "Your Role(s) are: " + string.Join(",", roles.ToList()));
    //}


