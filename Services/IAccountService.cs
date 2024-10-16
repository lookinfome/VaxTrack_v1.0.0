using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using System.Linq;


namespace VaxTrack_v1.Services
{
    public interface IAccountService
    {
        public UserDetailsModel ValidateUser(LoginDetailsModel submittedDetails);
        public bool IsUserAdmin(string username);
        public Task<LoginDetailsModel> SaveNewUser(UserDetailsModel submittedDetails);
    }

    public class AccountService : IAccountService
    {
        // variable:
        private readonly IAppUserService _appUserService;

        // variable: sqlite DB
        private AppDbContext _vaxTrackDBContext;

        // contructor
        public AccountService(AppDbContext vaxTrackDBContext, IAppUserService appUserService)
        {
            _vaxTrackDBContext = vaxTrackDBContext;
            _appUserService = appUserService;
        }

        // method: validate user based on credentials
        public UserDetailsModel ValidateUser(LoginDetailsModel submittedDetails)
        {
            try
            {
                if(submittedDetails.Username != null && submittedDetails.Password != null)
                {
                    // extract user details based on username and password
                    var _fetchedUserDetails = this._vaxTrackDBContext.UserDetails
                                        .FirstOrDefault(records => records.Username == submittedDetails.Username && records.Password == submittedDetails.Password);
                    
                    UserDetailsModel _userDetails = new UserDetailsModel();

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

        // method: save new user record in user details table and user vaccination details table
        public async Task<LoginDetailsModel> SaveNewUser(UserDetailsModel submittedDetails)
        {
            try
            {
                // new username logic for new user
                if (submittedDetails.Username == "DefaultUsername")
                {
                    // create new username
                    string newUsername = CreateUsername(submittedDetails);
                    // validate new username
                    while (IsUsernameExists(newUsername))
                    {
                        newUsername = CreateUsername(submittedDetails);
                    }
                    // assign new username to user
                    submittedDetails.Username = newUsername;
                }

                // Call RegisterUser method from AppUserService
                var registerResult = await _appUserService.RegisterUser(submittedDetails);

                if (!registerResult.Succeeded)
                {
                    Console.WriteLine($"Registration failed for user - {submittedDetails.Username}");
                    return new LoginDetailsModel();
                }

                // add new user record to user details table
                _vaxTrackDBContext.Add(submittedDetails);
                int userRegistrationSuccess = _vaxTrackDBContext.SaveChanges();
                // check if new record added or not
                if (userRegistrationSuccess <= 0)
                {
                    Console.WriteLine($"Registration failed for user - {submittedDetails.Username}");
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
                    Console.WriteLine($"Registration failed for user - {submittedDetails.Username}");
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
                Console.WriteLine($"An error occurred while saving new user: {ex.Message}");
                return new LoginDetailsModel();
            }
        }

        // method: check if user with role admin
        public bool IsUserAdmin(string username)
        {
            var _userDetails = _vaxTrackDBContext.UserDetails.FirstOrDefault(record=>record.Username == username);
            if(_userDetails?.Role != false)
            {
                return true;
            }
            return false;
        }

        // method: create username for new user
        private string CreateUsername(UserDetailsModel userDetails)
        {
            try
            {
                // collect details
                string _initials = (userDetails.Name[0].ToString()+userDetails.Name[userDetails.Name.Length-1].ToString()).ToUpper();
                string _birthYear = userDetails.Birthdate.ToString("yy");
                string _uniqueNum = userDetails.Uid.Substring(userDetails.Uid.Length-3);
                Random _randomNumGenerator = new Random();
                string _randomNumber = _randomNumGenerator.Next(0,999).ToString();

                // create new username
                string _username = _initials+_birthYear+_uniqueNum+_randomNumber;

                return _username;

            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occurred while creating username existence: {ex.Message}");
                return "";
            }
        }

        // method: validate username
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

    }
}
