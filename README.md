# aws-lambda-emailsender
Função lambda que faz o disparo de e-mail para uma caixa de entrada 

Esta função lambda é acionada via chamada de um endpoint que é funciona por meio de integração com o AWS API Gateway e utiliza variáveis de ambiente para armazenar credenciais e configurações usadas no aplicativo. Ela foi escrita na linguagem C#, com .NET 6.0 e também faz o uso de logs no Cloud Watch para ajudar no monitoramento.

## Tecnologias utilizadas

- AWS Lambda
- AWS API Gateway
- AWS CloudWatch
- C# /.NET 6.0
