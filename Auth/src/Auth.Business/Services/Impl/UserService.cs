using Auth.Api.Handlers;
using Auth.Business.Factories;
using Auth.Business.Handlers;
using Auth.Business.Models;
using Auth.Business.Repositories;
using Microsoft.Extensions.Logging;
using SharedKernel.Enums;
using SharedKernel.Notifications;


namespace Auth.Business.Services.Impl
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFactory _userFactory;
        private readonly INotification _notification;
        private readonly ILogger<UserService> _logger;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IJwtHandler _jwtHandler;

        public UserService(
            IUserRepository userRepository,
            IUserFactory userFactory,
            ILogger<UserService> logger,
            INotification notification,
            IPasswordHandler passwordHandler,
            IJwtHandler jwtHandler)
        {
            _userRepository = userRepository;
            _userFactory = userFactory;
            _logger = logger;
            _notification = notification;
            _passwordHandler = passwordHandler;
            _jwtHandler = jwtHandler;
        }

        public async Task CreateAsync(UserModel user)
        {
            try
            {
                var userEntity = await _userRepository.GetByOrAsync(user.Id, user.UserName);
                if (userEntity is { })
                {
                    _notification.Add($"User {user.UserName} or Id {user.Id} already exists.");
                    return;
                }

                if (string.IsNullOrEmpty(user.Password))
                {
                    _notification.Add("Password is required.");
                    return;
                }

                userEntity = _userFactory.From(user);
                userEntity.SetPasswordHash(_passwordHandler.HashPassword(user.Password));
                var validation = userEntity.Validate();

                if (validation.Any())
                {
                    _notification.Add(validation);
                    return;
                }

                await _userRepository.AddAsync(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to add user {Name} Error: {Message}.", user.UserName, ex.Message);
                _notification.Add($"Failed to add user {user.UserName}.", NotificationType.Unexpected);
            }
        }

        public async Task<string> LoginAsync(string userName, string password)
        {
            try
            {
                var userEntity = await _userRepository.GetByAsync(userName);
                if (userEntity is null)
                {
                    _notification.Add($"Invalid UserName/Password.");
                    _logger.LogInformation("Login attempt for {Name} not found.", userName);
                }

                var isValidPassword = _passwordHandler.VerifyPassword(password, userEntity!.PasswordHash);

                if (!isValidPassword)
                {
                    _notification.Add($"Invalid UserName/Password.");
                    _logger.LogInformation("Login attempt for {Name} with not valid password.", userName);
                }

                var user = _userFactory.From(userEntity);
                var token = _jwtHandler.GenerateToken(user);

                if (string.IsNullOrEmpty(token))
                {
                    _notification.Add($"Invalid UserName/Password.");
                    _logger.LogInformation("Token no created for {Name} with not valid password.", userName);
                }

                return token ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to Login UserName {Id} Error: {Message}.", userName, ex.Message);
                _notification.Add($"Failed to Login UserName {userName}.", NotificationType.Unexpected);
            }

            return string.Empty;
        }
    }
}
