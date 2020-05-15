using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reversi.Core.Service.Score;
using Service;

namespace ReversiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private IScoreService _scoreService = new ScoreServiceEF();

        // GET: api/Score
        [HttpGet]
        public IEnumerable<Score> Get()
        {
            return _scoreService.GetTopScores();
        }

        // POST: api/Score
        [HttpPost]
        public void Post([FromBody] Score score)
        {
            _scoreService.AddScore(score);
        }
    }

}