using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Griffin.MvcContrib.Providers.Membership;

namespace Griffin.MvcContrib.Areas.Griffin.Models.Account
{
	public class Account : IMembershipAccount
	{
		/// <summary>
		/// Gets or sets application that the user belongs to
		/// </summary>
		public string ApplicationName { get; set; }

		/// <summary>
		/// Gets or sets email address
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets password question that must be answered to reset password
		/// </summary>
		/// <remarks>
		/// Controlled by the <see cref="IPasswordPolicy.IsPasswordQuestionRequired"/> property.
		/// </remarks>
		public string PasswordQuestion { get; set; }

		/// <summary>
		/// Gets or sets answer for the <see cref="PasswordQuestion"/>.
		/// </summary>
		public string PasswordAnswer { get; set; }

		/// <summary>
		/// Gets or sets a comment about the user.
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// Gets or sets date/time when the user logged in last.
		/// </summary>
		public DateTime LastLoginAt { get; set; }

		/// <summary>
		/// Gets or sets whether a new user have been approved and may login.
		/// </summary>
		public bool IsApproved { get; set; }

		/// <summary>
		/// Gets or sets when the password were changed last time.
		/// </summary>
		public DateTime LastPasswordChangeAt { get; set; }

		/// <summary>
		/// Gets or sets if the account has been locked (the user may not login)
		/// </summary>
		public bool IsLockedOut { get; set; }

		/// <summary>
		/// Gets or sets if the user is online
		/// </summary>
		/// <remarks>
		/// Caluclated with the help of <see cref="LastActivityAt"/>.
		/// </remarks>
		public bool IsOnline { get; set; }

		/// <summary>
		/// Gets or sets when the user was locked out.
		/// </summary>
		public DateTime LastLockedOutAt { get; set; }

		/// <summary>
		/// Gets or sets when the user entered an incorrect password for the first time
		/// </summary>
		/// <value>
		/// DateTime.MinValue if the user has not entered an incorrect password (or succeded to login again).
		/// </value>
		public DateTime FailedPasswordWindowStartedAt { get; set; }

		/// <summary>
		/// Gets or sets number of login attempts since <see cref="FailedPasswordWindowStartedAt"/>.
		/// </summary>
		public int FailedPasswordWindowAttemptCount { get; set; }

		/// <summary>
		/// Gets or sets when the user answered the password question incorrect for the first time.
		/// </summary>
		/// <value>
		/// DateTime.MinValue if the user has not entered an incorrect answer (or succeded to login again).
		/// </value>
		public DateTime FailedPasswordAnswerWindowStartedAt { get; set; }

		/// <summary>
		/// Gets or sets number of times that the user have answered the password question incorrectly since <see cref="FailedPasswordAnswerWindowAttemptCount"/>
		/// </summary>
		public int FailedPasswordAnswerWindowAttemptCount { get; set; }

		/// <summary>
		/// Gets or sets when the user account was created.
		/// </summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// Gets or sets date/time when the user did something on the site
		/// </summary>
		public DateTime LastActivityAt { get; set; }

		/// <summary>
		/// Gets or sets ID identifying the user
		/// </summary>
		/// <remarks>
		/// Should be an id in your system (for instance i your database)
		/// </remarks>
		public object Id { get; set; }

		/// <summary>
		/// Gets or sets username
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets password
		/// </summary>
		/// <remarks>The state of the password depends on the <seealso cref="IPasswordStrategy"/> that is used.</remarks>
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the salt if a hashing strategy is used for the password.
		/// </summary>
		public string PasswordSalt { get; set; }
	}
}