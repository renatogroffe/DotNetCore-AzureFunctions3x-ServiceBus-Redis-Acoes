using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FunctionAppAcoes.Models;
using FunctionAppAcoes.Data;

namespace FunctionAppAcoes
{
    public static class Acoes
    {
        [FunctionName("Acoes")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string codigo = req.Query["codigo"];
            if (String.IsNullOrWhiteSpace(codigo))
            {
                log.LogError(
                    $"Acoes HTTP trigger - Codigo de Acao nao informado");
                return new BadRequestObjectResult(new
                {
                    Sucesso = false,
                    Mensagem = "Código de Ação não informado"
                });
            }

            log.LogInformation($"Acoes HTTP trigger - codigo da Acao: {codigo}");
            Acao acao = null;
            if (!String.IsNullOrWhiteSpace(codigo))
                acao = AcoesRepository.Get(codigo.ToUpper());

            if (acao != null)
            {
                log.LogInformation(
                    $"Acoes HTTP trigger - Acao: {codigo} | Valor atual: {acao.Valor} | Ultima atualizacao: {acao.UltimaAtualizacao}");
                return new OkObjectResult(acao);
            }
            else
            {
                log.LogError(
                    $"Acoes HTTP trigger - Codigo de Acao nao encontrado: {codigo}");
                return new NotFoundObjectResult(new
                {
                    Sucesso = false,
                    Mensagem = $"Código de Ação não encontrado: {codigo}"
                });
            }
        }
    }
}
