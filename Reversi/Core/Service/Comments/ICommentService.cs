using System.Collections.Generic;

namespace Reversi.Core.Service.Comments {
    public interface ICommentService {
        void AddComment(Comment comment);

        IList<Comment> GetLastComments();

        void ClearComments();
    }
}