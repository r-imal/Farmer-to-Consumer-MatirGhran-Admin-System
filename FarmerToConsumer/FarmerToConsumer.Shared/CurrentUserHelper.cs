using Microsoft.AspNetCore.Http;

namespace FarmerToConsumer.Shared;

public class CurrentUserHelper(IHttpContextAccessor accessor)
{
    public bool IsAuthenticated
    {
        get
        {
            try
            {
                return accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
            }
            catch
            {
                return false;
            }
        }
    }

    public int UserId
    {
        get
        {
            try
            {
                var id = accessor.HttpContext?.User?.FindFirst("UserId")?.Value;
                return id != null ? int.Parse(id) : -1;
            }
            catch
            {
                return -1;
            }
        }
    }

    public string Email
    {
        get
        {
            try
            {
                return accessor.HttpContext?.User?.FindFirst("Email")?.Value ?? "-";
            }
            catch
            {
                return "-";
            }
        }
    }
}
