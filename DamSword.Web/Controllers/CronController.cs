using System.Collections.Generic;
using System.Linq;
using DamSword.Common;
using DamSword.Data.Repositories;
using DamSword.Watch;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web.Controllers
{
    public class CronController : Controller
    {
        private readonly IWatchRepository _watchRepository;
        private readonly IEnumerable<IWatchService> _watchServices;
        
        public CronController(IWatchRepository watchRepository, IEnumerable<IWatchService> watchServices)
        {
            _watchRepository = watchRepository;
            _watchServices = watchServices;
        }

        [Route("/cron/fetch-data")]
        public IActionResult FetchData()
        {
            var watchServicesWebSesourceIds = _watchServices.ToDictionary(s => s, s => s.WebResourceIds);
            var webResourceIds = watchServicesWebSesourceIds.Values.SelectMany(ids => ids).Distinct().ToArray();
            var watches = _watchRepository.Select(w => webResourceIds.Contains(w.WebResourceId), w => new
            {
                w.PersonId,
                w.WebResourceId
            }).ToArray();

            foreach (var watchService in _watchServices)
            {
                var relatedWatchIds = watches
                    .Where(w => watchServicesWebSesourceIds[watchService].Contains(w.WebResourceId))
                    .Select(w => w.PersonId)
                    .Distinct()
                    .ToArray();

                foreach (var watchIdBatch in relatedWatchIds.Batch(watchService.MaxStackSize))
                {
                    watchService.FetchOnline(watchIdBatch);
                }
            }
            
            return new EmptyResult();
        }
    }
}