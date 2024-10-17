using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using System.Linq;


namespace VaxTrack_v1.Services
{
    // interface: account service | to serve as service and allowed as injectable
    public interface IAccountService
    {
        public bool IsUserAdmin(string username);
        public Task<LoginDetailsModel> SaveNewUser(UserDetailsModel submittedDetails);
    }

    // class: account service | implementing service methods and handeling utility methods
    public class AccountService : IAccountService
    {
        // variable: app user service | to handle authentication
        private readonly IAppUserService _appUserService;

        // variable: sqlite DB | to access DB tables
        private readonly AppDbContext _vaxTrackDBContext;

        // contructor: account service | to initialize account service class variables
        public AccountService(AppDbContext vaxTrackDBContext, IAppUserService appUserService)
        {
            _vaxTrackDBContext = vaxTrackDBContext;
            _appUserService = appUserService;
        }
        
        /*
        *   service method: SaveNewUser()
        *   purpose: to validate user basd on provided username from user details table
        *   parameter: user details model object
        *   return: return login details of user as object
        */
        public async Task<LoginDetailsModel> SaveNewUser(UserDetailsModel submittedDetails)
        {
            try
            {
                // create and assign new username to new user
                if (submittedDetails.Username == "DefaultUsername")
                {
                    // create new username
                    string newUsername = CreateUsername(submittedDetails);
                    
                    // validate new username
                    while (IsUsernameExists(newUsername))
                    {
                        // if username exists, create again new username
                        newUsername = CreateUsername(submittedDetails);
                    }
                    
                    // assign new username to user
                    submittedDetails.Username = newUsername;
                }

                // call RegisterUser method from AppUserService
                var registerResult = await _appUserService.RegisterUser(submittedDetails);

                // if user registration failed in AspNetUsers for authentication
                if (!registerResult.Succeeded)
                {
                    Console.WriteLine($"Registration failed for user - {submittedDetails.Username}");
                    return new LoginDetailsModel();
                }

                // save new user record to user details table
                _vaxTrackDBContext.Add(submittedDetails);
                int userRegistrationSuccess = _vaxTrackDBContext.SaveChanges();
                
                // check if new record added or not to user detials table
                if (userRegistrationSuccess <= 0)
                {
                    Console.WriteLine($"Error while saving new user details in table for - {submittedDetails.Username}");
                    return new LoginDetailsModel();
                }

                // create a record for new user's vaccination details
                var newUserVaccinationDetail = new UserVaccinationDetailsModel
                {
                    Username = submittedDetails.Username
                };

                // save new user's vaccination details
                _vaxTrackDBContext.UserVaccinationDetails.Add(newUserVaccinationDetail);
                int newUserVaccinationDetailSuccess = _vaxTrackDBContext.SaveChanges();
                
                // check if new vaccination details added or not
                if (newUserVaccinationDetailSuccess <= 0)
                {
                    Console.WriteLine($"Error while saving new user's vaccination details for - {submittedDetails.Username}");
                    return new LoginDetailsModel();
                }

                // return login details
                return new LoginDetailsModel
                {
                    Username = submittedDetails.Username,
                    Password = submittedDetails.Password
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while registeration/saving new user/user's vacination details: {ex.Message}");
                return new LoginDetailsModel();
            }
        }

        /*
        *   service method: IsUserAdmin()
        *   purpose: to check if user is admin or not
        *   parameter: username
        *   return: return true if user is admin, else false
        */
        public bool IsUserAdmin(string username)
        {
            // fetch user details
            var _userDetails = _vaxTrackDBContext.UserDetails.FirstOrDefault(record=>record.Username == username);
             
            if(_userDetails?.Role != false)
            {
                // if admin
                return true;
            }

            // else user
            return false;
        }

        /*
        *   utiltiy method: CreateUsername()
        *   purpose: to create unique new username
        *   parameter: user details model object
        *   return: return new unique username as string
        */
        private string CreateUsername(UserDetailsModel userDetails)
        {
            try
            {
                // collect user's details
                string _initials = (userDetails.Name[0].ToString()+userDetails.Name[userDetails.Name.Length-1].ToString()).ToUpper();
                string _birthYear = userDetails.Birthdate.ToString("yy");
                string _uniqueNum = userDetails.Uid.Substring(userDetails.Uid.Length-3);
                Random _randomNumGenerator = new Random();
                string _randomNumber = _randomNumGenerator.Next(0,999).ToString();

                // create new username
                string _username = _initials+_birthYear+_uniqueNum+_randomNumber;

                // return new username
                return _username;

            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occurred while creating username existence: {ex.Message}");
                return "";
            }
        }

        /*
        *   utiltiy method: IsUsernameExists()
        *   purpose: to check if username already exists in user detail table
        *   parameter: username as string
        *   return: return true if username exists, else false 
        */
        private bool IsUsernameExists(string newUsername)
        {
            try
            {
                return _vaxTrackDBContext.UserDetails.Any(record => record.Username == newUsername);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occurred while checking username existence: {ex.Message}");
                return false;
            }
            
        }

        /*
        *   utiltiy method: ValidateUser()
        *   purpose: to validate user basd on provided username from user details table
        *   parameter: login details model object
        *   return: return user details record as object
        */
        public UserDetailsModel ValidateUser(LoginDetailsModel submittedDetails)
        {
            try
            {
                if(submittedDetails.Username != null && submittedDetails.Password != null)
                {
                    // local variable: to store fetched user details 
                    UserDetailsModel _userDetails = new UserDetailsModel();

                    // fetch user details based on username and password
                    var _fetchedUserDetails = this._vaxTrackDBContext.UserDetails
                                        .FirstOrDefault(records => records.Username == submittedDetails.Username && records.Password == submittedDetails.Password);
                    
                    // if record exists
                    if(_fetchedUserDetails != null)
                    {
                        _userDetails = _fetchedUserDetails;
                    }
                    
                    return _userDetails;
                }
                else
                {
                    Console.WriteLine($"No record found for user - {submittedDetails.Username}");
                    return new UserDetailsModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while validating user: {ex.Message}");
                return new UserDetailsModel();
            }

            
        }


    }
}
