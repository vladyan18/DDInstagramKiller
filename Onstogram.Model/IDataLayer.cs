using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onstogram.Model
{
    public interface IDataLayer
    {
        User AddUser(User user);
        User GetUser(Guid id);

        Image AddImage(Image image);
        Image GetImage(Guid imageId);
        void DeleteImage(Guid imageId);

        Comment AddCommentToImage(Guid imageId, Comment comment );
        List<Comment> GetImageComments(Guid imageId);
        Comment GetComment(Guid id);
        void DeleteComment(Guid commentId);

        void addHashTagToImage(Guid imageId, string tag);

        List<Image> GetLastImages();
        List<Image> GetImagesByHashtag(string tag);
    }
}
