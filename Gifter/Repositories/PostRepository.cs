using Gifter.Models;
using Gifter.Utils;
using System.Data.SqlClient;

namespace Gifter.Repositories;

public class PostRepository : BaseRepository, IPostRepository
{
    public PostRepository(IConfiguration configuration) : base(configuration) { }

    public List<Post> GetAll(bool includeComments = false)
    {
        using var conn = Connection;
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT p.Id AS PostId
                                  ,p.Title
                                  ,p.Caption
                                  ,p.DateCreated AS PostDateCreated
                                  ,p.ImageUrl AS PostImageUrl
                                  ,p.UserProfileId AS PostUserProfileId
                                  ,up.Name
                                  ,up.Bio
                                  ,up.Email
                                  ,up.DateCreated AS UserProfileDateCreated
                                  ,up.ImageUrl AS UserProfileImageUrl
                                  ,c.Id AS CommentId
                                  ,c.Message
                                  ,c.UserProfileId AS CommentUserProfileId
                            FROM Post p
                            LEFT JOIN UserProfile up ON p.UserProfileId = up.id
                            LEFT JOIN Comment c on c.PostId = p.id
                            ORDER BY p.DateCreated";

        using var reader = cmd.ExecuteReader();

        var posts = new List<Post>();
        while (reader.Read())
        {
            var postId = DbUtils.GetInt(reader, "PostId");
            var existingPost = posts.FirstOrDefault(p => p.Id == postId);
            if (existingPost is null)
            {
                existingPost = new Post()
                {
                    Id = postId,
                    Title = DbUtils.GetString(reader, "Title"),
                    Caption = DbUtils.GetString(reader, "Caption"),
                    DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
                    ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
                    UserProfileId = DbUtils.GetInt(reader, "PostUserProfileId"),
                    UserProfile = new UserProfile()
                    {
                        Id = DbUtils.GetInt(reader, "PostUserProfileId"),
                        Name = DbUtils.GetString(reader, "Name"),
                        Email = DbUtils.GetString(reader, "Email"),
                        DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                        ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                    },
                    Comments = includeComments ? new List<Comment>() : null
                };

                posts.Add(existingPost);
            }

            if (includeComments && DbUtils.IsNotDbNull(reader, "CommentId"))
            {
                existingPost.Comments.Add(new Comment()
                {
                    Id = DbUtils.GetInt(reader, "CommentId"),
                    Message = DbUtils.GetString(reader, "Message"),
                    PostId = postId,
                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                });
            }

        }

        return posts;
    }

    public Post? GetById(int id, bool includeComments = false)
    {
        using var conn = Connection;
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT p.Id AS PostId
                                  ,p.Title
                                  ,p.Caption
                                  ,p.DateCreated AS PostDateCreated
                                  ,p.ImageUrl AS PostImageUrl
                                  ,p.UserProfileId AS PostUserProfileId
                                  ,up.Name
                                  ,up.Bio
                                  ,up.Email
                                  ,up.DateCreated AS UserProfileDateCreated
                                  ,up.ImageUrl AS UserProfileImageUrl
                                  ,c.Id AS CommentId
                                  ,c.Message
                                  ,c.UserProfileId AS CommentUserProfileId
                            FROM Post p
                            LEFT JOIN UserProfile up ON p.UserProfileId = up.id
                            LEFT JOIN Comment c on c.PostId = p.id
                            ORDER BY p.DateCreated
                            WHERE p.Id = @Id";

        DbUtils.AddParameter(cmd, "@Id", id);

        using var reader = cmd.ExecuteReader();
        Post? post = null;
        if (reader.Read())
        {
            int postId = DbUtils.GetInt(reader, "PostId");
            if (post is null)
            {
                post = new Post()
                {
                    Id = postId,
                    Title = DbUtils.GetString(reader, "Title"),
                    Caption = DbUtils.GetString(reader, "Caption"),
                    DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
                    ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
                    UserProfileId = DbUtils.GetInt(reader, "PostUserProfileId"),
                    UserProfile = new UserProfile()
                    {
                        Id = DbUtils.GetInt(reader, "PostUserProfileId"),
                        Name = DbUtils.GetString(reader, "Name"),
                        Email = DbUtils.GetString(reader, "Email"),
                        DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                        ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                    },
                    Comments = includeComments ? new List<Comment>() : null
                };
            }

            if (includeComments && DbUtils.IsNotDbNull(reader, "CommentId"))
            {
                post.Comments.Add(new Comment()
                {
                    Id = DbUtils.GetInt(reader, "CommentId"),
                    Message = DbUtils.GetString(reader, "Message"),
                    PostId = postId,
                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                });
            }
        }

        return post;
    }

    public bool Add(Post post)
    {
        using var conn = Connection;
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO Post
                              (Title
                              ,Caption
                              ,DateCreated
                              ,ImageUrl
                              ,UserProfileId)
                            OUTPUT INSERTED.ID
                            VALUES (@Title
                                   ,@Caption
                                   ,@DateCreated
                                   ,@ImageUrl
                                   ,@UserProfileId)";

        DbUtils.AddParameter(cmd, "@Title", post.Title);
        DbUtils.AddParameter(cmd, "@Caption", post.Caption);
        DbUtils.AddParameter(cmd, "@DateCreated", post.DateCreated);
        DbUtils.AddParameter(cmd, "@ImageUrl", post.ImageUrl);
        DbUtils.AddParameter(cmd, "@UserProfileId", post.UserProfileId);

        post.Id = (int)cmd.ExecuteScalar();
        return post.Id > 0;
    }

    public bool Update(Post post)
    {
        using var conn = Connection;
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"UPDATE Post
                            SET Title = @Title
                               ,Caption = @Caption
                               ,DateCreated = @DateCreated
                               ,ImageUrl = @ImageUrl
                               ,UserProfileId = @UserProfileId
                            WHERE Id = @Id";

        DbUtils.AddParameter(cmd, "@Title", post.Title);
        DbUtils.AddParameter(cmd, "@Caption", post.Caption);
        DbUtils.AddParameter(cmd, "@DateCreated", post.DateCreated);
        DbUtils.AddParameter(cmd, "@ImageUrl", post.ImageUrl);
        DbUtils.AddParameter(cmd, "@UserProfileId", post.UserProfileId);
        DbUtils.AddParameter(cmd, "@Id", post.Id);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using var conn = Connection;
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"DELETE FROM Post 
                            WHERE Id = @Id";
        DbUtils.AddParameter(cmd, "@id", id);

        return cmd.ExecuteNonQuery() > 0;
    }
}