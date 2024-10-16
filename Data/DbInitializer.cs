// using System.Linq;
// using VaxTrack_v1.Models;

// namespace VaxTrack_v1.Data
// {
//     // base data to be seeded in in-house DB. -- Vivek
//     public static class DbInitializer
//     {
//         public static void Initialize(AppDbContext context)
//         {
//             // seeding users

//             if (context.UserDetails.Any())
//             {
//                 return;   // DB has been seeded
//             }

//             if (context.UserVaccinationDetails.Any())
//             {
//                 return; // DB has been seeded
//             }

//             if (context.HospitalDetails.Any())
//             {
//                 return; // DB has been seeded
//             }

//             if (context.BookingDetails.Any())
//             {
//                 return; // DB has been seeded
//             }


//             // user details

//             var userDetails = new UserDetailsModel[]
//             {
//                 new UserDetailsModel {
//                     Name = "John Doe",
//                     Birthdate = new DateTime(1998, 2, 10),
//                     Uid = "123456789012",
//                     Gender = "M",
//                     Phone = "123-456-7890",
//                     Username = "johndoe",
//                     Password = "password123"

//                 },
//                 new UserDetailsModel {
//                     Name = "Jane Smith",
//                     Birthdate = new DateTime(1999, 6, 8),
//                     Uid = "234567890123",
//                     Gender = "F",
//                     Phone = "234-567-8901",
//                     Username = "janesmith",
//                     Password = "password123"

//                 },
//                 new UserDetailsModel {
//                     Name = "Alice Brown",
//                     Birthdate = new DateTime(1998, 3, 8),
//                     Uid = "345678901234",
//                     Gender = "F",
//                     Phone = "345-678-9012",
//                     Username = "alicebrown",
//                     Password = "password123"

//                 }
//             };

//             foreach (var record in userDetails)
//             {
//                 context.UserDetails.Add(record);
//             }
//             context.SaveChanges();

//             // user vaccination details

//             var userVaccinationDetails = new UserVaccinationDetailsModel[]
//             {
//                 new UserVaccinationDetailsModel {
//                     Username = "johndoe",
//                     VaccinationStatus = "Not Vaccinated",
//                     Dose1Date = DateTime.MinValue,
//                     Dose2Date = DateTime.MinValue
//                 },

//                 new UserVaccinationDetailsModel {
//                     Username = "janesmith",
//                     VaccinationStatus = "Not Vaccinated",
//                     Dose1Date = DateTime.MinValue,
//                     Dose2Date = DateTime.MinValue
//                 },

//                 new UserVaccinationDetailsModel {
//                     Username = "alicebrown",
//                     VaccinationStatus = "Not Vaccinated",
//                     Dose1Date = DateTime.MinValue,
//                     Dose2Date = DateTime.MinValue
//                 }
//             };

//             foreach (var record in userVaccinationDetails)
//             {
//                 context.UserVaccinationDetails.Add(record);
//             }
//             context.SaveChanges();

//             // booking details
//             var bookingDetails = new BookingDetailsModel[]
//             {
//                 new BookingDetailsModel {
//                     Username = "alicebrown",
//                     Dose1Date = DateTime.MinValue,
//                     Dose2Date = DateTime.MinValue,
//                     D1HospitalName = "",
//                     D2HospitalName = "",
//                     SlotNumber = 0
//                 }
//             };

//             // hospital details

//             var hospitalDetails = new HospitalDetailsModel[]
//             {
//                 new HospitalDetailsModel {
//                     HospitalId = "H1",
//                     HospitalName = "Hos1",
//                     SlotsAvailable = 10
//                 },
//                 new HospitalDetailsModel {
//                     HospitalId = "H2",
//                     HospitalName = "HOS2",
//                     SlotsAvailable = 10
//                 }
//             };

//             foreach (var record in hospitalDetails)
//             {
//                 context.HospitalDetails.Add(record);
//             }
//             context.SaveChanges();


//         }
//     }    
// }