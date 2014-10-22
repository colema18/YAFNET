/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System.Collections.Generic;
using Sitecore.Security.Accounts;

namespace YAF.Core
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The role membership helper.
    /// </summary>
    public static class RoleMembershipHelper
    {
        #region Public Methods

        /// <summary>
        /// The add user to role.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="role">
        /// The role.
        /// </param>
        public static void AddUserToRole([NotNull] string username, [NotNull] string role)
        {
            YafContext.Current.Get<RoleProvider>().AddUsersToRoles(new[] { username }, new[] { role });
        }

        /// <summary>
        /// Creates the user in the YAF DB from the ASP.NET Membership user information.
        ///   Also copies the Roles as groups into YAF DB for the current user
        /// </summary>
        /// <param name="user">
        /// Current Membership User
        /// </param>
        /// <param name="pageBoardID">
        /// Current BoardID
        /// </param>
        /// <returns>
        /// Returns the UserID of the user if everything was successful. Otherwise, null.
        /// </returns>
        public static int? CreateForumUser([NotNull] MembershipUser user, int pageBoardID)
        {
            return CreateForumUser(user, user.UserName, pageBoardID);
        }

        /// <summary>
        /// Creates the user in the YAF DB from the ASP.NET Membership user information.
        ///   Also copies the Roles as groups into YAF DB for the current user
        /// </summary>
        /// <param name="user">
        /// Current Membership User
        /// </param>
        /// <param name="displayName">
        /// The display Name.
        /// </param>
        /// <param name="pageBoardID">
        /// Current BoardID
        /// </param>
        /// <returns>
        /// Returns the UserID of the user if everything was successful. Otherwise, null.
        /// </returns>
        public static int? CreateForumUser([NotNull] MembershipUser user, [NotNull] string displayName, int pageBoardID)
        {
            int? userID = null;

            try
            {
                userID = LegacyDb.user_aspnet(
                    pageBoardID,
                    user.UserName,
                    displayName,
                    user.Email,
                    user.UserName,
                    user.IsApproved);

                foreach (string role in GetRolesForUser(user.UserName))
                {
                    LegacyDb.user_setrole(pageBoardID, user.UserName, role);
                }

                // YAF.Classes.Data.DB.eventlog_create(DBNull.Value, user, string.Format("Created forum user {0}", user.UserName));
            }
            catch (Exception x)
            {
                YafContext.Current.Get<ILogger>().Error(x, "Error in CreateForumUser");
            }

            return userID;
        }

        /// <summary>
        /// The create role.
        /// </summary>
        /// <param name="roleName">
        /// The role name.
        /// </param>
        public static void CreateRole([NotNull] string roleName)
        {
            try
            {
                //MEAU: Won't have rights to create roles in AD so Try Catching This
                YafContext.Current.Get<RoleProvider>().CreateRole(roleName);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// The delete role.
        /// </summary>
        /// <param name="roleName">
        /// The role name.
        /// </param>
        /// <param name="throwOnPopulatedRole">
        /// The throw on populated role.
        /// </param>
        public static void DeleteRole([NotNull] string roleName, bool throwOnPopulatedRole)
        {
            YafContext.Current.Get<RoleProvider>().DeleteRole(roleName, throwOnPopulatedRole);
        }

        /// <summary>
        /// Check if the forum user was created.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="pageBoardID">The page board ID.</param>
        /// <returns>
        /// The did create forum user.
        /// </returns>
        public static bool DidCreateForumUser([NotNull] MembershipUser user, int pageBoardID)
        {
            int? userID = CreateForumUser(user, pageBoardID);
            return userID != null;
        }

        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <returns>
        /// Returns all Roles
        /// </returns>
        public static string[] GetAllRoles()
        {
            return YafContext.Current.Get<RoleProvider>().GetAllRoles();
        }

        /// <summary>
        /// Gets the roles for the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>
        /// Returns all Roles
        /// </returns>
        public static string[] GetRolesForUser([NotNull] string username)
        {
            return (from role in YafContext.Current.Get<RoleProvider>().GetRolesForUser(username)
                    where role != null && role.Contains("\\")
                    select role.Substring(role.IndexOf("\\") + 1)).ToArray();
        }

        /// <summary>
        /// Groups the in group table.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        /// <param name="groupTable">The group table.</param>
        /// <returns>
        /// The group in group table.
        /// </returns>
        public static bool GroupInGroupTable([NotNull] string groupName, [NotNull] DataTable groupTable)
        {
            return
                groupTable.AsEnumerable()
                    .Any(row => row["Member"].ToType<int>() == 1 && row["Name"].ToType<string>() == groupName);
        }

        /// <summary>
        /// Determines whether [is user in role] [the specified username].
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="role">The role.</param>
        /// <returns>
        /// The is user in role.
        /// </returns>
        public static bool IsUserInRole([NotNull] string username, [NotNull] string role)
        {
            return YafContext.Current.Get<RoleProvider>().IsUserInRole(username, role);
        }

        /// <summary>
        /// The remove user from role.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="role">
        /// The role.
        /// </param>
        public static void RemoveUserFromRole([NotNull] string username, [NotNull] string role)
        {
            YafContext.Current.Get<RoleProvider>().RemoveUsersFromRoles(new[] { username }, new[] { role });
        }

        /// <summary>
        /// Roles the exists.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>
        /// The role exists.
        /// </returns>
        public static bool RoleExists([NotNull] string roleName)
        {
            return YafContext.Current.Get<RoleProvider>().RoleExists(roleName);
        }

        /// <summary>
        /// Roles the in role array.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <param name="roleArray">The role array.</param>
        /// <returns>
        /// The role in role array.
        /// </returns>
        public static bool RoleInRoleArray([NotNull] string roleName, [NotNull] string[] roleArray)
        {
            return roleArray.Any(role => role == roleName);
        }

        /// <summary>
        /// Sets up the user roles from the "start" settings for a given group/role
        /// </summary>
        /// <param name="pageBoardID">Current BoardID</param>
        /// <param name="userName">Name of the user.</param>
        public static void SetupUserRoles(int pageBoardID, [NotNull] string userName)
        {
            using (DataTable dt = LegacyDb.group_list(pageBoardID, DBNull.Value))
            {
                foreach (string roleName in (from DataRow row in dt.Rows
                                             let roleFlags = new GroupFlags(row["Flags"])
                                             where roleFlags.IsStart && !roleFlags.IsGuest
                                             select row["Name"].ToString()
                                             into roleName
                                             where roleName.IsSet()
                                             select roleName).Where(roleName => !IsUserInRole(userName, roleName)))
                {
                    AddUserToRole(userName, roleName);
                }
            }
        }

        /// <summary>
        /// Goes through every membership user and manually "syncs" them to the forum.
        ///   Best for an existing membership structure -- will migrate all users at once 
        ///   rather then one at a time...
        /// </summary>
        /// <param name="pageBoardId">
        /// The page Board Id.
        /// </param>
        public static void SyncAllMembershipUsers(int pageBoardId)
        {
            int totalRecords;

            // get all users in membership...
            var users =
                YafContext.Current.Get<MembershipProvider>()
                    .GetAllUsers(0, 999999, out totalRecords)
                    .Cast<MembershipUser>()
                    .Where(u => u != null && u.Email.IsSet());

            // create/update users...
            Parallel.ForEach(users, user => UpdateForumUser(user, pageBoardId));
        }

        /// <summary>
        /// Syncs the ASP.NET roles with YAF group based on YAF (not bi-directional)
        /// </summary>
        /// <param name="pageBoardID">The page board ID.</param>
        public static void SyncRoles(int pageBoardID)
        {
            // get all the groups in YAF DB and create them if they do not exist as a role in membership
            using (DataTable dt = LegacyDb.group_list(pageBoardID, DBNull.Value))
            {
                foreach (var name in from DataRow row in dt.Rows
                                     let name = (string)row["Name"]
                                     let roleFlags = new GroupFlags(row["Flags"])
                                     where name.IsSet() && !roleFlags.IsGuest && !RoleExists(name)
                                     select name)
                {
                    CreateRole(name);
                }
            }
        }

        /// <summary>
        /// Takes all YAF users and creates them in the Membership Provider
        /// </summary>
        /// <param name="pageBoardID">The page board ID.</param>
        public static void SyncUsers(int pageBoardID)
        {
            // first sync unapproved users...
            using (DataTable dt = LegacyDb.user_list(pageBoardID, DBNull.Value, false))
            {
                MigrateUsersFromDataTable(false, dt);
            }

            // then sync approved users...
            using (DataTable dt = LegacyDb.user_list(pageBoardID, DBNull.Value, true))
            {
                MigrateUsersFromDataTable(true, dt);
            }
        }

        /// <summary>
        /// Updates the information in the YAF DB from the ASP.NET Membership user information.
        /// Called once per session for a user to sync up the data
        /// </summary>
        /// <param name="user">Current Membership User</param>
        /// <param name="pageBoardID">Current BoardID</param>
        /// <param name="roles">The DNN user roles.</param>
        /// <returns>
        /// The update forum user.
        /// </returns>
        public static int? UpdateForumUser([NotNull] MembershipUser user, int pageBoardID, string[] roles = null)
        {
            if (user == null)
            {
                // Check to make sure its not a guest
                return null;
            }
            //MEAU: force ProvideruserKey to be username to allow for switcher - AD and Sitecore
            var userKeyString = user.ProviderUserKey == null ? null : user.UserName;

            int userId = UserMembershipHelper.GetUserIDFromProviderUserKey(userKeyString);

            if (userId == UserMembershipHelper.GuestUserId)
            {
                return userId;
            }

            if (user.ProviderUserKey == null)
            {
                // problem -- log and move on...
                YafContext.Current.Get<ILogger>()
                    .Log(
                        userId,
                        "UpdateForumUser",
                        "Null User Provider Key for UserName {0}. Please check your provider key settings for your ASP.NET membership provider."
                            .FormatWith(user.UserName));

                return userId;
            }

            // is this a new user?
            bool isNewUser = userId <= 0;

            userId = LegacyDb.user_aspnet(
                pageBoardID,
                user.UserName,
                null,
                user.Email,
                userKeyString,
                user.IsApproved);

            // get user groups...
            DataTable groupTable = LegacyDb.group_member(pageBoardID, userId);
            string[] userRoles = GetRolesForUser(user.UserName);

            if (Config.IsDotNetNuke && roles != null)
            {
                userRoles = roles;
            }

            if (Config.IsMojoPortal)
            {
                string roles1 = userRoles.Where(t => !string.IsNullOrEmpty(t))
                    .Aggregate(string.Empty, (current, t) => current.Trim() + "," + t.Trim());
                userRoles = roles1.Trim(',').Split(',');
            }

            // add groups...
            foreach (string role in userRoles.Where(role => !GroupInGroupTable(role, groupTable)))
            {
                // add the role...
                LegacyDb.user_setrole(pageBoardID, userKeyString, role);
            }

            // remove groups...
            foreach (DataRow row in
                groupTable.AsEnumerable().Where(row => !userRoles.Contains(row["Name"].ToString())))
            {
                // remove since there is no longer an association in the membership...
                LegacyDb.usergroup_save(userId, row["GroupID"], 0);
            }

            if (!isNewUser || userId <= 0)
            {
                return userId;
            }

            try
            {
                UserNotificationSetting defaultNotificationSetting =
                    YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting;

                bool defaultSendDigestEmail = YafContext.Current.Get<YafBoardSettings>().DefaultSendDigestEmail;

                // setup default notifications...
                bool autoWatchTopicsEnabled = defaultNotificationSetting
                                              == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

                // save the settings...
                LegacyDb.user_savenotification(
                    userId,
                    true,
                    autoWatchTopicsEnabled,
                    defaultNotificationSetting,
                    defaultSendDigestEmail);
            }
            catch (Exception ex)
            {
                YafContext.Current.Get<ILogger>()
                    .Log(
                        userId,
                        "UpdateForumUser",
                        "Failed to save default notifications for new user: {0}".FormatWith(ex));
            }

            return userId;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Migrates the create user.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <param name="question">The question.</param>
        /// <param name="answer">The answer.</param>
        /// <param name="approved">The approved.</param>
        /// <param name="user">The user.</param>
        /// <returns>
        /// The migrate create user.
        /// </returns>
        private static MembershipCreateStatus MigrateCreateUser(
            [NotNull] string name,
            [NotNull] string email,
            [NotNull] string question,
            [NotNull] string answer,
            bool approved,
            [NotNull] out MembershipUser user)
        {
            MembershipCreateStatus status;

            // create a new user and generate a password.
            int retry = 0;
            do
            {
                string password = Membership.GeneratePassword(7 + retry, 1 + retry);
                user = YafContext.Current.Get<MembershipProvider>()
                    .CreateUser(name, password, email, question, answer, approved, null, out status);
            }
            while (status == MembershipCreateStatus.InvalidPassword && ++retry < 10);

            return status;
        }

        /// <summary>
        /// The migrate users from <paramref name="dataTable"/>.
        /// </summary>
        /// <param name="approved">
        /// The approved.
        /// </param>
        /// <param name="dataTable">
        /// The dataTable.
        /// </param>
        private static void MigrateUsersFromDataTable(bool approved, [NotNull] DataTable dataTable)
        {
            // is this the Yaf membership provider?
            bool isYafProvider = YafContext.Current.Get<MembershipProvider>().Name == "YafMembershipProvider";
            bool isLegacyYafDB = dataTable.Columns.Contains("Location");

            var userRows = dataTable.AsEnumerable();

            Parallel.ForEach(
                userRows,
                row =>
                    {
                        // skip the guest user
                        if (row.Field<bool>("IsGuest"))
                        {
                            return;
                        }

                        // validate that name and email are available...
                        if (row["Name"].IsNullOrEmptyDBField() || row["Email"].IsNullOrEmptyDBField())
                        {
                            return;
                        }

                        string name = row.Field<string>("Name").Trim();
                        string email = row.Field<string>("Email").ToLower().Trim();

                        // clean up the name by removing commas...
                        name = name.Replace(",", string.Empty);

                        // verify this user & email are not empty
                        if (!name.IsSet() || !email.IsSet())
                        {
                            return;
                        }

                        MembershipUser user = UserMembershipHelper.GetUser(name, false);

                        if (user == null)
                        {
                            MembershipCreateStatus status = MigrateCreateUser(
                                name,
                                email,
                                "Your email in all lower case",
                                email,
                                approved,
                                out user);

                            if (status != MembershipCreateStatus.Success)
                            {
                                YafContext.Current.Get<ILogger>()
                                    .Log(0, "MigrateUsers", "Failed to create user {0}: {1}".FormatWith(name, status));
                            }
                            else
                            {
                                // update the YAF table with the ProviderKey -- update the provider table if this is the YAF provider...
                                LegacyDb.user_migrate(row["UserID"], user.UserName, isYafProvider);

                                user.Comment = "Migrated from YetAnotherForum.NET";

                                YafContext.Current.Get<MembershipProvider>().UpdateUser(user);

                                if (!isYafProvider)
                                {
                                    /* Email generated password to user
                            System.Text.StringBuilder msg = new System.Text.StringBuilder();
                            msg.AppendFormat( "Hello {0}.\r\n\r\n", name );
                            msg.AppendFormat( "Here is your new password: {0}\r\n\r\n", password );
                            msg.AppendFormat( "Visit {0} at {1}", ForumName, ForumURL );

                            YAF.Classes.Data.DB.mail_create( ForumEmail, user.Email, "Forum Upgrade", msg.ToString() );
                            */
                                }
                            }

                            if (isLegacyYafDB)
                            {
                                // copy profile data over...
                                YafUserProfile userProfile = YafUserProfile.GetProfile(name);
                                if (dataTable.Columns.Contains("AIM") && !row["AIM"].IsNullOrEmptyDBField())
                                {
                                    userProfile.AIM = row["AIM"].ToString();
                                }

                                if (dataTable.Columns.Contains("YIM") && !row["YIM"].IsNullOrEmptyDBField())
                                {
                                    userProfile.YIM = row["YIM"].ToString();
                                }

                                if (dataTable.Columns.Contains("MSN") && !row["MSN"].IsNullOrEmptyDBField())
                                {
                                    userProfile.MSN = row["MSN"].ToString();
                                }

                                if (dataTable.Columns.Contains("ICQ") && !row["ICQ"].IsNullOrEmptyDBField())
                                {
                                    userProfile.ICQ = row["ICQ"].ToString();
                                }

                                if (dataTable.Columns.Contains("RealName") && !row["RealName"].IsNullOrEmptyDBField())
                                {
                                    userProfile.RealName = row["RealName"].ToString();
                                }

                                if (dataTable.Columns.Contains("Occupation")
                                    && !row["Occupation"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Occupation = row["Occupation"].ToString();
                                }

                                if (dataTable.Columns.Contains("Location") && !row["Location"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Location = row["Location"].ToString();
                                }

                                if (dataTable.Columns.Contains("Homepage") && !row["Homepage"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Homepage = row["Homepage"].ToString();
                                }

                                if (dataTable.Columns.Contains("Interests") && !row["Interests"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Interests = row["Interests"].ToString();
                                }

                                if (dataTable.Columns.Contains("Weblog") && !row["Weblog"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Blog = row["Weblog"].ToString();
                                }

                                if (dataTable.Columns.Contains("Gender") && !row["Gender"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Gender = row["Gender"].ToType<int>();
                                }

                                userProfile.Save();
                            }
                        }
                        else
                        {
                            // just update the link just in case...
                            LegacyDb.user_migrate(row["UserID"], user.ProviderUserKey, false);
                        }

                        // setup roles for this user...
                        using (DataTable dtGroups = LegacyDb.usergroup_list(row["UserID"]))
                        {
                            foreach (DataRow rowGroup in dtGroups.Rows)
                            {
                                AddUserToRole(user.UserName, rowGroup["Name"].ToString());
                            }
                        }
                    });
        }

        #endregion
    }
}