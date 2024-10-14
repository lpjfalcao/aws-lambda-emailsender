using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EmailSenderLambda;

public class Function
{
    public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"Conteúdo da requisição: {request}");
            context.Logger.LogInformation(($"Conteúdo do body: {request.Body}"));
            context.Logger.LogInformation("Deserializando objeto...");
            var formData = JsonConvert.DeserializeObject<FormData>(request.Body);
            context.Logger.LogInformation($"Objeto deserializado com sucesso");

            if (formData == null)
                throw new Exception("O conteúdo da requisição nao pode ser deserializado");

            context.Logger.LogInformation(("Enviando e-mail...."));

            int port = int.Parse(Environment.GetEnvironmentVariable("mail_smtp_port"));
            string smtp = Environment.GetEnvironmentVariable("mail_smtp");
            string user = Environment.GetEnvironmentVariable("mail_user");
            string password = Environment.GetEnvironmentVariable("mail_password");
            string from = Environment.GetEnvironmentVariable("mail_from");
            string to = Environment.GetEnvironmentVariable("mail_to");

            context.Logger.LogInformation("Variáveis de ambiente...");
            context.Logger.LogInformation($"Porta: {port}");
            context.Logger.LogInformation($"Smtp: {smtp}");
            context.Logger.LogInformation($"User: {user}");
            context.Logger.LogInformation($"Password: {password}");
            context.Logger.LogInformation($"From: {from}");
            context.Logger.LogInformation($"To: {to}");
            
            context.Logger.LogInformation("Estabelecendo conexão com o servidor SMTP...");
            using (var client = new SmtpClient(smtp))
            {
                context.Logger.LogInformation("Conexão com o servidor SMTP estabelecida com sucesso!");
                context.Logger.LogInformation("Definindo portas e credenciais...");
                client.Port = port;
                client.Credentials = new NetworkCredential(user, password);
                client.EnableSsl = true;
                context.Logger.LogInformation("Portas e credenciais definidas com sucesso!");

                context.Logger.LogInformation("Configurando e-mail...");
                var message = new MailMessage();
                message.From = new MailAddress(from!);
                message.To.Add(to!);
                message.Subject = "Novo formulário de contato - luafalcao.com";
                message.Body =
                    $"Nome: {formData.Nome}\nEmail: {formData.Email}\nTelefone: {formData.Telefone}\nMensagem: {formData.Mensagem}";
                context.Logger.LogInformation("E-mail configurado com sucesso!");

                context.Logger.LogInformation("Enviando o e-mail...");
                client.Send(message);
                context.Logger.LogInformation("E-mail enviado com sucesso!");
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = "E-mail enviado com sucesso!",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "https://luafalcao.com/" },
                    {
                        "Access-Control-Allow-Headers",
                        "Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token"
                    },
                    { "Access-Control-Allow-Methods", "GET,POST,OPTIONS" },
                    { "Access-Control-Allow-Credentials", "true" }
                }
            };
        }
        catch (Exception ex)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = $"Erro ao enviar e-mail: {ex.Message}"
            };
        }
    }
}

public class FormData
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Mensagem { get; set; }
    public string Telefone { get; set; }
}