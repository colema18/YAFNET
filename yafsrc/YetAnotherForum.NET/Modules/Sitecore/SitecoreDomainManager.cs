using System.Web.Security;

namespace YAF.Classes.Utils
{
  public class SitecoreDomainManager
  {
    private static readonly string _currentDomain = Sitecore.Context.Domain.Name;
    private const string _defaultDomain = "default";
    private static readonly string _anonymousUser = _currentDomain + "\\" + "Anonymous";

    /// <summary>
    /// Gets the fully qualified username including domain name for the username specified
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns></returns>
    public static string GetFullyQualifiedUserName(string userName)
    {
      if (userName.StartsWith(_defaultDomain + "\\"))
        return userName;
      if (userName.StartsWith(_currentDomain + "\\"))
        return userName;
      return _currentDomain + "\\" + userName;
    }

    /// <summary>
    /// Gets the name of the user, without the domain name
    /// </summary>
    /// <param name="fullName">The full name.</param>
    /// <returns></returns>
    public static string GetUserName(string fullName)
    {
      if (fullName.StartsWith(_currentDomain))
        return fullName.Remove(0, _currentDomain.Length + 1);
      if (fullName.StartsWith(_defaultDomain))
        return fullName.Remove(0, _defaultDomain.Length + 1);
      return fullName;
    }

    /// <summary>
    /// Determines whether the specified user is from an allowed domain
    /// </summary>
    /// <param name="fullName">The full name.</param>
    /// <returns>
    /// 	<c>true</c> if [is user in allowed domain] [the specified full name]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsUserInAllowedDomain(string fullName)
    {
      // User is not a sitecore domain user unless name contains "\"
      // If the user is not from Sitecore he is allowed
      if (!fullName.Contains("\\"))
        return true;
      // If user is from the current domain he is also allowed
      if (fullName.StartsWith(_currentDomain))
        return true;
      // User is from Sitecore but not part of the current domain. User is disallowed
      return false;
    }

    /// <summary>
    /// Gets the domain name from a fully qualified user name
    /// </summary>
    /// <param name="fullName">The full name.</param>
    /// <returns></returns>
    public static string GetDomainName(string fullName)
    {
      if (!fullName.Contains("\\"))
        return string.Empty;
      string[] userAndDomain = fullName.Split('\\');
      return userAndDomain[0];
    }

    /// <summary>
    /// Gets the fully qualified user name of the current domains Anonynous user.
    /// </summary>
    /// <value>The anonymous.</value>
    public static string Anonymous
    {
      get { return _anonymousUser; }
    }

    /// <summary>
    /// Gets the anonymous user from the current domain.
    /// </summary>
    /// <value>The anonymous user.</value>
    public static SitecoreMembershipUser AnonymousUser
    {
      get
      {
        return new SitecoreMembershipUser(Membership.GetUser( Anonymous ));
      }
    }


  }
}
