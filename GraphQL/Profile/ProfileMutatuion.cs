﻿using BusinessLayer;
using BusinessLayer.Authentication;
using DataLayer;
using DataLayer.Entities;
using DataLayer.Providers;
using GraphQL;
using GraphQL.Types;
using TimeTracker.GraphQL.Profile.Types;

namespace TimeTracker.GraphQL.Profile
{
    public class ProfileMutation : ObjectGraphType
    {
        private readonly IUserProvider userProvider;
        private readonly IAuthenticationService authenticationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ProfileMutation(IUserProvider userProvider,
            IAuthenticationService authenticationService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.userProvider = userProvider;
            this.authenticationService = authenticationService;
            this.httpContextAccessor = httpContextAccessor;

            Field<AuthenticationResultType>("Login")
                .Description("Authenticates user. Returns authentication info in case of success.")
                .Argument<NonNullGraphType<LoginInputType>>("input")
                .Resolve(context => ResolveLogin(context));

            Field<AuthenticationResultType>("FirstUserRegister")
                .Description("Registers first user into system.")
                .Argument<NonNullGraphType<FirstUserRegisterInputType>>("input")
                .Resolve(context => ResolveFirstUserRegister(context));

            Field<AuthenticationResultType>("Authenticate")
                .Description("Authenticates user.")
                .Resolve(context => ResolveAuthenticate(context));

            Field<BooleanGraphType>("Logout")
               .Description("Logout user.")
               .Authorize()
               .Resolve(context =>
               {
                   httpContextAccessor.HttpContext!.Response.Headers.Remove("Authorization");
                   return true;
               });
        }

        private AuthenticationResult? ResolveFirstUserRegister(IResolveFieldContext context)
        {
            var input = context.GetArgument<FirstUserRegisterInput>("input");
            var salt = authenticationService.GenerateSalt();
            var user = new User()
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                EmploymentDate = DateTime.Now,
                Salt = salt,
                EmploymentType = Constants.EmploymentType.FullTime,
                Name = input.Name,
                Surname = input.Surname,
                Email = input.Email,
                Password = authenticationService.GenerateHash(input.Password, salt),
                WorkingHoursCount = Constants.MaxWorkingHours,
            };

            userProvider.Create(user);

            if (authenticationService.Authenticate(user, input.Password, out var token))
            {
                var userContext = context.RequestServices!.GetRequiredService<UserContext>();
                userContext.User = user;
                return new AuthenticationResult()
                {

                    UserInfo = InitializeUserInfo(context.RequestServices!.GetRequiredService<UserContext>()),
                    Token = token!,
                };
            }

            return null;
        }

        private AuthenticationResult? ResolveLogin(IResolveFieldContext context)
        {
            var input = context.GetArgument<LoginInput>("input");
            var user = userProvider.GetByEmail(input.Email);

            if (user == null || !user.IsActive)
                return null;

            if (authenticationService.Authenticate(user, input.Password, out var token))
            {
                var userContext = context.RequestServices!.GetRequiredService<UserContext>();
                userContext.User = user;

                return new AuthenticationResult()
                {
                    UserInfo = InitializeUserInfo(userContext),
                    Token = token!,
                };
            }

            return null;
        }

        private AuthenticationResult? ResolveAuthenticate(IResolveFieldContext context)
        {
            var httpContext = httpContextAccessor.HttpContext;

            if (!httpContext?.User?.Identity?.IsAuthenticated ?? false)
                return null;

            return new AuthenticationResult()
            {
                UserInfo = InitializeUserInfo(context.RequestServices!.GetRequiredService<UserContext>()),
                Token = httpContext!.Request.Headers["Authorization"].First()!.Split(' ')[1],
            };
        }

        private UserInfo? InitializeUserInfo(UserContext userContext)
        {
            var authenticatedUser = userContext.User;

            if (authenticatedUser == null)
                return null;

            return new UserInfo()
            {
                Id = authenticatedUser.Id,
                Name = authenticatedUser.Name,
                Surname = authenticatedUser.Surname,
                Permissions = userContext.GetGrantedPermissions()
            };
        }
    }
}