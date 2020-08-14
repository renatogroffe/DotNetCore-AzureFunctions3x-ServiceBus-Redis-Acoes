using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using FunctionAppAcoes.Models;
using FunctionAppAcoes.Validators;
using FunctionAppAcoes.Data;

namespace FunctionAppAcoes
{
    public static class AcoesServiceBusTopicTrigger
    {
        [FunctionName("AcoesServiceBusTopicTrigger")]
        public static void Run([ServiceBusTrigger("topic-acoes", "redis", Connection = "AzureServiceBus")] string mySbMsg, ILogger log)
        {
            log.LogInformation($"AcoesServiceBusTopicTrigger - Dados: {mySbMsg}");

            Acao acao = null;
            try
            {
                acao = JsonSerializer.Deserialize<Acao>(mySbMsg,
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                log.LogError("AcoesServiceBusTopicTrigger - Erro durante a deserializacao!");
            }

            if (acao != null)
            {
                var validationResult = new AcaoValidator().Validate(acao);
                if (validationResult.IsValid)
                {
                    log.LogInformation($"AcoesServiceBusTopicTrigger - Dados pos formatacao: {JsonSerializer.Serialize(acao)}");
                    AcoesRepository.Save(acao);
                    log.LogInformation("AcoesServiceBusTopicTrigger - Acao registrada com sucesso!");
                }
                else
                {
                    log.LogError("AcoesServiceBusTopicTrigger - Dados invalidos para a Acao");
                    foreach (var error in validationResult.Errors)
                        log.LogError($"AcoesServiceBusTopicTrigger - {error.ErrorMessage}");
                }
            }
        }
    }
}