using MieTanakaLocalTheaterCompanyV2.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using static MieTanakaLocalTheaterCompanyV2.ApplicationSignInManager;
/// <summary>
/// Name:Mie Tanaka
/// Name:02/03/2017
/// Description: allows admin edit all users detail and roles:

/// private RolesUserIsNotIn(string userName0
///         returns role names that has no user allocated to it as a list
/// </summary>
namespace MieTanakaLocalTheaterCompanyV2.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        // Controllers


        /// <summary>
        /// list of username of all users, 
        /// a user can be filtered out by inputing the part of user name in serch string text box
        /// it is put in page list
        /// </summary>
        /// <param name="searchStringUserNameOrEmail"></param>
        /// <param name="currentFilter"></param>
        /// <param name="page"></param>
        /// <returns>returns the list of username of all users with edit delete link</returns>
        [Authorize(Roles = "Administrator")]
        #region public ActionResult Index(string searchStringUserNameOrEmail)       
        public ActionResult Index(string searchStringUserNameOrEmail, string currentFilter, int? page)
        {
            try
            {
                int intPage = 1;//set initial page number
                int intPageSize = 5; // set max record each page can have
                int intTotalPageCount = 0;//set initial page count to 0

                if (searchStringUserNameOrEmail != null)
                {
                    intPage = 1;
                }
                else
                {
                    if (currentFilter != null)
                    {
                        searchStringUserNameOrEmail = currentFilter;
                        intPage = page ?? 1;
                    }
                    else
                    {
                        searchStringUserNameOrEmail = "";
                        intPage = page ?? 1;
                    }
                }

                ViewBag.CurrentFilter = searchStringUserNameOrEmail;

                List<ExpandedUserDTO> col_UserDTO = new List<ExpandedUserDTO>();
                int intSkip = (intPage - 1) * intPageSize;

                intTotalPageCount = UserManager.Users
                    .Where(x => x.UserName.Contains(searchStringUserNameOrEmail))
                    .Count();

                var result = UserManager.Users
                    .Where(x => x.UserName.Contains(searchStringUserNameOrEmail))
                    .OrderBy(x => x.UserName)
                    .Skip(intSkip)
                    .Take(intPageSize)
                    .ToList();

                foreach (var item in result)
                {
                    ExpandedUserDTO objUserDTO = new ExpandedUserDTO();

                    objUserDTO.UserName = item.UserName;
                    objUserDTO.Email = item.Email;
                    objUserDTO.LockoutEndDateUtc = item.LockoutEndDateUtc;

                    col_UserDTO.Add(objUserDTO);
                }

                // Set the number of pages
                var _UserDTOAsIPagedList =
                    new StaticPagedList<ExpandedUserDTO>
                    (
                        col_UserDTO, intPage, intPageSize, intTotalPageCount
                        );

                return View(_UserDTOAsIPagedList);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                List<ExpandedUserDTO> col_UserDTO = new List<ExpandedUserDTO>();

                return View(col_UserDTO.ToPagedList(1, 25));
            }
        }
        #endregion

        // Users *****************************

        // GET: /Admin/Edit/Create
        /// <summary>
        /// returns a black Admin/Create view
        /// </summary>
        /// <returns>returns empty view filled with ExpandedUserDTO objExpandedUserDTO parameter</returns> 
        [Authorize(Roles = "Administrator")]
        #region public ActionResult Create()
        public ActionResult Create()
        {
            ExpandedUserDTO objExpandedUserDTO = new ExpandedUserDTO();

            ViewBag.Roles = GetAllRolesAsSelectList();

            return View(objExpandedUserDTO);
        }
        #endregion

        // PUT: /Admin/Create
        /// <summary>
        /// Gets input data and save it to database if successful, go to Admin/Index view
        /// Returns same screen with error message
        /// </summary>
        /// <param name="paramExpandedUserDTO"></param>
        /// <returns>returns Admin/Index View if input parameter successfully saved on to database 
        ///         returns create method with curren input data with error message, 
        ///         if input parameter failed to load on to database </returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult Create(ExpandedUserDTO paramExpandedUserDTO)
        public ActionResult Create(ExpandedUserDTO paramExpandedUserDTO)
        {
            try
            {
                if (paramExpandedUserDTO == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var Forename = paramExpandedUserDTO.Forename.Trim();
                var Surname = paramExpandedUserDTO.Surname.Trim();
                var Street = paramExpandedUserDTO.Street.Trim();
                var Town = paramExpandedUserDTO.Town.Trim();
                var Postcode = paramExpandedUserDTO.Postcode.Trim();
                var PhoneNumber = paramExpandedUserDTO.PhoneNumber.Trim();
                var Email = paramExpandedUserDTO.Email.Trim();
                var UserName = paramExpandedUserDTO.Email.Trim();
                var Password = paramExpandedUserDTO.Password.Trim();

                if (Email == "")
                {
                    throw new Exception("No Email");
                }

                if (Password == "")
                {
                    throw new Exception("No Password");
                }

                // set UserName as LowerCase of the Email
                UserName = Email.ToLower();

                // Create user

                var objNewAdminUser = new ApplicationUser
                {
                    UserName = UserName, Email = Email ,
                    Forename = Forename,
                    Surname = Surname,
                    Street = Street,
                    Town = Town,
                    Postcode = Postcode,
                    PhoneNumber = PhoneNumber
                };
                //Create user with objNewAdminUser with Password
                var AdminUserCreateResult = UserManager.Create(objNewAdminUser, Password);

                if (AdminUserCreateResult.Succeeded == true)
                {
                    string strNewRole = Convert.ToString(Request.Form["Roles"]);

                    if (strNewRole != "0")
                    {
                        // Put user in role
                        UserManager.AddToRole(objNewAdminUser.Id, strNewRole);
                    }

                    return Redirect("~/Admin");
                }
                else
                {
                    ViewBag.Roles = GetAllRolesAsSelectList();
                    ModelState.AddModelError(string.Empty,
                        "Error: Failed to create the user. Check password requirements.");
                    return View(paramExpandedUserDTO);
                }
            }
            // if exception occur return create method with error message
            catch (Exception ex)
            {
                ViewBag.Roles = GetAllRolesAsSelectList();
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("Create");
            }
        }
        #endregion

        // GET: /Admin/Edit/TestUser 
        /// <summary>
        /// Show editable Admin/Edit user view filled with selected user deatil, pass input to the next EditUserView
        /// </summary>
        /// <returns> Returns an editable EditUserView filled with selected user detail.</returns>
        [Authorize(Roles = "Administrator")]
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

        // PUT: /Admin/EditUser
        /// <summary>
        /// Gets input data and if state changed update it to database if successful, go to Admin/Index view
        /// Returns same screen with error message
        /// </summary>
        /// <param name="paramExpandedUserDTO"></param>
        /// <returns>returns Home/Index view after successfully updating database with updated data in pramExpandedUserDTO
        ///         if error, returns Admin/Edit|User with input data with error message </returns>
        [Authorize(Roles = "Administrator")]
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

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(paramExpandedUserDTO.UserName));
            }
        }
        #endregion

        // DELETE: /Admin/DeleteUser
        /// <summary>
        /// Gets selected user data and display pop up view with confirm delete,
        /// if confirm yes, delete the user record from database and returns Admin/Index view 
        /// </summary>
        /// <param name="string UserName"></param>
        /// <returns>returns Admin/Index view if successful , 
        ///         if error returns current EditUser view with error message. </returns>
        [Authorize(Roles = "Administrator")]
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

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(UserName));
            }
        }
        #endregion

        // GET: /Admin/EditRoles/TestUser
        /// <summary>
        /// Gets selected role name data and display editable Admin/EditRole view, 
        /// when submit button clicked send the input as objUserAndRolesDTO
        /// to the next Admin/EditRoles view 
        /// </summary>
        /// <param name="string UserName"></param>
        /// <returns>returns Admin/EditRoels</returns> 
        [Authorize(Roles = "Administrator")]
        #region ActionResult EditRoles(string UserName)
        public ActionResult EditRoles(string UserName)
        {
            if (UserName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserName = UserName.ToLower();

            // Check that we have an actual user
            ExpandedUserDTO objExpandedUserDTO = GetUser(UserName);

            if (objExpandedUserDTO == null)
            {
                return HttpNotFound();
            }

            UserAndRolesDTO objUserAndRolesDTO =
                GetUserAndRoles(UserName);

            return View(objUserAndRolesDTO);
        }
        #endregion

        // PUT: /Admin/EditRoles/TestUser 
        /// <summary>
        /// Gets passed updated role name data and save it on to database
        /// </summary>
        /// <param name="pramUserAndRolesDTO"></param>
        /// <returns>Returns Admin/EditRoles view filled with updated objUserAndRolesDTO data if successful, 
        ///         if error returns Admin/EditRoles with input data with error message</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult EditRoles(UserAndRolesDTO paramUserAndRolesDTO)
        public ActionResult EditRoles(UserAndRolesDTO paramUserAndRolesDTO)
        {
            try
            {
                if (paramUserAndRolesDTO == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                string UserName = paramUserAndRolesDTO.UserName;
                string strNewRole = Convert.ToString(Request.Form["AddRole"]);

                if (strNewRole != "No Roles Found")
                {
                    // Go get the User
                    ApplicationUser user = UserManager.FindByName(UserName);

                    // Put user in role
                    UserManager.AddToRole(user.Id, strNewRole);
                }

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesDTO objUserAndRolesDTO =
                    GetUserAndRoles(UserName);

                return View(objUserAndRolesDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditRoles");
            }
        }
        #endregion

        // DELETE: /Admin/DeleteRole?UserName="TestUser&RoleName=Administrator
        /// <summary>
        /// pop delete confirmation message with confirm delete,
        /// if yes delete role name data and save it on to database
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="RoleName"></param>
        /// <returns>returns the EditRole view with current username data after successfully deleting the role from database
        ///         if error, return EditRole view with current user role data saved in UserAdnRolesDTO objUserAndRolesDTO</returns>
        [Authorize(Roles = "Administrator")]
        #region public ActionResult DeleteRole(string UserName, string RoleName)
        public ActionResult DeleteRole(string UserName, string RoleName)
        {
            try
            {
                if ((UserName == null) || (RoleName == null))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                UserName = UserName.ToLower();

                // Check that we have an actual user
                ExpandedUserDTO objExpandedUserDTO = GetUser(UserName);

                if (objExpandedUserDTO == null)
                {
                    return HttpNotFound();
                }

                if (UserName.ToLower() ==
                    this.User.Identity.Name.ToLower() && RoleName == "Administrator")
                {
                    ModelState.AddModelError(string.Empty,
                        "Error: Cannot delete Administrator Role for the current user");
                }

                // Go get the User
                ApplicationUser user = UserManager.FindByName(UserName);
                // Remove User from role
                UserManager.RemoveFromRoles(user.Id, RoleName);
                UserManager.Update(user);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                return RedirectToAction("EditRoles", new { UserName = UserName });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesDTO objUserAndRolesDTO =
                    GetUserAndRoles(UserName);

                return View("EditRoles", objUserAndRolesDTO);
            }
        }
        #endregion

        // Roles *****************************

        // GET: /Admin/ViewAllRoles
        /// <summary>
        /// displays list of all role names extracted from database
        /// </summary>
        /// <returns>returns ViewAllRoles view filled with list of all the Role names</returns>
        [Authorize(Roles = "Administrator")]
        #region public ActionResult ViewAllRoles()
        public ActionResult ViewAllRoles()
        {   //create roleManger to access function of RoleManager
            var roleManager =
                new RoleManager<IdentityRole>
                (
                    new RoleStore<IdentityRole>(new ApplicationDbContext())
                    );
            //create list to store all roles from database using linq
            List<RoleDTO> colRoleDTO = (from objRole in roleManager.Roles
                                        select new RoleDTO
                                        {
                                            Id = objRole.Id,
                                            RoleName = objRole.Name
                                        }).ToList();

            return View(colRoleDTO);
        }
        #endregion

        // GET: /Admin/AddRole
        /// <summary>
        /// Displays blank role name view and pass the input to the next AddRole view
        /// </summary>
        /// <returns>returns returns empty AddRole view 
        /// and pass input data as objRoleDTO to next AddRole view</returns>
        [Authorize(Roles = "Administrator")]
        #region public ActionResult AddRole()
        public ActionResult AddRole()
        {
            RoleDTO objRoleDTO = new RoleDTO();

            return View(objRoleDTO);
        }
        #endregion

        // PUT: /Admin/AddRole
        /// <summary>
        /// saves the passed pramRoleDTO data on to database if successful returns ViewAlllRoles view
        /// <param name="pramRoleDTO"></param>
        /// <returns>return ViewAllRoles view after saving passed updated RolDTO input data on to database
        ///          if error, keep returning AddRole view </returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult AddRole(RoleDTO paramRoleDTO)
        public ActionResult AddRole(RoleDTO paramRoleDTO)
        {
            try
            {
                if (paramRoleDTO == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var RoleName = paramRoleDTO.RoleName.Trim();

                if (RoleName == "")
                {
                    throw new Exception("No RoleName");
                }

                // Create Role 
                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext())
                        );

                if (!roleManager.RoleExists(RoleName))
                {
                    roleManager.Create(new IdentityRole(RoleName));
                }

                return Redirect("~/Admin/ViewAllRoles");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("AddRole");
            }
        }
        #endregion

        // DELETE: /Admin/DeleteUserRole?RoleName=TestRole
        /// <summary>
        /// On click of delete, pops up delete confirmation message, 
        /// if yes, deletes the role name from the role database
        /// <param name="RoleName"></param>
        /// <returns>return current view of ViewAllRoles filled with updated roles list after deleteing the role from database </returns>
        [Authorize(Roles = "Administrator")]
        #region public ActionResult DeleteUserRole(string RoleName)
        public ActionResult DeleteUserRole(string RoleName)
        {
            try
            {
                if (RoleName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (RoleName.ToLower() == "administrator")
                {
                    throw new Exception(String.Format("Cannot delete {0} Role.", RoleName));
                }
                //create roleManager to access RoleManager function
                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext()));
                //find out if any users allocated this role already
                var UsersInRole = roleManager.FindByName(RoleName).Users.Count();
                if (UsersInRole > 0)
                {
                    throw new Exception(
                        String.Format(
                            "Canot delete {0} Role because it still has users.",
                            RoleName)
                            );
                }
                //find object to be deleted from database using linq
                var objRoleToDelete = (from objRole in roleManager.Roles
                                       where objRole.Name == RoleName
                                       select objRole).FirstOrDefault();
                if (objRoleToDelete != null)
                {
                    roleManager.Delete(objRoleToDelete);
                }
                else
                {
                    throw new Exception(
                        String.Format(
                            "Canot delete {0} Role does not exist.",
                            RoleName)
                            );
                }
                // now get up to date list of role names  and pass it to the list using linq query
                List<RoleDTO> colRoleDTO = (from objRole in roleManager.Roles
                                            select new RoleDTO
                                            {
                                                Id = objRole.Id,
                                                RoleName = objRole.Name
                                            }).ToList();

                return View("ViewAllRoles", colRoleDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                // create roleManage to access function of RoleManager
                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext()));
                // get existing list of role names and put it to the list using linq query
                List<RoleDTO> colRoleDTO = (from objRole in roleManager.Roles
                                            select new RoleDTO
                                            {
                                                Id = objRole.Id,
                                                RoleName = objRole.Name
                                            }).ToList();

                return View("ViewAllRoles", colRoleDTO);
            }
        }
        #endregion


        // Utility
        // getter and setter of UserManager
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
        // getter and setter of RoleManager
        #region public ApplicationRoleManager RoleManager
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion
        /// <summary>
        /// Get all roles and saves it as a list as a selectRoleListItem 
        /// returns a List<SelectListItem> SelectRoleListItems filled with Role names saved in database
        /// <returns>List<SelectListItem> SelectRoleListItems</returns>
        #region private List<SelectListItem> GetAllRolesAsSelectList()
        private List<SelectListItem> GetAllRolesAsSelectList()
        {
            List<SelectListItem> SelectRoleListItems =
                new List<SelectListItem>();

            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(new ApplicationDbContext()));

            var colRoleSelectList = roleManager.Roles.OrderBy(x => x.Name).ToList();

            SelectRoleListItems.Add(
                new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                });

            foreach (var item in colRoleSelectList)
            {
                SelectRoleListItems.Add(
                    new SelectListItem
                    {
                        Text = item.Name.ToString(),
                        Value = item.Name.ToString()
                    });
            }

            return SelectRoleListItems;
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
            objExpandedUserDTO.Email = result.Email;
            objExpandedUserDTO.LockoutEndDateUtc = result.LockoutEndDateUtc;
            objExpandedUserDTO.LockoutEnabled = result.LockoutEnabled;
            objExpandedUserDTO.AccessFailedCount = result.AccessFailedCount;


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
            ApplicationUser result =
                UserManager.FindByName(paramExpandedUserDTO.UserName);

            // If we could not find the user, throw an exception
            if (result == null)
            {
                throw new Exception("Could not find the User");
            }


            result.Forename = paramExpandedUserDTO.Forename;
            result.Surname = paramExpandedUserDTO.Surname;
            result.Street = paramExpandedUserDTO.Street;
            result.Town = paramExpandedUserDTO.Town;
            result.Postcode = paramExpandedUserDTO.Postcode;
            result.PhoneNumber = paramExpandedUserDTO.PhoneNumber;
            result.UserName = paramExpandedUserDTO.UserName;
            result.Email = paramExpandedUserDTO.Email;
            result.LockoutEndDateUtc = paramExpandedUserDTO.LockoutEndDateUtc;
            result.LockoutEnabled = paramExpandedUserDTO.LockoutEnabled;
            result.AccessFailedCount = paramExpandedUserDTO.AccessFailedCount;
            


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

        /// <summary>
        /// returns UserAndRolesDTO objUserAndRolesDTO that contains user and role 
        /// <param name="UserName"></param>
        /// <returns>objUserAndRolesDTO</returns>
        #region private UserAndRolesDTO GetUserAndRoles(string UserName)
        private UserAndRolesDTO GetUserAndRoles(string UserName)
        {
            // Go get the User
            ApplicationUser user = UserManager.FindByName(UserName);

            List<UserRoleDTO> colUserRoleDTO =
                (from objRole in UserManager.GetRoles(user.Id)
                 select new UserRoleDTO
                 {
                     RoleName = objRole,
                     UserName = UserName
                 }).ToList();

            if (colUserRoleDTO.Count() == 0)
            {
                colUserRoleDTO.Add(new UserRoleDTO { RoleName = "No Roles Found" });
            }

            ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

            // Create UserRolesAndPermissionsDTO
            UserAndRolesDTO objUserAndRolesDTO =
                new UserAndRolesDTO();
            objUserAndRolesDTO.UserName = UserName;
            objUserAndRolesDTO.colUserRoleDTO = colUserRoleDTO;
            return objUserAndRolesDTO;
        }
        #endregion

        /// <summary>
        /// returns role names that has no user allocated to it as a list 
        /// <param name="UserName"></param>
        /// <returns>colRolesUserInNotIn</returns>
        #region private List<string> RolesUserIsNotIn(string UserName)
        private List<string> RolesUserIsNotIn(string UserName)
        {
            // Get roles the user is not in
            var colAllRoles = RoleManager.Roles.Select(x => x.Name).ToList();

            // Go get the roles for an individual
            ApplicationUser user = UserManager.FindByName(UserName);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }

            var colRolesForUser = UserManager.GetRoles(user.Id).ToList();
            var colRolesUserInNotIn = (from objRole in colAllRoles
                                       where !colRolesForUser.Contains(objRole)
                                       select objRole).ToList();

            if (colRolesUserInNotIn.Count() == 0)
            {
                colRolesUserInNotIn.Add("No Roles Found");
            }

            return colRolesUserInNotIn;
        }
        #endregion
    }
}




