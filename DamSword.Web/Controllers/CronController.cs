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
        private readonly IEnumerable<IWatch> _watches;
        
        public CronController(IWatchRepository watchRepository, IEnumerable<IWatch> watches)
        {
            _watchRepository = watchRepository;
            _watches = watches;
        }

        [HttpPost]
        [Route("/cron/fetch-data")]
        public IActionResult FetchData()
        {
            var watchWebSesourceIdDict = _watches.ToDictionary(s => s, s => s.WebResourceId);
            var webResourceIds = watchWebSesourceIdDict.Values.Distinct().ToArray();
            var watches = _watchRepository.Select(w => webResourceIds.Contains(w.WebResourceId), w => new
            {
                w.PersonId,
                w.WebResourceId
            }).ToArray();

            foreach (var watch in _watches)
            {
                var relatedWatchIds = watches
                    .Where(w => w.WebResourceId == watchWebSesourceIdDict[watch])
                    .Select(w => w.PersonId)
                    .Distinct()
                    .ToArray();

                foreach (var watchIdBatch in relatedWatchIds.Batch(watch.MaxStackSize))
                {
                    watch.FetchOnline(watchIdBatch);
                }
            }
            
            return new EmptyResult();
        }
    }
}