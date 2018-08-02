﻿using System;

namespace Seqrus.Web.Services.Authentication
{
    public class BasicAuthenticator : IAuthenticationService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _hasher;

        public BasicAuthenticator(IAccountRepository accountRepository, IPasswordHasher hasher)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        public void Authenticate(string username, string password)
        {
            // Note that this implementation is susceptible to account name enumeration using a combination of
            // Timing and brute-force attacks (https://en.wikipedia.org/wiki/Timing_attack)

            if (username == null)
                throw new LoginFailedException("No username provided");

            if (!_accountRepository.TryGetAccountByName(username, out var account))
                throw new LoginFailedException($"No such user: {username}");
            
            if(!_hasher.VerifyHash(password, account.PasswordHash))
                throw new LoginFailedException($"Incorrect password for user: {username}");
        }
    }
}