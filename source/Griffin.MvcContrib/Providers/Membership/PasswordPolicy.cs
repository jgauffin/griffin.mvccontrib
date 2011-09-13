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
    /// Default policy object
    /// </summary>
    public class PasswordPolicy : IPasswordPolicy
    {
        #region Implementation of IPasswordPolicy

        /// <summary>
        /// Gets number of invalid password or password-answer attempts allowed before the membership user is locked out
        /// </summary>
        public int MaxInvalidPasswordAttempts { get; set; }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        public bool IsPasswordQuestionRequired { get; set; }

        /// <summary>
        /// Gets whether the membership provider is configured to allow users to reset their passwords
        /// </summary>
        public bool IsPasswordResetEnabled { get; set; }

        /// <summary>
        /// Gets whether the membership provider is configured to allow users to retrieve their passwords
        /// </summary>
        public bool IsPasswordRetrievalEnabled { get; set; }

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        public int PasswordAttemptWindow { get; set; }

        /// <summary>
        /// Get minimum length required for a password
        /// </summary>
        public int PasswordMinimumLength { get; set; }

        /// <summary>
        /// Gets minimum number of special characters that must be present in a valid password
        /// </summary>
        public int MinRequiredNonAlphanumericCharacters { get; set; }

        /// <summary>
        /// Gets the regular expression used to evaluate a password
        /// </summary>
        public string PasswordStrengthRegularExpression { get; set; }

        #endregion
    }
}