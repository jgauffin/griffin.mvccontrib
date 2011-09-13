using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Griffin.MvcContrib.Providers.Membership.PasswordStrategies
{
    public static class PasswordPolicyExtensions
    {
        private static readonly Random _random = new Random();

        public static bool IsPasswordValid(this IPasswordPolicy passwordPolicy, string password)
        {
            var alphaCount = password.Count(ch => !char.IsLetterOrDigit(ch));
            if (alphaCount < passwordPolicy.MinRequiredNonAlphanumericCharacters)
                return false;
            return password.Length >= passwordPolicy.PasswordMinimumLength;
        }

        /// <summary>
        /// Generate a new password
        /// </summary>
        /// <param name="policy">Policy that should be used when generating a new password.</param>
        /// <returns>A password which is not encrypted.</returns>
        /// <remarks>Uses characters which can't be mixed up along with "@!?&%/\" if non alphas are required</remarks>
        public static string GeneratePassword(this IPasswordPolicy policy)
        {
            var length = _random.Next(policy.PasswordMinimumLength, policy.PasswordMinimumLength + 5);
            string password = "";

            var allowedCharacters = "abcdefghjkmnopqrstuvxtzABCDEFGHJKLMNPQRSTUVXYZ23456789";
            var alphas = "@!?&%/\\";
            if (policy.MinRequiredNonAlphanumericCharacters > 0)
                allowedCharacters += alphas;

            var nonAlphaLeft = policy.MinRequiredNonAlphanumericCharacters;
            for (int i = 0; i < length; i++)
            {
                char ch = allowedCharacters[_random.Next(0, allowedCharacters.Length)];
                if (alphas.IndexOf(ch) != -1)
                    nonAlphaLeft--;

                if (length - i <= nonAlphaLeft)
                    ch = alphas[_random.Next(0, alphas.Length)];

                password += ch;
            }

            return password;
        }

    }
}