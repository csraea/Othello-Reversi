using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Reversi.Core.Service.Comments {
    public class CommentService : ICommentService {
        
        private const string FileName = "comments.bin";

        private List<Comment> comments = new List<Comment>();

        public void AddComment(Comment comment) {
            comments.Add(comment);

            SaveComment();
        }

        public IList<Comment> GetLastComments() {
            LoadComment();

            return (from s in comments orderby s.Time
                descending select s).Take(2).ToList();
        }

        public void ClearComments() {
            comments.Clear();
            File.Delete(FileName);
        }


        private void SaveComment() {
            using (var fs = File.OpenWrite(FileName)) {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, comments);
            }
        }

        private void LoadComment() {
            if (File.Exists(FileName)) {
                using (var fs = File.OpenRead(FileName)) {
                    var bf = new BinaryFormatter();
                    comments = (List<Comment>)bf.Deserialize(fs);
                }
            }
        }
    }
}