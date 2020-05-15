using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reversi.Core.Service.Comments;

namespace ReversiWeb.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private ICommentService _commentService = new CommentServiceEF();

        // GET: api/Score
        [HttpGet]
        public IEnumerable<Comment> Get()
        {
            return _commentService.GetLastComments();
        }

        // POST: api/Score
        [HttpPost]
        public void Post([FromBody] Comment comment)
        {
            _commentService.AddComment(comment);
        }
    }
}