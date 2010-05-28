/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Utilities
{
  using System;
  using CookComputing.XmlRpc;

  /// <summary>
  /// The meta weblog.
  /// </summary>
  public class MetaWeblog : XmlRpcClientProtocol
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MetaWeblog"/> class.
    /// </summary>
    /// <param name="metaWeblogServiceUrl">
    /// The meta weblog service url.
    /// </param>
    public MetaWeblog(string metaWeblogServiceUrl)
    {
      Url = metaWeblogServiceUrl;
    }

    /// <summary>
    /// Posts a new entry to a blog.
    /// </summary>
    /// <param name="blogid">
    /// The blogid, not sure if this is needed.  I know subtext doesn't need it, but not sure of others.
    /// </param>
    /// <param name="username">
    /// The name the user uses to login
    /// </param>
    /// <param name="password">
    /// The user�s password.
    /// </param>
    /// <param name="content">
    /// The content.
    /// </param>
    /// <param name="publish">
    /// If false, this is a draft post.
    /// </param>
    /// <returns>
    /// The postid of the newly-created post.
    /// </returns>
    [XmlRpcMethod("metaWeblog.newPost")]
    public string newPost(string blogid, string username, string password, Post content, bool publish)
    {
      // TODO: We'll most likely want to keep the returned postid with the message that's posted to the forum.
      // That way, if the user edits/deletes we can also make the appropriate change to their blog as well. See
      // editPost and deletePost method's below.
      return (string) Invoke(
                        "newPost", 
                        new object[]
                          {
                            blogid, username, password, content, publish
                          });
    }

    /// <summary>
    /// Posts a new entry to a blog.
    /// </summary>
    /// <param name="blogid">
    /// The blogid, not sure if this is needed.  I know subtext doesn't need it, but not sure of others.
    /// </param>
    /// <param name="username">
    /// The name the user uses to login
    /// </param>
    /// <param name="password">
    /// The user�s password.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// The postid of the newly-created post.
    /// </returns>
    [XmlRpcMethod("metaWeblog.newPost")]
    public string newPost(string blogid, string username, string password, string subject, string message)
    {
      var post = new Post();
      post.title = subject;
      post.description = message;
      post.dateCreated = DateTime.UtcNow;
      return newPost(blogid, username, password, post, true);
    }

    /// <summary>
    /// 
    /// Edits an existing entry on a blog. 
    /// </summary>
    /// <param name="postid">
    /// The ID of the post to update.
    /// </param>
    /// <param name="username">
    /// Username to login to the blog
    /// </param>
    /// <param name="password">
    /// Password to login to the blog
    /// </param>
    /// <param name="content">
    /// The content.
    /// </param>
    /// <param name="publish">
    /// If false, this is a draft post.
    /// </param>
    /// <returns>
    /// Always returns true.
    /// </returns>
    [XmlRpcMethod("metaWeblog.editPost")]
    public bool editPost(string postid, string username, string password, Post content, bool publish)
    {
      return (bool) Invoke(
                      "editPost", 
                      new object[]
                        {
                          postid, username, password, content, publish
                        });
    }

    /// <summary>
    /// 
    /// Edits an existing entry on a blog. 
    /// </summary>
    /// <param name="postid">
    /// The ID of the post to update.
    /// </param>
    /// <param name="username">
    /// Username to login to the blog
    /// </param>
    /// <param name="password">
    /// Password to login to the blog
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public void editPost(string postid, string username, string password, string subject, string message)
    {
      Post post = getPost(postid, username, password);
      post.title = subject;
      post.description = message;
      editPost(postid, username, password, post, true);
    }

    /// <summary>
    /// 
    /// Get a specific entry from the blog. 
    /// </summary>
    /// <param name="postid">
    /// The ID of the post to get. 
    /// </param>
    /// <param name="username">
    /// Username to login to the blog
    /// </param>
    /// <param name="password">
    /// Password to login to the blog
    /// </param>
    /// <returns>
    /// Returns a specific entry from a blog.
    /// </returns>
    [XmlRpcMethod("metaWeblog.getPost")]
    public Post getPost(string postid, string username, string password)
    {
      return (Post) Invoke(
                      "getPost", 
                      new object[]
                        {
                          postid, username, password
                        });
    }

    #region Don't think we'll need this, but what the Heck

    /// <summary>
    /// 
    /// Deletes a post from the blog. 
    /// </summary>
    /// <param name="appKey">
    /// This value is ignored.
    /// </param>
    /// <param name="postid">
    /// The ID of the post to update.
    /// </param>
    /// <param name="username">
    /// Username to login to the blog
    /// </param>
    /// <param name="password">
    /// Password to login to the blog
    /// </param>
    /// <param name="publish">
    /// This value is ignored.
    /// </param>
    /// <returns>
    /// Always returns true.
    /// </returns>
    [XmlRpcMethod("blogger.deletePost")]
    public bool deletePost(string appKey, string postid, string username, string password, bool publish)
    {
      throw new Exception("Not yet implemented");

      // return (bool)this.Invoke("deletePost", new object[] { appKey, postid, username, password, publish });
    }

    #endregion

    #region Nested type: Category

    /// <summary>
    /// 
    /// This struct represents the information about a category
    /// </summary>
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct Category
    {
      /// <summary>
      /// The description.
      /// </summary>
      public string description;

      /// <summary>
      /// The title.
      /// </summary>
      public string title;
    }

    #endregion

    #region Nested type: Post

    /// <summary>
    /// 
    /// This struct represents the information about a post
    /// </summary>
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct Post
    {
      /// <summary>
      /// The categories.
      /// </summary>
      [XmlRpcMember("categories", Description = "Contains categories for the post.")] public string[] categories;

      /// <summary>
      /// The date created.
      /// </summary>
      public DateTime dateCreated;

      /// <summary>
      /// The description.
      /// </summary>
      [XmlRpcMissingMapping(MappingAction.Error)] [XmlRpcMember(Description = "Required when posting.")] public string description;

      /// <summary>
      /// The link.
      /// </summary>
      public string link;

      /// <summary>
      /// The permalink.
      /// </summary>
      public string permalink;

      /// <summary>
      /// The postid.
      /// </summary>
      [XmlRpcMember(
        Description =
          "Not required when posting. Depending on server may " + "be either string or integer. " + "Use Convert.ToInt32(postid) to treat as integer or " +
          "Convert.ToString(postid) to treat as string")] public object postid;

      /// <summary>
      /// The title.
      /// </summary>
      [XmlRpcMissingMapping(MappingAction.Error)] [XmlRpcMember(Description = "Required when posting.")] public string title;

      /// <summary>
      /// The userid.
      /// </summary>
      public string userid;
    }

    #endregion
  }
}