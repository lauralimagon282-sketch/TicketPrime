using Dapper;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

string connectionString = "Server=localhost\\SQLEXPRESS;Database=TicketPrime;Trusted_Connection=True;TrustServerCertificate=True;";

// ==========================
// USUÁRIOS & LOGIN
// ==========================

// CADASTRAR (Nome, CPF, Email, Senha, NivelAcesso)
app.MapPost("/api/usuarios", async (Usuario usuario) =>
{
    // 1. Validação de campos vazios (Mantida)
    if (string.IsNullOrWhiteSpace(usuario.Nome) || 
        string.IsNullOrWhiteSpace(usuario.Cpf) || 
        string.IsNullOrWhiteSpace(usuario.Email) || 
        string.IsNullOrWhiteSpace(usuario.Senha))
    {
        return Results.BadRequest("Todos os campos são obrigatórios.");
    }

    using var db = new SqlConnection(connectionString);
    
    // 2. Validação de CPF Duplicado (Mantida)
    var existe = await db.QueryFirstOrDefaultAsync<int>(
        "SELECT 1 FROM Usuarios WHERE Cpf = @Cpf", new { usuario.Cpf }
    );
    if (existe == 1) return Results.BadRequest("Este CPF já está cadastrado.");

    // 3. Inserção no Banco (CORRIGIDA: agora salva o NivelAcesso)
    var sql = @"INSERT INTO Usuarios (Cpf, Nome, Email, Senha, NivelAcesso) 
                VALUES (@Cpf, @Nome, @Email, @Senha, @NivelAcesso)";
    
    await db.ExecuteAsync(sql, usuario);
    
    return Results.Created($"/api/usuarios/{usuario.Cpf}", new { 
        usuario.Cpf, 
        usuario.Nome, 
        usuario.Email, 
        usuario.NivelAcesso 
    });
});
// NOVA ROTA: Listar eventos de um vendedor específico
app.MapGet("/api/eventos/vendedor/{vendedorId}", async (int vendedorId) => {
    using var db = new SqlConnection(connectionString);
    var sql = "SELECT * FROM Eventos WHERE VendedorId = @vendedorId ORDER BY Data ASC";
    var eventos = await db.QueryAsync<Evento>(sql, new { vendedorId });
    return Results.Ok(eventos);
});
// LOGIN (Email e Senha)
app.MapPost("/api/login", async (LoginRequest login) =>
{
    using var db = new SqlConnection(connectionString);
    
    //Checa se o e-mail existe
    var existeEmail = await db.QueryFirstOrDefaultAsync<int>(
        "SELECT 1 FROM Usuarios WHERE LOWER(TRIM(Email)) = LOWER(TRIM(@Email))", 
        new { login.Email }
    );

    if (existeEmail == 0) 
    {
        // Retorna um erro específico: 404 (Não encontrado)
        return Results.NotFound("Usuário não cadastrado. Por favor, cadastre-se primeiro.");
    }

    // 2. Se o e-mail existe, checa a senha
    var sql = "SELECT * FROM Usuarios WHERE LOWER(TRIM(Email)) = LOWER(TRIM(@Email)) AND Senha = @Senha";
    var user = await db.QueryFirstOrDefaultAsync<Usuario>(sql, login);
    
    if (user != null) return Results.Ok(user);

    // Se chegou aqui, o e-mail existe mas a senha está errada
    return Results.BadRequest("Senha incorreta.");
});
app.MapPost("/api/suporte", async (SuporteMensagem msg) =>
{
    // Fail-Fast: Validação básica
    if (string.IsNullOrWhiteSpace(msg.Nome) || string.IsNullOrWhiteSpace(msg.Mensagem))
        return Results.BadRequest("Nome e Mensagem são obrigatórios.");

    using var db = new SqlConnection(connectionString);
    var sql = @"INSERT INTO Suporte (Nome, Email, Tipo, Mensagem) 
                VALUES (@Nome, @Email, @Tipo, @Mensagem)";
    
    await db.ExecuteAsync(sql, msg);
    return Results.Ok("Mensagem enviada com sucesso!");
});
// ROTA PARA CADASTRAR EVENTOS (Usada pelo Vendedor)
app.MapPost("/api/eventos", async (Evento ev) => {
    // Adicione a validação de quantidade aqui também por segurança
    if (ev.Preco <= 0 || ev.Quantidade <= 0) 
        return Results.BadRequest("Preço e quantidade devem ser maiores que zero.");

    using var db = new SqlConnection(connectionString);   
    var sql = @"INSERT INTO Eventos (Nome, Local, Data, Preco, Quantidade, ImagemURL, VendedorId) 
                VALUES (@Nome, @Local, @Data, @Preco, @Quantidade, @ImagemURL, @VendedorId)";
    
    await db.ExecuteAsync(sql, ev);
    return Results.Ok("Show anunciado com sucesso!");
});

// ROTA PARA LISTAR EVENTOS (Usada pelo Cliente na Vitrine)
app.MapGet("/api/eventos", async () => {
    using var db = new SqlConnection(connectionString);
    var sql = "SELECT * FROM Eventos ORDER BY Data ASC";
    var eventos = await db.QueryAsync<Evento>(sql);
    return Results.Ok(eventos);
});
// ROTA PARA EXCLUIR EVENTO (Acessada pelo Admin)
app.MapDelete("/api/eventos/{id}", async (int id) => {
    using var db = new SqlConnection(connectionString);
    var sql = "DELETE FROM Eventos WHERE Id = @id";
    await db.ExecuteAsync(sql, new { id });
    return Results.Ok("Evento removido com sucesso!");
});

// ROTA PARA CRIAR CUPOM ADMIN
app.MapPost("/api/cupons", async (Cupom cp) => {
    // Validação de segurança no Back-end
    if (cp.PorcentagemDesconto <= 0) {
        return Results.BadRequest("O desconto deve ser maior que zero.");
    }

    using var db = new SqlConnection(connectionString);
    var sql = "INSERT INTO Cupons (Codigo, PorcentagemDesconto, ValorMinimo) VALUES (@Codigo, @PorcentagemDesconto, @ValorMinimo)";
    
    await db.ExecuteAsync(sql, cp);
    return Results.Ok("Cupom criado com sucesso!");
});
// ROTA PARA LISTAR TODOS OS CUPONS
app.MapGet("/api/cupons", async () => {
    using var db = new SqlConnection(connectionString);
    var sql = "SELECT * FROM Cupons";
    var cupons = await db.QueryAsync<Cupom>(sql);
    return Results.Ok(cupons);
});

// ROTA PARA EXCLUIR UM CUPOM
app.MapDelete("/api/cupons/{id}", async (int id) => {
    using var db = new SqlConnection(connectionString);
    var sql = "DELETE FROM Cupons WHERE Id = @id";
    await db.ExecuteAsync(sql, new { id });
    return Results.Ok("Cupom removido!");
});
// ROTA PARA VALIDAR O CUPOM (Adicione isso!)
app.MapGet("/api/cupons/{codigo}", async (string codigo) => {
    using var db = new SqlConnection(connectionString);
    var sql = "SELECT * FROM Cupons WHERE Codigo = @codigo";
    var cupom = await db.QueryFirstOrDefaultAsync<Cupom>(sql, new { codigo });

    if (cupom != null) {
        return Results.Ok(new { 
            codigo = cupom.Codigo, 
            porcentagem = cupom.PorcentagemDesconto 
        });
    }
    return Results.NotFound("Cupom não encontrado.");
});
app.Run();

// MODELS (Records)
public record Usuario(int Id, string Nome, string Cpf, string Email, string Senha, int NivelAcesso);
public record LoginRequest(string Email, string Senha);
public record Evento(int Id, string Nome, string Local, DateTime Data, decimal Preco, int Quantidade, string ImagemURL, int VendedorId);
public record Cupom(int Id, string Codigo, decimal PorcentagemDesconto, decimal ValorMinimo);
public record SuporteMensagem(string Nome, string Email, string Tipo, string Mensagem);