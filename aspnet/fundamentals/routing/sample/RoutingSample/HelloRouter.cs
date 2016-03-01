using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;

namespace RoutingSample
{
    public class HelloRouter : IRouter
    {
        public async Task RouteAsync(RouteContext context)
        {
            object name;
            if (!context.RouteData.Values.TryGetValue("name", out name))
            {
                return;
            }
            await context.HttpContext.Response.WriteAsync($"Hi {name}!");
            context.IsHandled = true;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }
    }
}