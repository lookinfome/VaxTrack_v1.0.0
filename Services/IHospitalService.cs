using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VaxTrack_v1.Models;
using System.Linq;

namespace VaxTrack_v1.Services
{
    public interface IHospitalService
    {
        public List<HospitalDetailsModel> FetchHospitalDetails();
        public List<HospitalDetailsModel> FilterHospitalDetails(string filter);
        public bool UpdateHospitalDetails(string hospitalName1, string hospitalName2);
    }

    public class HospitalService:IHospitalService
    {
        private AppDbContext _vaxTrackDBContext;

        // constructor
        public HospitalService(AppDbContext vaxTrackDBContext)
        {
            _vaxTrackDBContext = vaxTrackDBContext;
        }

        // method: fetch hospital details
        public List<HospitalDetailsModel> FetchHospitalDetails()
        {
            try
            {
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

        // method: filter hospital name by slots available
        public List<HospitalDetailsModel> FilterHospitalDetails(string filter)
        {
            try
            {
                var _hospitalDetails = FetchHospitalDetails().AsQueryable();

                if(!string.IsNullOrEmpty(filter))
                {
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

        // method: update hospital details
        public bool UpdateHospitalDetails(string hospitalName1, string hospitalName2)
        {
            try
            {
                if(hospitalName1 == hospitalName2)
                {
                    var _hospitalDetails = _vaxTrackDBContext.HospitalDetails.FirstOrDefault(record=>record.HospitalName == hospitalName1);
                    if(_hospitalDetails != null)
                    {
                        _hospitalDetails.SlotsAvailable = _hospitalDetails.SlotsAvailable+2;
                        _vaxTrackDBContext.HospitalDetails.Update(_hospitalDetails);
                        _vaxTrackDBContext.SaveChanges();

                        return true;
                    }
                }
                else
                {
                    var _hospitalDetails1 = _vaxTrackDBContext.HospitalDetails.FirstOrDefault(record=>record.HospitalName == hospitalName1);
                    var _hospitalDetails2 = _vaxTrackDBContext.HospitalDetails.FirstOrDefault(record=>record.HospitalName == hospitalName2);

                    if(_hospitalDetails1 != null && _hospitalDetails2 != null)
                    {
                        _hospitalDetails1.SlotsAvailable = _hospitalDetails1.SlotsAvailable+1;
                        _vaxTrackDBContext.HospitalDetails.Update(_hospitalDetails1);
                        _vaxTrackDBContext.SaveChanges();

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