using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reversi.Core.Service.Comments;
using Reversi.Core.Service.Score;
using Service;

namespace Reversi.Test {
    
    [TestClass]
    public class TComment {
        private ICommentService CommentService() {
            var commentService = new CommentService();
            commentService.ClearComments();
            return commentService;
        }


        [TestMethod]
        public void AddComment1() {
            var commentService = CommentService();

            commentService.AddComment(new Comment { Player = "Janko", Text = "Nothing.", Time = DateTime.Now});
            var comments = commentService.GetLastComments();

            Assert.AreEqual<int>(1, comments.Count);
            Assert.AreEqual<string>("Janko", comments[0].Player);
            Assert.AreEqual<string>("Nothing.", comments[0].Text);
        }

        [TestMethod]
        public void AddComment2() {
            var commentService = CommentService();

            commentService.AddComment(new Comment { Player = "Janko", Text = "Nothing.", Time = DateTime.Now});
            commentService.AddComment(new Comment { Player = "Marius", Text = "Something.", Time = DateTime.Now});
            commentService.AddComment(new Comment { Player = "Jakub", Text = "Anything.", Time = DateTime.Now});
            var comments = commentService.GetLastComments();

            Assert.AreEqual<int>(2, comments.Count);
            Assert.AreEqual<string>("Jahub", comments[0].Player);
            Assert.AreEqual<string>("Anything.", comments[0].Text);
            Assert.AreEqual<string>("Marius", comments[1].Player);
            Assert.AreEqual<string>("Something.", comments[1].Text);
        }

        [TestMethod]
        public void AddComment3() {
            var commentService = CommentService();
            var comments = commentService.GetLastComments();
            Assert.AreEqual<int>(0, comments.Count);
        }

    }
}