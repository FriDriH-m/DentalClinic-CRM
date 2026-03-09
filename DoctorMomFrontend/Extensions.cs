using DoctorMomFrontend.Utils;
using System.Net.Http;

namespace DoctorMomFrontend.Extensions
{
    public static class HttpClientExtension
    {
        public static void AddHeaders(this HttpClient client)
        {
            client.DefaultRequestHeaders.Add("Employee-Id", EmployeeSession.EmployeeId.ToString());
            client.DefaultRequestHeaders.Add("Employee-Role", EmployeeSession.Role);
        }
    }
}
