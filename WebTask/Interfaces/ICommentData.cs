using System;
using WebTask.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTask.Interfaces
{
    public interface ICommentData
    {
        IEnumerable<Comment> GetComment();
        void AddComment(Comment comment);

    }
}
