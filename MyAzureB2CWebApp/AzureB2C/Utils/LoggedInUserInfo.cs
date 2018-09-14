namespace AzureB2C.Utils
{
    public static class LoggedInUserInfo
    {
        public static string GetLoginUserEmailId()
        {
            var email = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("emails");
            return (email == null) ?null:email.Value;
        }

        public static string FullName()
        {
            var findFirstName = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            var firstName = (findFirstName == null) ? string.Empty : findFirstName.Value;

            var findLastName = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");
            var lastName = (findLastName == null) ? string.Empty : findLastName.Value;
            return firstName +" "+ lastName;
        }
    }
}