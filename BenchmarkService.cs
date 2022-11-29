using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using Microsoft.EntityFrameworkCore;
using MakeMeFaster.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MakeMeFaster
{
    [MemoryDiagnoser]
    [InProcess]
    //[HideColumns(BenchmarkDotNet.Columns.Column.Job, BenchmarkDotNet.Columns.Column.RatioSD, BenchmarkDotNet.Columns.Column.StdDev, BenchmarkDotNet.Columns.Column.AllocRatio)]
    //[Config(typeof(Config))]
    public class BenchmarkService
    {
        public BenchmarkService()
        {
        }

        //private class Config : ManualConfig
        //{
        //public Config()
        //{
        //    SummaryStyle = BenchmarkDotNet.Reports.SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
        //}
        //}

        /// <summary>
        /// Get top 2 Authors (FirstName, LastName, UserName, Email, Age, Country) 
        /// from country Serbia aged 27, with the highest BooksCount
        /// and all his/her books (Book Name/Title and Publishment Year) published before 1900
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public List<AuthorDTO> GetAuthors()
        {
            using var dbContext = new AppDbContext();

            var authors = dbContext.Authors
                                        .Include(x => x.User)
                                        .ThenInclude(x => x.UserRoles)
                                        .ThenInclude(x => x.Role)
                                        .Include(x => x.Books)
                                        .ThenInclude(x => x.Publisher)
                                        .ToList()
                                        .Select(x => new AuthorDTO
                                        {
                                            UserCreated = x.User.Created,
                                            UserEmailConfirmed = x.User.EmailConfirmed,
                                            UserFirstName = x.User.FirstName,
                                            UserLastActivity = x.User.LastActivity,
                                            UserLastName = x.User.LastName,
                                            UserEmail = x.User.Email,
                                            UserName = x.User.UserName,
                                            UserId = x.User.Id,
                                            RoleId = x.User.UserRoles.FirstOrDefault(y => y.UserId == x.UserId).RoleId,
                                            BooksCount = x.BooksCount,
                                            AllBooks = x.Books.Select(y => new BookDto
                                            {
                                                Id = y.Id,
                                                Name = y.Name,
                                                Published = y.Published,
                                                ISBN = y.ISBN,
                                                PublisherName = y.Publisher.Name
                                            }).ToList(),
                                            AuthorAge = x.Age,
                                            AuthorCountry = x.Country,
                                            AuthorNickName = x.NickName,
                                            Id = x.Id
                                        })
                                        .ToList()
                                        .Where(x => x.AuthorCountry == "Serbia" && x.AuthorAge == 27)
                                        .ToList();

            var orderedAuthors = authors.OrderByDescending(x => x.BooksCount).ToList().Take(2).ToList();

            List<AuthorDTO> finalAuthors = new List<AuthorDTO>();
            foreach (var author in orderedAuthors)
            {
                List<BookDto> books = new List<BookDto>();

                var allBooks = author.AllBooks;

                foreach (var book in allBooks)
                {
                    if (book.Published.Year < 1900)
                    {
                        book.PublishedYear = book.Published.Year;
                        books.Add(book);
                    }
                }

                author.AllBooks = books;
                finalAuthors.Add(author);
            }

            return finalAuthors;
        }

        [Benchmark]
        public List<AuthorDTO> GetAuthors_Optimized()
        {
            // Include your optimization below, with comments explaining your thought process

            /*
              1. The first think which I see here is that we use .ToList() which will return all records from database
                  I will remove all .ToList()

              2. I moved OrderBy  in WHere statment to limited the number of authors returns

              3. Moving Published.Years to .Select() statment because we alredy fetch the actors, books
                 and we can move the part from belowe to .AllBooks part

              4. If we pay attention, we can see that our query returns a lot of necessary information
                 such as UserCreated = User.Created,UserEmailConfirmed etc...So I will be remove all
                of necessary field

              5. Same for the book, we only need Name and Published

              6. I removed Include() and .ThenInclude() from query because we don't need it and it doesn't do
                 anything usefull for this query and query can execute fine without them.
                 We will not increase performance but we can make our query more readable.

              7. I move .OrderByDescending(x => x.BooksCount) in the top of query
                 and remove BookCount() because we don't need it and it will give at least a slighty perormance
             
             8. I moved .Where() clause to top of the query

             9. I used the feature which was implemented in EF CORE 5.0 (Filter Include) 
                and I will move the part from belowe query which is Publish.Year < 1900 and 
                implement in Filter Include.

            10. I didn't implement but It is good advice to implement index in Published.Year,Country and Age 
                column in db and we can get better performance as well.
             
             */

            using var dbContext = new AppDbContext();

            var authors = dbContext.Authors
                                   .Include(x => x.Books.Where(b => b.Published.Year < 1900))
                                   .OrderByDescending(x => x.BooksCount)
                                   .Where(x =>
                                          x.Country == "Serbia" &&
                                          x.Age == 27)
                                   .Select(x => new AuthorDTO
                                   {
                                       UserFirstName = x.User.FirstName,
                                       UserLastName = x.User.LastName,
                                       UserEmail = x.User.Email,
                                       UserName = x.User.UserName,
                                       AllBooks = x.Books
                                       .Select(y => new BookDto
                                       {
                                           Id = y.Id,
                                           Name = y.Name,
                                           Published = y.Published,
                                       }).ToList(),
                                       AuthorAge = x.Age,
                                       AuthorCountry = x.Country,
                                       Id = x.Id
                                   })
                                        .Take(2)
                                        .ToList();
            return authors;
        }
    }
}