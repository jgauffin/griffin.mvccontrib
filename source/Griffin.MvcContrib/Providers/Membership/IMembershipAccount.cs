/*
 * Copyright (c) 2011, Jonas Gauffin. All rights reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301 USA
 */

using System;

namespace Griffin.MvcContrib.Providers.Membership
{
    /// <summary>
    /// Account information for a user 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that all fields are get/set. The motivation is that this interface (or any implementation) should not be considered
    /// as a first class citizen, but as a DTO. It's solely purpose it to be able to fetch/store information
    /// in any datasource in a simple way (just implement this class and the repository interface). It should not be used
    /// for anything else.
    /// </para>
    /// <para>Breaking change: The ID field has been replaced with a "ProviderUserKey" field.</para>
    /// </remarks>
    public interface IMembershipAccount
    {
        /// <summary>
        /// Gets or sets application that the user belongs to
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets email address
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets password question that must be answered to reset password
        /// </summary>
        /// <remarks>
        /// Controlled by the <see cref="IPasswordPolicy.IsPasswordQuestionRequired"/> property.
        /// </remarks>
        string PasswordQuestion { get; set; }

        /// <summary>
        /// Gets or sets answer for the <see cref="PasswordQuestion"/>.
        /// </summary>
        string PasswordAnswer { get; set; }

        /// <summary>
        /// Gets or sets a comment about the user.
        /// </summary>
        string Comment { get; set; }

        /// <summary>
        /// Gets or sets date/time when the user logged in last.
        /// </summary>
        DateTime LastLoginAt { get; set; }

        /// <summary>
        /// Gets or sets whether a new user have been approved and may login.
        /// </summary>
        bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets when the password were changed last time.
        /// </summary>
        DateTime LastPasswordChangeAt { get; set; }

        /// <summary>
        /// Gets or sets if the account has been locked (the user may not login)
        /// </summary>
        bool IsLockedOut { get; set; }

        /// <summary>
        /// Gets or sets if the user is online
        /// </summary>
        /// <remarks>
        /// Caluclated with the help of <see cref="LastActivityAt"/>.
        /// </remarks>
        bool IsOnline { get; }

        /// <summary>
        /// Gets or sets when the user was locked out.
        /// </summary>
        DateTime LastLockedOutAt { get; set; }

        /// <summary>
        /// Gets or sets when the user entered an incorrect password for the first time
        /// </summary>
        /// <value>
        /// DateTime.MinValue if the user has not entered an incorrect password (or succeded to login again).
        /// </value>
        DateTime FailedPasswordWindowStartedAt { get; set; }

        /// <summary>
        /// Gets or sets number of login attempts since <see cref="FailedPasswordWindowStartedAt"/>.
        /// </summary>
        int FailedPasswordWindowAttemptCount { get; set; }

        /// <summary>
        /// Gets or sets when the user answered the password question incorrect for the first time.
        /// </summary>
        /// <value>
        /// DateTime.MinValue if the user has not entered an incorrect answer (or succeded to login again).
        /// </value>
        DateTime FailedPasswordAnswerWindowStartedAt { get; set; }

        /// <summary>
        /// Gets or sets number of times that the user have answered the password question incorrectly since <see cref="FailedPasswordAnswerWindowAttemptCount"/>
        /// </summary>
        int FailedPasswordAnswerWindowAttemptCount { get; set; }

        /// <summary>
        /// Gets or sets when the user account was created.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets date/time when the user did something on the site
        /// </summary>
        DateTime LastActivityAt { get; set; }

        /// <summary>
        /// Gets or sets ID identifying the user
        /// </summary>
        /// <remarks>
        /// Should be an id in your system (for instance in your database)
        /// </remarks>
        object ProviderUserKey { get; set; }

        /// <summary>
        /// Gets or sets username
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Gets or sets password
        /// </summary>
        /// <remarks>The state of the password depends on the <seealso cref="IPasswordStrategy"/> that is used.</remarks>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets the salt if a hashing strategy is used for the password.
        /// </summary>
        string PasswordSalt { get; set; }
    }
}