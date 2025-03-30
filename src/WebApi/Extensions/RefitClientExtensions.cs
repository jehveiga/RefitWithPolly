using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;
using System.Net;

namespace WebApi.Extensions
{
    public static class RefitClientExtensions
    {
        public static IServiceCollection AddRefitClientWithResilience<TInterface>(this IServiceCollection services, string baseUrl)
            where TInterface : class
        {
            services.AddRefitClient<TInterface>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddResilienceHandler("RefitResiliencePipeline", builder =>
                {
                    // Configuração do Retry
                    builder.AddRetry(new HttpRetryStrategyOptions
                    {
                        BackoffType = DelayBackoffType.Exponential,
                        MaxRetryAttempts = 5,
                        UseJitter = false,
                        ShouldHandle = static args =>
                        {
                            Console.WriteLine($"Tentando novamente Retry. Status: {args.Outcome.Result?.StatusCode}");

                            return ValueTask.FromResult(args.Outcome switch
                            {
                                { Result.StatusCode: HttpStatusCode.TooManyRequests } => true,
                                { Result.StatusCode: HttpStatusCode.BadGateway } => true,
                                { Result.StatusCode: HttpStatusCode.RequestTimeout } => true,
                                { Result.StatusCode: HttpStatusCode.InternalServerError } => true,
                                { Exception: not null } => true, // Retry em caso de exceções
                                _ => false
                            });
                        }
                    });

                    // Configuração do Circuit Breaker
                    builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                    {
                        SamplingDuration = TimeSpan.FromSeconds(10),
                        FailureRatio = 0.2, // Circuito abre após 20% de falhas
                        MinimumThroughput = 3, // Requer no mínimo 3 requisições para avaliar
                        ShouldHandle = static args =>
                        {
                            return ValueTask.FromResult(args.Outcome switch
                            {
                                { Result.StatusCode: HttpStatusCode.RequestTimeout } => true,
                                { Result.StatusCode: HttpStatusCode.TooManyRequests } => true,
                                { Result.StatusCode: HttpStatusCode.InternalServerError } => true,
                                _ => false
                            });
                        }
                    });
                });

            return services;
        }
    }
}