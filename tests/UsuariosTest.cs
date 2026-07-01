using Xunit;
using System;

public class UsuariosTest
{
    [Fact]
    public void CriarUsuario_QuandoDadosForemValidos_DeveDefinirPropriedadesCorretamente()
    {
        // Arrange
        string cpfEsperado = "123.456.789-00";
        string nomeEsperado = "João Silva";
        string emailEsperado = "joao@email.com";

        // Act
        var usuario = new Usuario(cpfEsperado, nomeEsperado, emailEsperado);

        // Assert
        Assert.Equal(cpfEsperado, usuario.Cpf);
        Assert.Equal(nomeEsperado, usuario.Nome);
        Assert.Equal(emailEsperado, usuario.Email);
    }

    [Fact]
    public void CriarCupom_QuandoInstanciado_DeveReterPorcentagemEDescontoCorretos()
    {
        // Arrange
        string codigoEsperado = "PROMO10";
        decimal porcentagemEsperada = 10.0m;
        decimal valorMinimoEsperado = 50.0m;

        // Act
        var cupom = new Cupom(codigoEsperado, porcentagemEsperada, valorMinimoEsperado);

        // Assert
        Assert.Equal(porcentagemEsperada, cupom.PorcentagemDesconto);
        Assert.Equal(valorMinimoEsperado, cupom.valorMinimoregra);
    }

    [Fact]
    public void CriarEvento_QuandoDadosForemValidos_DeveGuardarPrecoPadraoInformado()
    {
        // Arrange
        string nomeEsperado = "Festival de Inverno";
        int capacidadeEsperada = 500;
        DateTime dataEsperada = DateTime.Now.AddDays(30);
        decimal precoEsperado = 120.50m;

        // Act
        var evento = new Evento(nomeEsperado, capacidadeEsperada, dataEsperada, precoEsperado);

        // Assert
        Assert.Equal(precoEsperado, evento.PrecoPadrao);
    }
}
