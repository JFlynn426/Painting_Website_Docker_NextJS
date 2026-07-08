namespace ServerApp.Application.Commands.Handlers;

using MediatR;
using ServerApp.Shared.Persistence;
using ServerApp.Application.Commands;
using ServerApp.Application.DTOs;
using ServerApp.Application.Services;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Factories;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.ValueObjects.Admin;
using ServerApp.Application.Exceptions;

public class LoginWithGoogleHandler : IRequestHandler<LoginWithGoogle, GoogleAuthResponse>
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAdminUserFactory _factory;
    private readonly IAdminUserWriteRepository _adminWriteRepository;
    private readonly IAdminUserReadRepository _adminReadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnumerable<string> _authorizedEmails;

    public LoginWithGoogleHandler(
        IGoogleAuthService googleAuthService,
        IJwtTokenService jwtTokenService,
        IAdminUserFactory factory,
        IAdminUserWriteRepository adminWriteRepository,
        IAdminUserReadRepository adminReadRepository,
        IUnitOfWork unitOfWork,
        IEnumerable<string> authorizedEmails)
    {
        _googleAuthService = googleAuthService;
        _jwtTokenService = jwtTokenService;
        _factory = factory;
        _adminWriteRepository = adminWriteRepository;
        _adminReadRepository = adminReadRepository;
        _unitOfWork = unitOfWork;
        _authorizedEmails = authorizedEmails.Select(e => e.ToLowerInvariant());
    }

    public async Task<GoogleAuthResponse> Handle(LoginWithGoogle command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Exchange Google auth code for user profile
            var userProfile = await _googleAuthService.ExchangeCodeForUserProfileAsync(command.Code);

            if (userProfile == null)
            {
                throw new InvalidOperationException("Failed to retrieve user profile from Google.");
            }

            // Check if email is authorized
            if (!_authorizedEmails.Contains(userProfile.Email.ToLowerInvariant()))
            {
                throw new AdminEmailNotAuthorizedException(userProfile.Email);
            }

            var email = new AdminEmail(userProfile.Email);
            var displayName = new AdminName(userProfile.DisplayName);
            var pictureUrl = string.IsNullOrEmpty(userProfile.PictureUrl)
                ? null
                : new AdminPictureUrl(userProfile.PictureUrl);
            var googleSub = new AdminGoogleSub(userProfile.GoogleSubjectId);

            // Try to find existing admin user by email
            var existingAdmin = await _adminWriteRepository.GetByEmailAsync(userProfile.Email, cancellationToken);

            AdminUser adminUser;
            if (existingAdmin != null)
            {
                // Update login info for existing user
                existingAdmin.UpdateLoginInfo(displayName, pictureUrl);
                await _adminWriteRepository.UpdateAsync(existingAdmin, cancellationToken);
                adminUser = existingAdmin;
            }
            else
            {
                // Create new admin user
                adminUser = _factory.Create(email, displayName, pictureUrl, googleSub);
                await _adminWriteRepository.AddAsync(adminUser, cancellationToken);
            }

            await _unitOfWork.CommitForceAsync(cancellationToken);

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(adminUser);

            // Return response with token and user info
            var adminDto = new AdminUserDto
            {
                Id = adminUser.Id,
                Email = adminUser.Email.Value,
                DisplayName = adminUser.DisplayName.Value,
                PictureUrl = adminUser.PictureUrl?.Value,
                LastLoginAt = adminUser.LastLoginAt.Value,
                CreatedAt = adminUser.CreatedAt.Value,
                IsActive = adminUser.IsActive.Value
            };

            return new GoogleAuthResponse(token, adminDto);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
