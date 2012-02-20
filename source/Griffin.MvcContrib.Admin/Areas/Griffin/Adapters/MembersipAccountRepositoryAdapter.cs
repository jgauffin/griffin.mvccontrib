namespace Griffin.MvcContrib.Areas.Griffin.Adapters
{
    /*
	/// <summary>
	/// Adapter between the MembershipProvider and IAccountRepository interface
	/// </summary>
	/// <remarks>Used by <see cref="AccountController"/> when no other IAccountRepository has been registered</remarks>
	public class MembersipAccountRepositoryAdapter : IAccountRepository
	{
		/// <summary>
		/// Gets whether all users must have unique email addresses.
		/// </summary>
		public bool IsUniqueEmailRequired
		{
			get { return Membership.Provider.RequiresUniqueEmail; }
		}

		/// <summary>
		/// Register a new account.
		/// </summary>
		/// <param name="account">Acount to register</param>
		/// <returns>Result indication</returns>
		/// <remarks>
		/// Implementations should set the <see cref="IMembershipAccount.Id"/> property before returning.
		/// </remarks>
		public MembershipCreateStatus Register(IMembershipAccount account)
		{
			MembershipCreateStatus status;
			var user = Membership.Provider.CreateUser(account.UserName, account.Password, account.Email, account.PasswordQuestion,
			                                          account.PasswordAnswer, account.IsApproved, account.Id, out status);
			if (user != null)
				account.Id = user.ProviderUserKey;
			return status;
		}

		private MembershipUser CreateUser(IMembershipAccount account)
		{
			
		}

		private class MembershipAccountUser : IMembershipAccount
		{
			private readonly MembershipUser _user;

			public MembershipAccountUser(string applicationName, MembershipUser user)
			{
				ApplicationName = applicationName;
				_user = user;
			}

			/// <summary>
			/// Gets or sets application that the user belongs to
			/// </summary>
			public string ApplicationName { get; set; }

			/// <summary>
			/// Gets or sets email address
			/// </summary>
			public string Email
			{
				get { return _user.Email; }
				set { _user.Email = value; }
			}

			/// <summary>
			/// Gets or sets password question that must be answered to reset password
			/// </summary>
			/// <remarks>
			/// Controlled by the <see cref="IPasswordPolicy.IsPasswordQuestionRequired"/> property.
			/// </remarks>
			public string PasswordQuestion
			{
				get { return _user.PasswordQuestion; }
				set { throw new NotSupportedException("Cannot assign password question"); }
			}

			/// <summary>
			/// Gets or sets answer for the <see cref="PasswordQuestion"/>.
			/// </summary>
			public string PasswordAnswer
			{
				get { throw new NotSupportedException("Cannot fetch answer"); }
				set { throw new NotSupportedException("Cannot assign password answer"); }
			}

			/// <summary>
			/// Gets or sets a comment about the user.
			/// </summary>
			public string Comment
			{
				get { return _user.Comment; }
				set { _user.Comment = value; }
			}

			/// <summary>
			/// Gets or sets date/time when the user logged in last.
			/// </summary>
			public DateTime LastLoginAt
			{
				get { return _user.LastLoginDate; }
				set { _user.LastLoginDate = value; }
			}

			/// <summary>
			/// Gets or sets whether a new user have been approved and may login.
			/// </summary>
			public bool IsApproved
			{
				get { return _user.IsApproved; }
				set { _user.IsApproved = value; }
			}

			/// <summary>
			/// Gets or sets when the password were changed last time.
			/// </summary>
			public DateTime LastPasswordChangeAt
			{
				get { return _user.LastPasswordChangedDate; }
				set { throw new NotSupportedException(); }
			}

			/// <summary>
			/// Gets or sets if the account has been locked (the user may not login)
			/// </summary>
			public bool IsLockedOut
			{
				get { return _user.IsLockedOut; }
				set { throw new NotSupportedException(); }
			}

			/// <summary>
			/// Gets or sets if the user is online
			/// </summary>
			/// <remarks>
			/// Caluclated with the help of <see cref="LastActivityAt"/>.
			/// </remarks>
			public bool IsOnline
			{
				get { return _user.IsOnline; }
			}

			/// <summary>
			/// Gets or sets when the user was locked out.
			/// </summary>
			public DateTime LastLockedOutAt
			{
				get { return _user.LastLockoutDate; }
				set { throw new NotSupportedException(); }
			}

			/// <summary>
			/// Gets or sets when the user entered an incorrect password for the first time
			/// </summary>
			/// <value>
			/// DateTime.MinValue if the user has not entered an incorrect password (or succeded to login again).
			/// </value>
			public DateTime FailedPasswordWindowStartedAt
			{
				get { return _user.pass; }
				set { _user.Email = value; }
			}

			/// <summary>
			/// Gets or sets number of login attempts since <see cref="FailedPasswordWindowStartedAt"/>.
			/// </summary>
			public int FailedPasswordWindowAttemptCount
			{
				get { return _user.Email; }
				set { _user.Email = value; }
			}

			/// <summary>
			/// Gets or sets when the user answered the password question incorrect for the first time.
			/// </summary>
			/// <value>
			/// DateTime.MinValue if the user has not entered an incorrect answer (or succeded to login again).
			/// </value>
			public DateTime FailedPasswordAnswerWindowStartedAt
			{
				get { return _user.Email; }
				set { _user.Email = value; }
			}

			/// <summary>
			/// Gets or sets number of times that the user have answered the password question incorrectly since <see cref="FailedPasswordAnswerWindowAttemptCount"/>
			/// </summary>
			public int FailedPasswordAnswerWindowAttemptCount
			{
				get { return _user.Email; }
				set { _user.Email = value; }
			}

			/// <summary>
			/// Gets or sets when the user account was created.
			/// </summary>
			public DateTime CreatedAt
			{
				get { return _user.Email; }
				set { _user.Email = value; }
			}

			/// <summary>
			/// Gets or sets date/time when the user did something on the site
			/// </summary>
			public DateTime LastActivityAt
			{
				get { return _user.Email; }
				set { _user.Email = value; }
			}

			/// <summary>
			/// Gets or sets ID identifying the user
			/// </summary>
			/// <remarks>
			/// Should be an id in your system (for instance i your database)
			/// </remarks>
			public object Id
			{
				get { return _user.Email; }
				set { _user.Email = value; }
			}

			/// <summary>
			/// Gets or sets username
			/// </summary>
			public string UserName
			{
				get { return _user.Email; }
				set { _user.Email = value; }
			}

			/// <summary>
			/// Gets or sets password
			/// </summary>
			/// <remarks>The state of the password depends on the <seealso cref="IPasswordStrategy"/> that is used.</remarks>
			public string Password
			{
				get { return _user.Email; }
				set { _user.Email = value; }
			}

			/// <summary>
			/// Gets or sets the salt if a hashing strategy is used for the password.
			/// </summary>
			public string PasswordSalt
			{
				get { return _user.Email; }
				set { _user.Email = value; }
			}
		}

		/// <summary>
		/// Fetch a user from the service.
		/// </summary>
		/// <param name="username">Unique user name</param>
		/// <returns>User if found; otherwise null.</returns>
		public IMembershipAccount Get(string username)
		{
			var acount = Membership.GetUser(username);
			return new UserAccount();
		}

		/// <summary>
		/// Update an existing user.
		/// </summary>
		/// <param name="account">Account being updated.</param>
		public void Update(IMembershipAccount account)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get a user by using the implementation specific (your) Id.
		/// </summary>
		/// <param name="id">User identity specific for each account repository implementation</param>
		/// <returns>User if found; otherwise null.</returns>
		public IMembershipAccount GetByProviderKey(object id)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Translate an email into a user name.
		/// </summary>
		/// <param name="email">Email to lookup</param>
		/// <returns>User name if the specified email was found; otherwise null.</returns>
		public string GetUserNameByEmail(string email)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Delete a user from the database.
		/// </summary>
		/// <param name="username">Unique user name</param>
		/// <param name="deleteAllRelatedData">Delete information from all other tables etc</param>
		/// <returns>true if was removed successfully; otherwise false.</returns>
		public bool Delete(string username, bool deleteAllRelatedData)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get number of users that are online
		/// </summary>
		/// <returns>Number of online users</returns>
		public int GetNumberOfUsersOnline()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Find all users
		/// </summary>
		/// <param name="pageIndex">zero based index</param>
		/// <param name="pageSize">Number of users per page</param>
		/// <param name="totalRecords">Total number of users</param>
		/// <returns>A collection of users or an empty collection if no users was found.</returns>
		public IEnumerable<IMembershipAccount> FindAll(int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Find by searching for user name
		/// </summary>
		/// <param name="usernameToMatch">User name (or partial user name)</param>
		/// <param name="pageIndex">Zero based index</param>
		/// <param name="pageSize">Number of items per page</param>
		/// <param name="totalRecords">total number of records that partially matched the specified user name</param>
		/// <returns>A collection of users or an empty collection if no users was found.</returns>
		public IEnumerable<IMembershipAccount> FindByUserName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Find by searching for the specified email
		/// </summary>
		/// <param name="emailToMatch">Number of users that have the specified email (no partial matches)</param>
		/// <param name="pageIndex">Zero based index</param>
		/// <param name="pageSize">Number of items per page</param>
		/// <param name="totalRecords">total number of records that matched the specified email</param>
		/// <returns>A collection of users or an empty collection if no users was found.</returns>
		public IEnumerable<IMembershipAccount> FindByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Create a new membership account
		/// </summary>
		/// <param name="providerUserKey">Primary key in the data source</param>
		/// <param name="applicationName">Name of the application that the account is created for</param>
		/// <param name="username">User name</param>
		/// <param name="email">Email address</param>
		/// <returns>Created account</returns>
		public IMembershipAccount Create(object providerUserKey, string applicationName, string username, string email)
		{
			throw new NotImplementedException();
		}
	}*/
}