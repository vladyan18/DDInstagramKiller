using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Onstogram.Model;
using Onstogram.DataLayer.SQL;
using System.Collections.Generic;

namespace Onstogram.Tests
{
    [TestClass]
    public class DataLayerSQLTests
    {
        private const string _connectionString = "Data Source=I-PC\\SQLSERVER14;Initial Catalog=Onstogram;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        [TestMethod]
        public void ShouldAddUser()
        {
            // arrange
            var user = new User
            {
                NickName = "Test"
            };
            var dataLayer = new DataLayer.SQL.DataLayer(_connectionString);
            // act
            user = dataLayer.AddUser(user);
            // asserts
            var resultUser = dataLayer.GetUser(user.Id);
            Assert.AreEqual(user.NickName, resultUser.NickName);
        }

        [TestMethod]
        public void ShouldAddImage()
        {
            // arrange
            var image = new Image
            {
                Picture = new byte[1],
                UserId = Guid.Parse("F94A87D3-BBB1-46C9-A487-57FDE39FB38B"),
                Time = DateTime.Now
            };
            image.Time = new DateTime(image.Time.Year, image.Time.Month, image.Time.Day, image.Time.Hour, image.Time.Minute, image.Time.Second, image.Time.Kind); 

            var dataLayer = new DataLayer.SQL.DataLayer(_connectionString);
            // act
            image = dataLayer.AddImage(image);
            // asserts
            var resultImage = dataLayer.GetImage(image.Id);
            Assert.AreEqual(image.Time, resultImage.Time);
        }

        [TestMethod]
        public void ShouldDeleteImage()
        {
            // arrange
            var image = new Image
            {
                Picture = new byte[1],
                UserId = Guid.Parse("F94A87D3-BBB1-46C9-A487-57FDE39FB38B"),
                Time = DateTime.Now

            };
            image.Time = new DateTime(image.Time.Year, image.Time.Month, image.Time.Day, image.Time.Hour, image.Time.Minute, image.Time.Second, image.Time.Kind);

            var dataLayer = new DataLayer.SQL.DataLayer(_connectionString);
            // act
            image = dataLayer.AddImage(image);
            dataLayer.DeleteImage(image.Id);
            // asserts
            var resultImage = dataLayer.GetImage(image.Id);
            Assert.IsNull(resultImage);
        }

        [TestMethod]
        public void ShouldAddComment()
        {
            // arrange
            var comment = new Comment
            {
                Text = "AddTest",
                UserId = Guid.Parse("F94A87D3-BBB1-46C9-A487-57FDE39FB38B")
            };

            var dataLayer = new DataLayer.SQL.DataLayer(_connectionString);
            // act
            var image = dataLayer.GetImage(Guid.Parse("176DCFA9-FDB7-4FAF-91CB-7E38D0C5F77C"));
            comment = dataLayer.AddCommentToImage(image.Id, comment);
            // asserts
            var resultComment = dataLayer.GetComment(comment.Id);
            Assert.AreEqual(comment.Text, resultComment.Text);
        }

        [TestMethod]
        public void ShouldGetCommentsToImage()
        {
            // arrange
            var comment = new Comment
            {
                Text = Guid.NewGuid().ToString(),
                UserId = Guid.Parse("F94A87D3-BBB1-46C9-A487-57FDE39FB38B")
            };
            var dataLayer = new DataLayer.SQL.DataLayer(_connectionString);
            // act
            dataLayer.AddCommentToImage(Guid.Parse("176DCFA9-FDB7-4FAF-91CB-7E38D0C5F77C"), comment);
            List<Comment> comments = dataLayer.GetImageComments(Guid.Parse("176DCFA9-FDB7-4FAF-91CB-7E38D0C5F77C"));

            // asserts
            Assert.IsTrue(comments.Exists(x => x.Text == comment.Text));
        }

        [TestMethod]
        public void ShouldDeleteComment()
        {
            // arrange
            var comment = new Comment
            {
                Text = Guid.NewGuid().ToString(),
                UserId = Guid.Parse("F94A87D3-BBB1-46C9-A487-57FDE39FB38B")
            };

            var dataLayer = new DataLayer.SQL.DataLayer(_connectionString);
            // act
            var image = dataLayer.GetImage(Guid.Parse("176DCFA9-FDB7-4FAF-91CB-7E38D0C5F77C"));
            comment = dataLayer.AddCommentToImage(image.Id, comment);
            dataLayer.DeleteComment(comment.Id);
            // asserts
            List<Comment> resultComments = dataLayer.GetImageComments(image.Id);
            Assert.IsFalse(resultComments.Exists(x => x.Text == comment.Text));
        }

        [TestMethod]
        public void ShouldGetLastImages()
        {
            // arrange
            var image = new Image
            {
                Picture = new byte[1],
                UserId = Guid.Parse("F94A87D3-BBB1-46C9-A487-57FDE39FB38B"),
                Time = DateTime.Now

            };
            image.Time = new DateTime(image.Time.Year, image.Time.Month, image.Time.Day, image.Time.Hour, image.Time.Minute, image.Time.Second, image.Time.Kind);

            var dataLayer = new DataLayer.SQL.DataLayer(_connectionString);
            // act
            image = dataLayer.AddImage(image);
            List<Image> lastImages = dataLayer.GetLastImages();
            // asserts
            Assert.IsTrue(lastImages.Count > 0);
        }

        [TestMethod]
        public void ShouldGetImagesByHashtag()
        {
            // arrange
            var image = new Image
            {
                Picture = new byte[1],
                UserId = Guid.Parse("F94A87D3-BBB1-46C9-A487-57FDE39FB38B"),
                Time = DateTime.Now
            };
            image.Time = new DateTime(image.Time.Year, image.Time.Month, image.Time.Day, image.Time.Hour, image.Time.Minute, image.Time.Second, image.Time.Kind);

            var tag = Guid.NewGuid().ToString();
            var dataLayer = new DataLayer.SQL.DataLayer(_connectionString);
            // act
            image = dataLayer.AddImage(image);
            dataLayer.addHashTagToImage(image.Id, tag);

            List<Image> hashtagImages = dataLayer.GetImagesByHashtag(tag);
            // asserts
            Assert.IsTrue(hashtagImages.Count > 0);
        }
    }
}
