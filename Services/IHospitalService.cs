using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using System.Linq;

namespace VaxTrack_v1.Services
{
    // interface: hospital service | to serve as service and allowed as injectable
    public interface IHospitalService
    {
        public List<HospitalDetailsModel> FetchHospitalDetails();
        public List<HospitalDetailsModel> FilterHospitalDetails(string filter);
        public bool UpdateHospitalDetails(string hospitalName1, string hospitalName2);
        public HospitalDetailsModel FetchHospitalDetailsById(string hospitalId);
    }

    // class: hospital service | implementing service methods and handeling utility methods
    public class HospitalService:IHospitalService
    {
        // variable: sqlite DB | to access DB tables
        private readonly AppDbContext _vaxTrackDBContext;

        // contructor: hospital service | to initialize account service class variables
        public HospitalService(AppDbContext vaxTrackDBContext)
        {
            _vaxTrackDBContext = vaxTrackDBContext;
        }

        /*
        *   service method: FetchHospitalDetails()
        *   purpose: to fetch list of hospital details with available slots
        *   return: list of hospital details with available slots
        */
        public List<HospitalDetailsModel> FetchHospitalDetails()
        {
            try
            {
                // fetch hospital details with available slots
                var _hospitalDetails = _vaxTrackDBContext.HospitalDetails;

                if(_hospitalDetails != null)
                {
                    return _hospitalDetails.ToList();
                }

                return new List<HospitalDetailsModel>();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpeted error occurred while fetching hospital details: {ex.Message}");
                return new List<HospitalDetailsModel>();
            }

        }

        /*
        *   service method: FetchHospitalDetailsById()
        *   purpose: to fetch list of hospital details with available slots
        *   parameter: hospital Id
        *   return: list of hospital details with available slots
        */
        public HospitalDetailsModel FetchHospitalDetailsById(string hospitalId)
        {
            HospitalDetailsModel hospitalDetails = _vaxTrackDBContext.HospitalDetails.FirstOrDefault(record => record.HospitalId == hospitalId);
            return hospitalDetails;
        }



        /*
        *   service method: FilterHospitalDetails()
        *   purpose: to filter list of hospital details based on available slots
        *   parameter: filter value as string
        *   return: filtered list of hospital details based on available slots
        */
        public List<HospitalDetailsModel> FilterHospitalDetails(string filter)
        {
            try
            {
                // fetch hospital details
                var _hospitalDetails = FetchHospitalDetails().AsQueryable();

                if(!string.IsNullOrEmpty(filter))
                {
                    // fetch filtered list
                    _hospitalDetails = _hospitalDetails.Where(record=>record.SlotsAvailable <= int.Parse(filter)).OrderBy(record=>record.SlotsAvailable).Reverse();
                }

                return _hospitalDetails.ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occured while filtering hospital details: {ex.Message}");
                return new List<HospitalDetailsModel>();
            }

        }

        /*
        *   service method: UpdateHospitalDetails()
        *   purpose: to update avilable slots of hospital 
        *   parameter: hospital names
        *   return: bool value, 0 for not updated, 1 for updated
        */
        public bool UpdateHospitalDetails(string hospitalName1, string hospitalName2)
        {
            try
            {
                if(hospitalName1 == hospitalName2)
                {
                    // fetch hospital details for the hospital name
                    var _hospitalDetails = _vaxTrackDBContext.HospitalDetails.FirstOrDefault(record=>record.HospitalName == hospitalName1);
                    if(_hospitalDetails != null)
                    {
                        // update the slots
                        _hospitalDetails.SlotsAvailable = _hospitalDetails.SlotsAvailable+2;

                        // save the updated slots
                        _vaxTrackDBContext.HospitalDetails.Update(_hospitalDetails);
                        _vaxTrackDBContext.SaveChanges();

                        return true;
                    }
                }
                else
                {
                    // fetch hospital details for the hospital name
                    var _hospitalDetails1 = _vaxTrackDBContext.HospitalDetails.FirstOrDefault(record=>record.HospitalName == hospitalName1);
                    var _hospitalDetails2 = _vaxTrackDBContext.HospitalDetails.FirstOrDefault(record=>record.HospitalName == hospitalName2);

                    if(_hospitalDetails1 != null && _hospitalDetails2 != null)
                    {
                        // update and save the slots
                        _hospitalDetails1.SlotsAvailable = _hospitalDetails1.SlotsAvailable+1;
                        _vaxTrackDBContext.HospitalDetails.Update(_hospitalDetails1);
                        _vaxTrackDBContext.SaveChanges();

                        // update and save the slots
                        _hospitalDetails2.SlotsAvailable = _hospitalDetails2.SlotsAvailable+1;
                        _vaxTrackDBContext.HospitalDetails.Update(_hospitalDetails2);
                        _vaxTrackDBContext.SaveChanges();

                        return true;
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred while updating hospital details: {ex.Message}");
                return false;
            }    
        }

    }
}