using Onstogram.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onstogram.DataLayer.SQL
{
    public class DataLayer : IDataLayer
    {
        private readonly string _connectionString;

        public DataLayer(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
        }

        public Comment AddCommentToImage(Guid imageId, Comment comment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    comment.Id = Guid.NewGuid();
                    command.CommandText = "insert into comments (id, [user id], [img id], text) values (@id, @uid, @iid, @text)";
                    command.Parameters.AddWithValue("@id", comment.Id);
                    command.Parameters.AddWithValue("@uid", comment.UserId);
                    command.Parameters.AddWithValue("@iid", imageId);
                    command.Parameters.AddWithValue("@text", comment.Text);
                    command.ExecuteNonQuery();
                    return comment;
                }
            }
        }

        public Image AddImage(Image image)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    image.Id = Guid.NewGuid();
                    command.CommandText = "insert into images (id, [user id], picture, time) values (@id, @uid, @picture, @time)";
                    command.Parameters.AddWithValue("@id", image.Id);
                    command.Parameters.AddWithValue("@uid", image.UserId);
                    command.Parameters.AddWithValue("@picture", image.Picture);
                    command.Parameters.AddWithValue("@time", image.Time);
                    command.ExecuteNonQuery();
                    return image;
                }
            }
        }

        public User AddUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    user.Id = Guid.NewGuid();
                    command.CommandText = "insert into users (id, nickname) values (@id, @nickname)";
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@nickname", user.NickName);
                    command.ExecuteNonQuery();
                    return user;
                }
            }
        }

        public void DeleteComment(Guid commentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "delete from comments where id = @id";
                    command.Parameters.AddWithValue("@id", commentId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteImage(Guid imageId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "delete from images where id = @id";
                    command.Parameters.AddWithValue("@id", imageId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Image GetImage(Guid imageId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, picture, time from images where id = @id";
                    command.Parameters.AddWithValue("@id", imageId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var img = new Image();
                            img.Id = reader.GetGuid(0);
                            img.Picture = new byte[((byte[])reader["picture"]).Length];
                            img.Picture = (byte[])reader["picture"];
                            img.Time = reader.GetDateTime(2);
                            return img;
                        } 
                        else
                        {
                            return null;
                        }
                    }

                }
            }
        }

        public Comment GetComment(Guid commentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, text, [img id], [user id] from comments where id = @id";
                    command.Parameters.AddWithValue("@id", commentId);
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        var comment = new Comment();
                        comment.Id = reader.GetGuid(0);
                        comment.Text = reader.GetString(1);
                        comment.ImgId = reader.GetGuid(2);
                        comment.UserId = reader.GetGuid(3);
                        return comment;
                    }

                }
            }
        }

        public List<Comment> GetImageComments(Guid imageId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, [user id], text from comments where [img id] = @iid";
                    command.Parameters.AddWithValue("@iid", imageId);
                    using (var reader = command.ExecuteReader())
                    {
                        List<Comment> comments = new List<Comment>();
                        while (reader.Read())
                        {
                            comments.Add(new Comment
                            {
                            Id = reader.GetGuid(0),
                            UserId = reader.GetGuid(1),
                            Text = reader.GetString(2),
                            ImgId = imageId 
                            });
                        }
                        return comments;
                    }

                }
            }
        }

        public List<Image> GetLastImages()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select TOP 10 id, picture, time from images order by time desc ";
                    using (var reader = command.ExecuteReader())
                    {
                        List<Image> images = new List<Image>();
                        while (reader.Read())
                        {
                            images.Add(new Image {
                            Id = reader.GetGuid(0),
                            Picture = (byte[])reader["picture"],
                            Time = reader.GetDateTime(2),
                            });
                        }
                        return images;
                    }

                }
            }
        }

        public User GetUser(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, nickname from users where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        return new User
                        {
                            Id = reader.GetGuid(0),
                            NickName = reader.GetString(1)
                        };
                    }
                    
                }
            }
        }

        public List<Image> GetImagesByHashtag(string tag)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select i.id, i.picture, i.time from images i inner join hashtgs_images h on  i.id = h.[img id]  inner join hashtags h2 on h2.text = @txt and h.[hastag id] = h2.id";
                    command.Parameters.AddWithValue("@txt", tag);
                    using (var reader = command.ExecuteReader())
                    {
                        List<Image> images = new List<Image>();
                        while (reader.Read())
                        {
                            images.Add(new Image
                            {
                                Id = reader.GetGuid(0),
                                Picture = (byte[])reader["picture"],
                                Time = reader.GetDateTime(2),
                            });
                        }
                        return images;
                    }

                }
            }
        }

        public void addHashTagToImage(Guid imageId, string tag)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                Guid id;
                using (var command = connection.CreateCommand())
                {
                    using (var idcommand = connection.CreateCommand())
                    {
                        idcommand.CommandText = "select id from hashtags where text = @txt";
                        idcommand.Parameters.AddWithValue("@txt", tag);

                        using (var reader = idcommand.ExecuteReader())
                        {
                            if (reader.Read())
                                id = reader.GetGuid(0);
                            else id = Guid.Empty;
                        }
                    }

                    if (id == Guid.Empty)
                    {
                        id = Guid.NewGuid();
                        using (var idcommand = connection.CreateCommand())
                        {
                            idcommand.CommandText = "insert into hashtags (id, text) values (@id, @txt)";
                            idcommand.Parameters.AddWithValue("@id", id);
                            idcommand.Parameters.AddWithValue("@txt", tag);
                            idcommand.ExecuteNonQuery();
                        }
                    }

                    command.CommandText = "insert into hashtgs_images ([img id], [hastag id]) values (@iid, @hid)";
                    command.Parameters.AddWithValue("@iid", imageId);
                    command.Parameters.AddWithValue("@hid", id);
                    command.ExecuteNonQuery();

                }
            }
        }
    }
}
