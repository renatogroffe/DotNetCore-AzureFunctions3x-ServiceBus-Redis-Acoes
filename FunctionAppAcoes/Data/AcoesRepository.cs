using System;
using System.Text.Json;
using FunctionAppAcoes.Models;
using StackExchange.Redis;

namespace FunctionAppAcoes.Data
{
    public static class AcoesRepository
    {
        private static ConnectionMultiplexer _CONEXAO_REDIS =
            ConnectionMultiplexer.Connect(
                Environment.GetEnvironmentVariable("BaseAcoesRedis"));

        public static void Save(Acao acao)
        {
            acao.UltimaAtualizacao = DateTime.Now;
            _CONEXAO_REDIS.GetDatabase().StringSet(
                "ACAO-" + acao.Codigo,
                JsonSerializer.Serialize(acao),
                expiry: null);
        }

        public static Acao Get(string codigo)
        {
            string strDadosAcao =
                _CONEXAO_REDIS.GetDatabase().StringGet($"ACAO-{codigo}");
            if (!String.IsNullOrWhiteSpace(strDadosAcao))
                return JsonSerializer.Deserialize<Acao>(
                    strDadosAcao,
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
            else
                return null;
        }
    }
}