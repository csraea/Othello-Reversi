using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;


namespace Reversi.Core.Service.Comments
{
    public class CommentServiceEF : ICommentService
    {
        public void AddComment(Comment comment)
        {
            using (var context = new ReversiDBContext())
            {
                context.Comments.Add(comment);
                context.SaveChanges();
            }
        }

        [System.Obsolete]
        public void ClearComments()
        {
            using (var context = new ReversiDBContext())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Comments");
            }
        }

        public IList<Comment> GetLastComments()
        {
            using (var context = new ReversiDBContext())
            {
                return (from c in context.Comments
                        orderby c.Time
                           descending
                        select c).Take(5).ToList();
            }
        }
    }
}
