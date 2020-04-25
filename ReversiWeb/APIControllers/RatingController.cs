using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reversi.Core.Service.Rating;

namespace ReversiWeb.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private IRatingService _ratingService = new RatingServiceEF();

        // GET: api/Score
        [HttpGet]
        public IEnumerable<Rating> Get()
        {
            return _ratingService.GetLastRatings();
        }

        // POST: api/Score
        [HttpPost]
        public void Post([FromBody] Rating rating)
        {
            _ratingService.Rate(rating);
        }
    }
}