using Yarp.ReverseProxy.Configuration;

namespace Aspire.ApiGateway
{
    public static class BaseConfiguration
    {
        public static IReadOnlyList<RouteConfig> GetRoutes()
        {
            return new[]
            {
                new RouteConfig {
                    RouteId = "productRoute",
                    ClusterId = "products",
                    Match = new RouteMatch
                    {
                        Path = "/products/{**catch-all}"
                    },
                    Transforms = new List<Dictionary<string, string>>
                    {
                        new() {
                            { "PathRemovePrefix", "/products" }
                        }
                    }
                },
                new RouteConfig {
                    RouteId = "authRoute",
                    ClusterId = "auth",
                    Match = new RouteMatch
                    {
                        Path = "/auth/{**catch-all}"
                    },
                    Transforms = new List<Dictionary<string, string>>
                    {
                        new() {{ "PathRemovePrefix", "/auth" }},
                        new() {{ "PathPrefix", "/api/Auth" }}
                    }
                },
            };
        }

        public static IReadOnlyList<ClusterConfig> GetClusters()
        {
            return new[]
            {
                new ClusterConfig
                {
                    ClusterId = "products",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        { "products", new DestinationConfig { Address = "https://localhost:7432" } }
                    }
                },
                new ClusterConfig
                {
                    ClusterId = "auth",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        { "auth", new DestinationConfig { Address = "https://localhost:7432" } }
                    }
                },
            };
        }
    }
}
