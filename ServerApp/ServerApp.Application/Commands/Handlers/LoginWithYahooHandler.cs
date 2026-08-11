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

public class LoginWithYahooHandler : IRequestHandler<LoginWithYahoo, AuthResponse>
{
    private readonly IYahooAuthService _yahooAuthService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAdminUserFactory _factory;
    private readonly IAdminUserWriteRepository _adminWriteRepository;
    private readonly IAdminUserReadRepository _adminReadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HashSet<string> _authorizedEmails;
    private readonly IStateStore _stateStore;

    public LoginWithYahooHandler(
        IYahooAuthService yahooAuthService,
        IJwtTokenService jwtTokenService,
        IAdminUserFactory factory,
        IAdminUserWriteRepository adminWriteRepository,
        IAdminUserReadRepository adminReadRepository,
        IUnitOfWork unitOfWork,
        HashSet<string> authorizedEmails,
        IStateStore stateStore)
    {
        _yahooAuthService = yahooAuthService;
        _jwtTokenService = jwtTokenService;
        _factory = factory;
        _adminWriteRepository = adminWriteRepository;
        _adminReadRepository = adminReadRepository;
        _unitOfWork = unitOfWork;
        _authorizedEmails = authorizedEmails;
        _stateStore = stateStore;
    }

    public async Task<AuthResponse> Handle(LoginWithYahoo command, CancellationToken cancellationToken = default)
    {
        // Step 1: Validate OAuth state parameter to prevent CSRF attacks (no transaction needed)
        if (!_stateStore.ValidateAndRemoveState(command.State))
        {
            throw new InvalidOperationException("Invalid or expired OAuth state parameter.");
        }

        // Step 2: Exchange Yahoo auth code for user profile (external API - no transaction)
        // OAuth authorization codes are single-use by specification (RFC 6749 Section 4.1.3).
        // Yahoo enforces this server-side, so application-level tracking is not required.
        var userProfile = await _yahooAuthService.ExchangeCodeForUserProfileAsync(command.Code);

        if (userProfile == null)
        {
            throw new InvalidOperationException("Failed to retrieve user profile from Yahoo.");
        }

        // Step 3: Check if email is authorized (no transaction needed)
        if (!_authorizedEmails.Contains(userProfile.Email))
        {
            throw new AdminEmailNotAuthorizedException(userProfile.Email);
        }

        var email = new AdminEmail(userProfile.Email);
        var displayName = new AdminName(userProfile.DisplayName);
        var pictureUrl = string.IsNullOrEmpty(userProfile.PictureUrl)
            ? null
            : new AdminPictureUrl(userProfile.PictureUrl);
        var yahooGuid = !string.IsNullOrEmpty(userProfile.YahooGuid)
            ? new AdminYahooGuid(userProfile.YahooGuid)
            : null;

        // Step 4: Start transaction AFTER external calls succeed (database operations only)
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        AdminUser adminUser;
        try
        {
            // Try to find existing admin user by email
            var existingAdmin = await _adminWriteRepository.GetByEmailAsync(userProfile.Email, cancellationToken);

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
                adminUser = _factory.Create(email, displayName, pictureUrl, yahooGuid: yahooGuid);
                await _adminWriteRepository.AddAsync(adminUser, cancellationToken);
            }

            await _unitOfWork.CommitForceAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }

        // Step 5: Generate JWT token (no transaction needed)
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

        return new AuthResponse(token, adminDto);
    }
}
