using System.Collections.Generic;
using crds_angular.Models.Crossroads;

namespace crds_angular.Services.Interfaces
{
    public interface ICongregationService
    {
        Congregation GetCongregationById(int id);
        List<Room> GetRooms(int congregationId);
    }
}