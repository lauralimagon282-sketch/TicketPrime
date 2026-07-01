using Dapper;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                          ?? throw new InvalidOperationException("A string de conexão 'DefaultConnection' não foi encontrada.");

//USUÁRIOS & LOGIN
app.MapPost("/api/usuarios", async (Usuario usuario) =>
{
    if (string.IsNullOrWhiteSpace(usuario.Cpf) || string.IsNullOrWhiteSpace(usuario.Nome))
        return Results.BadRequest("Campos obrigatórios ausentes.");

    using var db = new SqlConnection(connectionString);
    var existe = await db.QueryFirstOrDefaultAsync<int>("SELECT 1 FROM Usuarios WHERE Cpf = @Cpf", new { usuario.Cpf });
    if (existe == 1) return Results.BadRequest("Este CPF já está cadastrado.");

    var sql = @"INSERT INTO Usuarios (Cpf, Nome, Email, Senha, NivelAcesso) 
                VALUES (@Cpf, @Nome, @Email, @Senha, @NivelAcesso)";
    await db.ExecuteAsync(sql, usuario);
    return Results.Created($"/api/usuarios/{usuario.Cpf}", usuario);
});

app.MapPost("/api/login", async (LoginRequest login) =>
{
    using var db = new SqlConnection(connectionString);
    var sql = "SELECT * FROM Usuarios WHERE LOWER(TRIM(Email)) = LOWER(TRIM(@Email)) AND Senha = @Senha";
    var user = await db.QueryFirstOrDefaultAsync<Usuario>(sql, login);
    if (user != null) return Results.Ok(user);
    return Results.BadRequest("E-mail ou senha incorretos.");
});

//EVENTOS
app.MapPost("/api/eventos", async (Evento ev) => {
    using var db = new SqlConnection(connectionString); 
    
    var sql = @"INSERT INTO Eventos (Nome, Lugar, CapacidadeTotal, DataEvento, PrecoPadrao, ImagemURL) 
                VALUES (@Nome, @Lugar, @CapacidadeTotal, @DataEvento, @PrecoPadrao, @ImagemURL)";
    
    await db.ExecuteAsync(sql, ev);
    return Results.Ok("Show anunciado com sucesso!");
});

app.MapGet("/api/eventos", async () => {
    using var db = new SqlConnection(connectionString);
    var sql = "SELECT * FROM Eventos ORDER BY DataEvento ASC";
    var eventos = await db.QueryAsync<Evento>(sql);
    return Results.Ok(eventos);
});

app.MapDelete("/api/eventos/{id}", async (int id) => {
    using var db = new SqlConnection(connectionString);
    await db.ExecuteAsync("DELETE FROM Eventos WHERE Id = @id", new { id });
    return Results.Ok("Evento removido!");
});

//CUPONS
app.MapPost("/api/cupons", async (Cupom cp) => {
    using var db = new SqlConnection(connectionString);
    var sql = @"INSERT INTO Cupons (codigo, PorcentagemDesconto, valorMinimoregra) 
                VALUES (@codigo, @PorcentagemDesconto, @valorMinimoregra)";
    await db.ExecuteAsync(sql, cp);
    return Results.Ok("Cupom criado!");
});

app.MapGet("/api/cupons", async () => {
    using var db = new SqlConnection(connectionString);
    return Results.Ok(await db.QueryAsync<Cupom>("SELECT * FROM Cupons"));
});
app.MapGet("/api/cupons/{codigo}", async (string codigo) => {
    using var db = new SqlConnection(connectionString);
    var sql = "SELECT * FROM Cupons WHERE UPPER(codigo) = UPPER(@codigo)";
    var cupom = await db.QueryFirstOrDefaultAsync<Cupom>(sql, new { codigo });
    
    if (cupom != null) return Results.Ok(cupom);
    return Results.NotFound("Cupom não encontrado.");
});

app.MapDelete("/api/cupons/{codigo}", async (string codigo) => {
    using var db = new SqlConnection(connectionString);
    await db.ExecuteAsync("DELETE FROM Cupons WHERE codigo = @codigo", new { codigo });
    return Results.Ok("Cupom removido!");
});

//RESERVAS
app.MapPost("/api/reservas", async (Reserva res) => {
    using var db = new SqlConnection(connectionString);
    var sql = @"INSERT INTO Reservas (UsuarioCpf, EventoId, ValorFinalPago) 
                VALUES (@UsuarioCpf, @EventoId, @ValorFinalPago)";
    await db.ExecuteAsync(sql, res);
    return Results.Ok("Reserva realizada com sucesso!");
});

// ==========================
// RESERVAS
// ==========================

// CADASTRAR RESERVA (POST)
app.MapPost("/api/reservas", async (CriarReservaInput input) =>
{
    using var db = new SqlConnection(connectionString);

    // VALIDAÇÃO 1
    var usuarioExiste = await db.QueryFirstOrDefaultAsync<int>(
        "SELECT 1 FROM Usuarios WHERE Cpf = @UsuarioCpf", new { input.UsuarioCpf });
    if (usuarioExiste == 0) 
        return Results.BadRequest("Validação Falhou: O CPF do usuário informado não está cadastrado.");

    // VALIDAÇÃO 2
    var evento = await db.QueryFirstOrDefaultAsync<Evento>(
        "SELECT Nome, CapacidadeTotal, DataEvento, PrecoPadrao FROM Eventos WHERE Id = @EventoId", 
        new { input.EventoId });
    if (evento == null) 
        return Results.BadRequest("Validação Falhou: O evento informado não foi encontrado.");

    // Preparar valor padrão do ingresso
    decimal valorFinal = evento.PrecoPadrao;

    // VALIDAÇÃO 3
    if (!string.IsNullOrWhiteSpace(input.CupomUtilizado))
    {
        var cupom = await db.QueryFirstOrDefaultAsync<Cupom>(
            "SELECT codigo, PorcentagemDesconto, valorMinimoregra FROM Cupons WHERE codigo = @CupomUtilizado", 
            new { input.CupomUtilizado });
            
        if (cupom == null) 
            return Results.BadRequest("Validação Falhou: O cupom informado é inválido ou não existe.");

        // Regra de negócio: Verificar se o preço do evento atinge a regra do valor mínimo do cupom
        if (evento.PrecoPadrao < cupom.valorMinimoregra)
            return Results.BadRequest($"Validação Falhou: Este cupom exige uma compra mínima de R$ {cupom.valorMinimoregra}.");

        // Aplicar cálculo do desconto em C#
        valorFinal = evento.PrecoPadrao * (1 - (cupom.PorcentagemDesconto / 100));
    }

    // Se passou por todas as validações, insere a reserva usando query parametrizada (@)
    var sqlInsert = @"INSERT INTO Reservas (UsuarioCpf, EventoId, CupomUtilizado, ValorFinalPago) 
                      VALUES (@UsuarioCpf, @EventoId, @CupomUtilizado, @ValorFinalPago)";
                      
    await db.ExecuteAsync(sqlInsert, new { 
        input.UsuarioCpf, 
        input.EventoId, 
        input.CupomUtilizado, 
        ValorFinalPago = valorFinal 
    });

    return Results.Created($"/api/reservas", "Reserva realizada com sucesso!");
});

// BUSCAR RESERVAS POR USUÁRIO (GET)
app.MapGet("/api/reservas/usuario/{cpf}", async (string cpf) =>
{
    using var db = new SqlConnection(connectionString);
    
    var sql = @"
        SELECT 
            r.Id, 
            u.Nome AS NomeUsuario, 
            e.Nome AS NomeEvento, 
            e.DataEvento, 
            r.ValorFinalPago 
        FROM Reservas r
        INNER JOIN Usuarios u ON r.UsuarioCpf = u.Cpf
        INNER JOIN Eventos e ON r.EventoId = e.Id
        WHERE r.UsuarioCpf = @Cpf";

    var relatorioReservas = await db.QueryAsync<ReservaDetalhadaDto>(sql, new { Cpf = cpf });
    return Results.Ok(relatorioReservas);
});

app.Run();

// --- MODELS ---
public record Usuario(string Cpf, string Nome, string Email, string Senha, int NivelAcesso);
public record LoginRequest(string Email, string Senha);
public record Evento(int? Id, string Nome, string Lugar, int CapacidadeTotal, DateTime DataEvento, decimal PrecoPadrao, string ImagemURL);
public record Cupom(string codigo, decimal PorcentagemDesconto, decimal valorMinimoregra);
public record Reserva(string UsuarioCpf, int EventoId, decimal ValorFinalPago);
public record CriarReservaInput(string UsuarioCpf, int EventoId, string? CupomUtilizado);
public record ReservaDetalhadaDto(int Id, string NomeUsuario, string NomeEvento, DateTime DataEvento, decimal ValorFinalPago);
