using MieTanakaLocalTheaterCompanyV2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;

/// <summary>
/// Name:Mie Tanaka
/// Name:02/03/2017
/// Description: allows user edit their own detail


namespace MieTanakaLocalTheaterCompanyV2.Controllers
{

    public class UserController : Controller
    {
             private ApplicationUserManager _userManager;
        // GET: User
        // GET: /User/Edit/TestUser 
        /// <summary>
        /// returns a ediable EditUser View filled with logged in user detail
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns> <objExpandedUserDTO </returns>
        #region public ActionResult EditUser(string UserName)
        public ActionResult EditUser(string UserName)
             {
                 if (UserName == null)
                 {
                     return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                 }
                 ExpandedUserDTO objExpandedUserDTO = GetUser(UserName);
                 if (objExpandedUserDTO == null)
                 {
                     return HttpNotFound();
                 }
                 return View(objExpandedUserDTO);
             }
        #endregion

        /// PUT: /User/EditUser
        /// <summary> 
        /// returns Home/Index after updateing database with ExpandedUserDTO data
        /// if error keep returning EditUser View
        /// </summary>
        /// <param name="paramExpandedUserDTO"></param>
        /// <returns></returns>
        [HttpPost]
             [ValidateAntiForgeryToken]
             #region public ActionResult EditUser(ExpandedUserDTO paramExpandedUserDTO)
             public ActionResult EditUser(ExpandedUserDTO paramExpandedUserDTO)
             {
                 try
                 {
                     if (paramExpandedUserDTO == null)
                     {
                         return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                     }

                     ExpandedUserDTO objExpandedUserDTO = UpdateDTOUser(paramExpandedUserDTO);

                     if (objExpandedUserDTO == null)
                     {
                         return HttpNotFound();
                     }

                     return Redirect("~/Home/Index");
                 }
                 catch (Exception ex) //if there is an error, return back to EditUser with original input value
                 {
                     ModelState.AddModelError(string.Empty, "Error: " + ex);
                     return View("EditUser", GetUser(paramExpandedUserDTO.UserName));
                 }
             }
             #endregion


        /// <summary>
        /// getter and setter of userManager
        /// </summary>
             #region public ApplicationUserManager UserManager
             public ApplicationUserManager UserManager
             {
                 get
                 {
                     return _userManager ??
                         HttpContext.GetOwinContext()
                         .GetUserManager<ApplicationUserManager>();
                 }
                 private set
                 {
                     _userManager = value;
                 }
             }
        #endregion

        // DELETE: /User/DeleteUser
        // DELETE: /Admin/DeleteUser
        /// <summary>
        /// Gets selected user data and display pop up view with confirm delete,
        /// if confirm yes, delete the user record from database and returns Admin/Index view 
        /// </summary>
        /// <param name="string UserName"></param>
        /// <returns>returns Admin/Index view if successful , 
        ///         if error returns current EditUser view with error message. </returns>
        #region public ActionResult DeleteUser(string UserName)
        public ActionResult DeleteUser(string UserName)
             {
                 try
                 {
                     if (UserName == null)
                     {
                         return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                     }

                     if (UserName.ToLower() == this.User.Identity.Name.ToLower())
                     {
                         ModelState.AddModelError(
                             string.Empty, "Error: Cannot delete the current user");

                         return View("EditUser");
                     }

                     ExpandedUserDTO objExpandedUserDTO = GetUser(UserName);

                     if (objExpandedUserDTO == null)
                     {
                         return HttpNotFound();
                     }
                     else
                     {
                         DeleteUser(objExpandedUserDTO);
                     }

                     return Redirect("~/Home/Index");
                 }
                 catch (Exception ex)
                 {
                     ModelState.AddModelError(string.Empty, "Error: " + ex);
                     return View("EditUser", GetUser(UserName));
                 }
             }
        #endregion

        /// <summary>
        /// gets the logged in user's detail from database and 
        /// returns ExpandedUserDTO objExpandedUserDTO from database.
        /// <param name="paramUserName"></param>
        /// <returns>ExpandedUserDTO objExpandedUserDTO</returns>     
        #region private ExpandedUserDTO GetUser(string paramUserName)
        private ExpandedUserDTO GetUser(string paramUserName)
             {
                 ExpandedUserDTO objExpandedUserDTO = new ExpandedUserDTO();
            //get data from database by finding it by the logged in username
            var result = UserManager.FindByName(paramUserName);

                 // If we could not find the user, throw an exception
                 if (result == null) throw new Exception("Could not find the User");
                 objExpandedUserDTO.Forename = result.Forename;
                 objExpandedUserDTO.Surname = result.Surname;
                 objExpandedUserDTO.Street = result.Street;
                 objExpandedUserDTO.Town = result.Town;
                 objExpandedUserDTO.Postcode = result.Postcode;
                 objExpandedUserDTO.PhoneNumber = result.PhoneNumber;
                 objExpandedUserDTO.UserName = result.UserName;
                 objExpandedUserDTO.Email = result.UserName;



            return objExpandedUserDTO;
             }
        #endregion

        /// <summary>
        /// returns the updated ExpandedUserDTO class object paramExpandedUserDTO 
        /// after updating database user detail with input data
        /// <param name="objExpandedUserDTO"></param>
        /// <returns>ExpandedUserDTO paramExpandedUserDTO</returns>
        #region private ExpandedUserDTO UpdateDTOUser(ExpandedUserDTO objExpandedUserDTO)
        private ExpandedUserDTO UpdateDTOUser(ExpandedUserDTO paramExpandedUserDTO)
             {
              //Get the input user data from pramExpandedUserDTO by finding it by UserName.
                 ApplicationUser result =
                     UserManager.FindByName(paramExpandedUserDTO.UserName);

                 // If we could not find the user, throw an exception
                 if (result == null)
                 {
                     throw new Exception("Could not find the User");
                 }
                 result.UserName = paramExpandedUserDTO.UserName;
                 result.Email = paramExpandedUserDTO.UserName;
                 result.Forename = paramExpandedUserDTO.Forename;
                 result.Surname = paramExpandedUserDTO.Surname;
                 result.Street = paramExpandedUserDTO.Street;
                 result.Town = paramExpandedUserDTO.Town;
                 result.Postcode = paramExpandedUserDTO.Postcode;
                 result.PhoneNumber = paramExpandedUserDTO.PhoneNumber;
                 




            // Lets check if the account needs to be unlocked
            if (UserManager.IsLockedOut(result.Id))
                 {
                     // Unlock user
                     UserManager.ResetAccessFailedCountAsync(result.Id);
                 }

                 UserManager.Update(result);

                 // Was a password sent across?
                 if (!string.IsNullOrEmpty(paramExpandedUserDTO.Password))
                 {
                     // Remove current password
                     var removePassword = UserManager.RemovePassword(result.Id);
                     if (removePassword.Succeeded)
                     {
                         // Add new password
                         var AddPassword =
                             UserManager.AddPassword(
                                 result.Id,
                                 paramExpandedUserDTO.Password
                                 );

                         if (AddPassword.Errors.Count() > 0)
                         {
                             throw new Exception(AddPassword.Errors.FirstOrDefault());
                         }
                     }
                 }

                 return paramExpandedUserDTO;
             }
        #endregion

        /// <summary>
        /// delete user from database
        /// <param name="paramExpandedUserDTT"></param>
        /// <returns>void</returns>
        #region private void DeleteUser(ExpandedUserDTO paramExpandedUserDTO)
        private void DeleteUser(ExpandedUserDTO paramExpandedUserDTO)
             {
                 ApplicationUser user =
                     UserManager.FindByName(paramExpandedUserDTO.UserName);

                 // If we could not find the user, throw an exception
                 if (user == null)
                 {
                     throw new Exception("Could not find the User");
                 }

                 UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());
                 UserManager.Update(user);
                 UserManager.Delete(user);
             }
             #endregion
             
    }
}