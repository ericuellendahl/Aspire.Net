# 🚀 API .NET 8 com .NET Aspire

Este projeto é uma API construída com **.NET 8** utilizando o ecossistema **.NET Aspire**, focada em modularidade, observabilidade e escalabilidade. A solução está preparada para ambientes cloud-native e oferece integração com ferramentas modernas como **OpenTelemetry**, **Swagger** e suporte completo para containerização com **Docker**.

O objetivo deste projeto é promover a evolução contínua e melhorias constantes, visando o aprendizado e aprimoramento técnico.

## ✅ Tecnologias Utilizadas

- ASP.NET Core 8
- .NET Aspire
- OpenTelemetry
- Swagger (Swashbuckle)
- Docker 
- Clean Architecture (opcional) (separação em pasta por enquanto)
- Injeção de Dependência
- Middleware customizado
- Observabilidade nativa
- Worker Consumer do RabbitMQ

## ⚙️ Configuração

Antes de executar o projeto, você deve configurar a string de conexão do banco de dados no arquivo `appsettings.json`, conforme o banco de dados de sua preferência.

### Exemplo:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MinhaApiDb;User Id=usuario;Password=senha;"
  }
}
````

Você pode usar **SQL Server**, **PostgreSQL**, **MySQL**, **SQLite**, entre outros — basta ajustar o provedor e a string de conexão conforme necessário.

## ▶️ Executando o Projeto

```bash
dotnet run
```

A API estará disponível por padrão em:

```
https://localhost:{porta}/swagger
```

## 🧪 Testes

Você pode utilizar ferramentas como **Postman**, **curl** ou o próprio **Swagger UI** para testar os endpoints expostos pela API.

## 📄 Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

