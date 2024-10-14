using System;
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace EmailSenderLambda.Tests
{
    public class FunctionTests
    {
        [Fact]
        public void FunctionHandler_ValidRequest_ReturnsOkResponse()
        {
            // Arrange
            var function = new Function();
            var context = new Mock<ILambdaContext>();
            var formData = new FormData
            {
                Nome = "Teste",
                Email = "teste@example.com",
                Telefone = "123456789",
                Mensagem = "Mensagem de teste"
            };
            var request = new APIGatewayProxyRequest
            {
                Body = JsonConvert.SerializeObject(formData)
            };

            // Configurar variáveis de ambiente mock
            Environment.SetEnvironmentVariable("mail_smtp_port", "587");
            Environment.SetEnvironmentVariable("mail_smtp", "smtp.example.com");
            Environment.SetEnvironmentVariable("mail_user", "user@example.com");
            Environment.SetEnvironmentVariable("mail_password", "password");
            Environment.SetEnvironmentVariable("mail_from", "from@example.com");
            Environment.SetEnvironmentVariable("mail_to", "to@example.com");

            // Act
            var response = function.FunctionHandler(request, context.Object);

            // Assert
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("E-mail enviado com sucesso!", response.Body);
            Assert.True(response.Headers.ContainsKey("Access-Control-Allow-Origin"));
            Assert.Equal("https://luafalcao.com/", response.Headers["Access-Control-Allow-Origin"]);
        }

        [Fact]
        public void FunctionHandler_InvalidRequest_ReturnsInternalServerError()
        {
            // Arrange
            var function = new Function();
            var context = new Mock<ILambdaContext>();
            var request = new APIGatewayProxyRequest
            {
                Body = "Invalid JSON"
            };

            // Act
            var response = function.FunctionHandler(request, context.Object);

            // Assert
            Assert.Equal(500, response.StatusCode);
            Assert.Contains("Erro ao enviar e-mail", response.Body);
        }
    }
}