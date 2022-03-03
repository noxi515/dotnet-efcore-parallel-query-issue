using System;
using System.Linq;
using Xunit;

namespace EFCoreParallelQuery
{
    public class UnitTest1
    {
        [Fact]
        public void Sqlite()
        {
            using (var context = DatabaseContext.CreateSqliteInstance())
            {
                Test(context);
            }
        }

        [Fact]
        public void SqlServer()
        {
            using (var context = DatabaseContext.CreateSqlServerInstance())
            {
                Test(context);
            }
        }

        private void Test(DatabaseContext context)
        {
            var posts = Enumerable.Range(1, 100)
                .Select(i => new BlogPost { Id = Guid.NewGuid(), Title = $"Title-{i:000}" });

            foreach (var post in posts)
            {
                context.BlogPost.Add(post);
                context.BlogComment.AddRange(Enumerable.Range(1, 10)
                    .Select(i => new BlogComment
                        { Id = Guid.NewGuid(), PostId = post.Id, Comment = $"Comment-{i:00}" }));
            }

            context.SaveChanges();

            foreach (var post in context.BlogPost.OrderBy(x => x.Title).Take(5))
            {
                _ = context.BlogComment.Where(x => x.PostId == post.Id).ToList();
            }

            Assert.True(true);
        }
    }
}