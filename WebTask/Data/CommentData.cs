using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTask.Interfaces;
using WebTask.Models;

namespace WebTask.Data
{
    public class CommentData : ICommentData
    {
        private readonly ApplicationContext context;

        public CommentData(ApplicationContext context)
        {
            this.context = context;
        }

        public void AddComment(Comment comment)
        {
            context.comments.Add(comment);
            context.SaveChanges();
        }

        public IEnumerable<Comment> GetComment()
        {
            return this.context.comments;
        }
    }
}
