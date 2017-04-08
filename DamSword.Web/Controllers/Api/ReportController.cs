using System;
using System.Collections.Generic;
using System.Linq;
using DamSword.Common;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Watch;
using DamSword.Web.Attributes;
using DamSword.Web.DTO;
using DamSword.Web.DTO.Report;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers.Api
{
    [Authorize]
    public class ReportController : ApiControllerBase
    {
        private readonly IPersonRepository _personRepository;
        private readonly IEnumerable<IWatch> _watches;

        public ReportController(IPersonRepository personRepository, IEnumerable<IWatch> watches)
        {
            _personRepository = personRepository;
            _watches = watches;
        }

        [HttpGet]
        [Require(UserPermissions.Owner)]
        public IActionResult Online([FromQuery] Request<OnlineRequest> request)
        {
            var shapshots = new List<OnlineResponse.OnlineSnapshot>();
            var response = new OnlineResponse
            {
                Snapshots = shapshots
            };
            
            var personIds = request.Data.PersonIds.IsNullOrEmpty() ? null : request.Data.PersonIds;
            var validPersonIds = _personRepository.Select(p => personIds == null || personIds.Contains(p.Id), p => p.Id).ToArray();
            if (validPersonIds.IsEmpty())
                return this.ApiResult(request, response);
            
            foreach (var watch in _watches)
            {
                var webResourceId = watch.WebResourceId;
                var onlineTimelines = watch.GetOnlineTimelines(request.Data.Begin, request.Data.End, validPersonIds);

                shapshots.AddRange(onlineTimelines.Select(onlineTimeline => new OnlineResponse.OnlineSnapshot
                {
                    From = onlineTimeline.From,
                    PersonId = onlineTimeline.PersonId,
                    WebResourceId = webResourceId,
                    AccountId = onlineTimeline.AccountId,
                    Chunks = onlineTimeline.Chunks.Select(c => new OnlineResponse.TimelineChunk
                    {
                        OnlineType = c.OnlineType, OnlineMeta = c.OnlineMeta, Length = c.Length
                    })
                }));
            }

            return this.ApiResult(request, response);
        }
    }
}