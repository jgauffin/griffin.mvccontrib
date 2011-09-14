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

namespace Griffin.MvcContrib.Providers.Membership
{
    /// <summary>
    /// Information used by the password strategies.
    /// </summary>
    public class AccountPasswordInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountPasswordInfo"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public AccountPasswordInfo(string username, string password)
        {
            UserName = username;
            Password = password;
        }

        /// <summary>
        /// Gest or sets the salt which was used when hashing the password.
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Gets the password
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Gets username for the accoount
        /// </summary>
        public string UserName { get; private set; }
    }
}