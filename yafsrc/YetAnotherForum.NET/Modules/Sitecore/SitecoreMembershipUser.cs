using System;
using System.Data;
using System.Web.Security;
using Sitecore;
using Sitecore.Diagnostics;

namespace YAF.Classes.Utils
{
  public class SitecoreMembershipUser : MembershipUser
  {
    private MembershipUser _innerUser;
    private readonly string _fullName;
    private readonly string _providerName;

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreMembershipUser"/> class.
    /// </summary>
    /// <param name="innerUser">The inner user.</param>
    public SitecoreMembershipUser([NotNull] MembershipUser innerUser) : this(innerUser, innerUser.UserName, innerUser.ProviderName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreMembershipUser"/> class.
    /// </summary>
    /// <param name="innerUser">The inner user.</param>
    /// <param name="fullName">The full name.</param>
    /// <param name="providerName">Name of the provider.</param>
    public SitecoreMembershipUser([NotNull] MembershipUser innerUser, [NotNull] string fullName, [NotNull] string providerName) 
    {
      Assert.ArgumentNotNull(innerUser, "innerUser");
      Assert.ArgumentNotNullOrEmpty(fullName, "fullName");
      Assert.ArgumentNotNull(providerName, "providerName");
      
      _innerUser = innerUser;
      _fullName = fullName;
      _providerName = providerName;
    }

    /// <summary>
    /// Gets or sets application-specific information for the membership user.
    /// </summary>
    /// <value></value>
    /// <returns>Application-specific information for the membership user.</returns>
    [CanBeNull]
    [AllowNull("value")]
    public override string Comment 
    {
      get 
      {
        return _innerUser.Comment;
      }
      set 
      {
        _innerUser.Comment = value;
      }
    }

    /// <summary>
    /// Gets the date and time when the user was added to the membership data store.
    /// </summary>
    /// <value></value>
    /// <returns>The date and time when the user was added to the membership data store. </returns>
    public override DateTime CreationDate 
    {
      get 
      {
        return _innerUser.CreationDate;
      }
    }

    /// <summary>
    /// Gets or sets the e-mail address for the membership user.
    /// </summary>
    /// <value></value>
    /// <returns>The e-mail address for the membership user.</returns>
    [CanBeNull]
    [AllowNull("value")]
    public override string Email 
    {
      get 
      {
        return _innerUser.Email;
      }
      set 
      {
        _innerUser.Email = value;
      }
    }

    /// <summary>
    /// Gets the inner user.
    /// </summary>
    /// <value>The inner user.</value>
    [NotNull]
    public virtual MembershipUser InnerUser 
    {
      get 
      {
        return _innerUser;
      }
    }

    /// <summary>
    /// Gets or sets whether the membership user can be authenticated.
    /// </summary>
    /// <value></value>
    /// <returns>true if the user can be authenticated; otherwise, false.</returns>
    public override bool IsApproved 
    {
      get 
      {
        return _innerUser.IsApproved;
      }
      set 
      {
        _innerUser.IsApproved = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the membership user is locked out and unable to be validated.
    /// </summary>
    /// <value></value>
    /// <returns>true if the membership user is locked out and unable to be validated; otherwise, false.</returns>
    public override bool IsLockedOut 
    {
      get 
      {
        return _innerUser.IsLockedOut;
      }
    }

    /// <summary>
    /// Gets or sets the date and time when the membership user was last authenticated or accessed the application.
    /// </summary>
    /// <value></value>
    /// <returns>The date and time when the membership user was last authenticated or accessed the application.</returns>
    public override DateTime LastActivityDate 
    {
      get 
      {
        return _innerUser.LastActivityDate;
      }
      set 
      {
        _innerUser.LastActivityDate = value;
      }
    }

    /// <summary>
    /// Gets the most recent date and time that the membership user was locked out.
    /// </summary>
    /// <value></value>
    /// <returns>A <see cref="T:System.DateTime"/> object that represents the most recent date and time that the membership user was locked out.</returns>
    public override DateTime LastLockoutDate 
    {
      get 
      {
        return _innerUser.LastLockoutDate;
      }
    }

    /// <summary>
    /// Gets or sets the date and time when the user was last authenticated.
    /// </summary>
    /// <value></value>
    /// <returns>The date and time when the user was last authenticated.</returns>
    public override DateTime LastLoginDate 
    {
      get 
      {
        return _innerUser.LastLoginDate;
      }
      set 
      {
        _innerUser.LastLoginDate = value;
      }
    }

    /// <summary>
    /// Gets the date and time when the membership user's password was last updated.
    /// </summary>
    /// <value></value>
    /// <returns>The date and time when the membership user's password was last updated.</returns>
    public override DateTime LastPasswordChangedDate 
    {
      get 
      {
        return _innerUser.LastPasswordChangedDate;
      }
    }

    /// <summary>
    /// Gets the password question for the membership user.
    /// </summary>
    /// <value></value>
    /// <returns>The password question for the membership user.</returns>
    [CanBeNull]
    public override string PasswordQuestion 
    {
      get 
      {
        return _innerUser.PasswordQuestion;
      }
    }

    /// <summary>
    /// Gets the name of the membership provider that stores and retrieves user information for the membership user.
    /// </summary>
    /// <value></value>
    /// <returns>The name of the membership provider that stores and retrieves user information for the membership user.</returns>
    [NotNull]
    public override string ProviderName 
    {
      get 
      {
        return _providerName;
      }
    }

    /// <summary>
    /// Gets the user identifier from the membership data source for the user.
    /// </summary>
    /// <value></value>
    /// <returns>The user identifier from the membership data source for the user.</returns>
    [CanBeNull]
    public override object ProviderUserKey 
    {
      get 
      {
        return _innerUser.ProviderUserKey;
      }
    }

    /// <summary>
    /// Gets the logon name of the membership user.
    /// </summary>
    /// <value></value>
    /// <returns>The logon name of the membership user.</returns>
    [NotNull]
    public override string UserName 
    {
      get 
      {
        return SitecoreDomainManager.GetUserName(_fullName);
      }
    }

    /// <summary>
    /// Updates the password for the membership user in the membership data store.
    /// </summary>
    /// <param name="oldPassword">The current password for the membership user.</param>
    /// <param name="newPassword">The new password for the membership user.</param>
    /// <returns>
    /// true if the update was successful; otherwise, false.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    /// 	<paramref name="oldPassword"/> is an empty string.-or-<paramref name="newPassword"/> is an empty string.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// 	<paramref name="oldPassword"/> is null.-or-<paramref name="newPassword"/> is null.</exception>
    [CanBeNull]
    [AllowNull("*")]
    public override bool ChangePassword(string oldPassword, string newPassword) 
    {
      Assert.IsNotNull(oldPassword, "oldPassword");
      Assert.IsNotNull(newPassword, "newPassword");

      MembershipProvider provider = Membership.Providers[this.ProviderName];

      if (provider == null)
      {
        return false;
      }

      if (!provider.ChangePassword(UserName, oldPassword, newPassword))
      {
        return false;
      }

      UpdateInnerUser();

      return true;
    }

    /// <summary>
    /// Updates the password question and answer for the membership user in the membership data store.
    /// </summary>
    /// <param name="password">The current password for the membership user.</param>
    /// <param name="newPasswordQuestion">The new password question value for the membership user.</param>
    /// <param name="newPasswordAnswer">The new password answer value for the membership user.</param>
    /// <returns>
    /// true if the update was successful; otherwise, false.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    /// 	<paramref name="password"/> is an empty string.-or-<paramref name="newPasswordQuestion"/> is an empty string.-or-<paramref name="newPasswordAnswer"/> is an empty string.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// 	<paramref name="password"/> is null.</exception>
    [CanBeNull]
    [AllowNull("*")]
    public override bool ChangePasswordQuestionAndAnswer(string password, string newPasswordQuestion, string newPasswordAnswer) 
    {
      return _innerUser.ChangePasswordQuestionAndAnswer(password, newPasswordQuestion, newPasswordAnswer);
    }

    /// <summary>
    /// Gets the password for the membership user from the membership data store.
    /// </summary>
    /// <returns>The password for the membership user.</returns>
    [CanBeNull]
    public override string GetPassword() 
    {
      return _innerUser.GetPassword();
    }

    /// <summary>
    /// Gets the password for the membership user from the membership data store.
    /// </summary>
    /// <param name="passwordAnswer">The password answer for the membership user.</param>
    /// <returns>The password for the membership user.</returns>
    [CanBeNull]
    [AllowNull("*")]
    public override string GetPassword(string passwordAnswer) 
    {
      return _innerUser.GetPassword(passwordAnswer);
    }

    /// <summary>
    /// Resets a user's password to a new, automatically generated password.
    /// </summary>
    /// <param name="passwordAnswer">The password answer for the membership user.</param>
    /// <returns>
    /// The new password for the membership user.
    /// </returns>
    [CanBeNull]
    [AllowNull("*")]
    public override string ResetPassword(string passwordAnswer) 
    {
      return _innerUser.ResetPassword(passwordAnswer);
    }

    /// <summary>
    /// Resets a user's password to a new, automatically generated password.
    /// </summary>
    /// <returns>
    /// The new password for the membership user.
    /// </returns>
    [CanBeNull]
    public override string ResetPassword() 
    {
      return _innerUser.ResetPassword();
    }

    /// <summary>
    /// Returns the user name for the membership user.
    /// </summary>
    /// <returns>
    /// The <see cref="P:System.Web.Security.MembershipUser.UserName"/> for the membership user.
    /// </returns>
    [CanBeNull]
    public override string ToString() 
    {
      return _innerUser.ToString();
    }

    /// <summary>
    /// Clears the locked-out state of the user so that the membership user can be validated.
    /// </summary>
    /// <returns>
    /// true if the membership user was successfully unlocked; otherwise, false.
    /// </returns>
    public override bool UnlockUser() 
    {
      return _innerUser.UnlockUser();
    }

    /// <summary>
    /// Updates the inner user.
    /// </summary>
    private void UpdateInnerUser()
    {
      MembershipProvider provider = Membership.Providers[this._innerUser.ProviderName];

      if (provider == null)
      {
        return;
      }

      MembershipUser user = provider.GetUser(this._innerUser.UserName, false);

      if (user == null)
      {
        return;
      }

      this._innerUser = user;
    }

    /// <summary>
    /// Gets the first found forum administrator.
    /// </summary>
    /// <value>The forum admin.</value>
    public static SitecoreMembershipUser ForumAdmin
    {
      get
      {
        DataTable table = YAF.Classes.Data.LegacyDb.user_list(1, null, null, null, 1);
        if (table.Rows.Count == 0)
          throw new System.Security.SecurityException("No forum administrator was found in the YAF database");
        string adminName = table.Rows[0]["Name"] as string;
        return new SitecoreMembershipUser(Membership.GetUser(SitecoreDomainManager.GetFullyQualifiedUserName(adminName)));
      }
    }
    

  }
}
