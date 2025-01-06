using System.Collections.Concurrent;

namespace AutService.JwAuthLogin.Api.Services
{
    public static class TokenRevocationService
    {
        // Lista en memoria para almacenar tokens revocados
        private static readonly ConcurrentDictionary<string, DateTime> RevokedTokens = new();

        // Agregar un token a la lista de revocación
        public static void RevokeToken(string token, DateTime expiration)
        {
            RevokedTokens[token] = expiration;
        }

        // Validar si un token está revocado
        public static bool IsTokenRevoked(string token)
        {
            return RevokedTokens.ContainsKey(token);
        }

        // Limpieza periódica de tokens expirados (opcional)
        public static void CleanUpExpiredTokens()
        {
            foreach (var token in RevokedTokens.Keys)
            {
                if (RevokedTokens[token] < DateTime.UtcNow)
                {
                    RevokedTokens.TryRemove(token, out _);
                }
            }
        }
    }
}
