//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Authorization.Infrastructure;

//namespace AuthenticationNetCore.Utils
//{
//    public class ConfigPrefix
//    {

//public class CustomAuthorizeAttribute : AuthorizeAttribute
//    {
//        private const string POLICY_PREFIX = "Quyen";

//        public CustomAuthorizeAttribute(string quyen)
//        {
//            // Tạo chính sách tùy chỉnh từ giá trị "Quyen="
//            Policy = $"{POLICY_PREFIX}={quyen}";
//        }
//    }




//public class CustomAuthorizationHandler : AuthorizationHandler<AuthorizationRequirement>
//    {
//        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
//        {
//            var policy = requirement as RolesAuthorizationRequirement;
//            if (policy != null)
//            {
//                var quyenClaim = context.User.Claims.FirstOrDefault(c => c.Type == "Quyen");

//                if (quyenClaim != null && policy.AllowedRoles.Contains(quyenClaim.Value))
//                {
//                    context.Succeed(requirement);
//                }
//            }

//            return Task.CompletedTask;
//        }
//    }
//        services.AddAuthorization(options =>
//{
//    options.AddPolicy("Quyen", policy =>
//        policy.Requirements.Add(new CustomAuthorizationHandler()));
//});

//services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();


//}
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class QuyenAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public string Quyen { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (Quyen != null)
        {
            // Thay thế giá trị "Quyen" thành "Roles" cho Authorize
            this.Roles = Quyen;
        }
    }
}
