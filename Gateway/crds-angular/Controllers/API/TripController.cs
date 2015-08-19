﻿using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using crds_angular.Exceptions.Models;
using crds_angular.Models.Crossroads.Trip;
using crds_angular.Security;
using crds_angular.Services.Interfaces;

namespace crds_angular.Controllers.API
{
    public class TripController : MPAuth
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [AcceptVerbs("GET")]
        [ResponseType(typeof(TripFormResponseDto))]
        [Route("api/trip/form-responses/{selectionId}/{selectionCount}")]
        public IHttpActionResult TripFormResponses(int selectionId, int selectionCount)
        {
            return Authorized(token =>
            {
                try
                {
                    var groups = _tripService.GetFormResponses(selectionId, selectionCount);
                    return Ok(groups);
                }
                catch (Exception ex)
                {
                    var apiError = new ApiErrorDto("GetGroupsForEvent Failed", ex);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }

        [AcceptVerbs("POST")]
        [Route("api/trip/participants")]
        public IHttpActionResult SaveParticipants([FromBody] SaveTripParticipantsDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(val => val.Errors).Aggregate("", (current, err) => current + err.Exception.Message);
                var dataError = new ApiErrorDto("Trip-SaveParticipants Data Invalid", new InvalidOperationException("Invalid SaveParticipants Data" + errors));
                throw new HttpResponseException(dataError.HttpResponseMessage);
            }

            try
            {
                _tripService.SaveParticipants(dto);
            }
            catch (Exception exception)
            {
                var apiError = new ApiErrorDto("SaveParticipants Failed", exception);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
            return Ok();
        }

        [AcceptVerbs("GET")]
        [ResponseType(typeof (TripParticipantDto))]
        [Route("api/trip/search/{query?}")]
        public IHttpActionResult Search(string query)
        {
            try
            {
                var list = _tripService.Search(query);
                return Ok(list);
            }
            catch (Exception ex)
            {
                var apiError = new ApiErrorDto("Trip Search Failed", ex);
                throw new HttpResponseException(apiError.HttpResponseMessage);
            }
        }

        [AcceptVerbs("GET")]
        [ResponseType(typeof (MyTripsDTO))]
        [Route("api/trip/mytrips/{contactId}")]
        public IHttpActionResult MyTrips(int contactId)
        {
            return Authorized(token =>
            {
                try
                {
                    var trips = _tripService.GetMyTrips(contactId, token);
                    return Ok(trips);
                }
                catch (Exception ex)
                {
                    var apiError = new ApiErrorDto("Failed to retrieve My Trips info", ex);
                    throw new HttpResponseException(apiError.HttpResponseMessage);
                }
            });
        }
    }
}