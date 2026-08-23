using System.Globalization;
using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class CpfCnpjTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    public void ParseAcceptsCpfRepresentations(string value)
    {
        CpfCnpj cpfCnpj = CpfCnpj.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(TipoCpfCnpj.Cpf, cpfCnpj.Tipo);
        Assert.Equal("52998224725", cpfCnpj.Value);
        Assert.Equal("529.982.247-25", cpfCnpj.Formatted);
        Assert.Equal("52998224725", cpfCnpj.ToString());
        Assert.Equal("52998224725", cpfCnpj.ToString("G", formatProvider: null));
        Assert.Equal("529.982.247-25", cpfCnpj.ToString("F", formatProvider: null));
    }

    [Theory]
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    public void ParseAcceptsNumericCnpjRepresentations(string value)
    {
        CpfCnpj cpfCnpj = CpfCnpj.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(TipoCpfCnpj.Cnpj, cpfCnpj.Tipo);
        Assert.Equal("11222333000181", cpfCnpj.Value);
        Assert.Equal("11.222.333/0001-81", cpfCnpj.Formatted);
    }

    [Theory]
    [InlineData("00000000E08G12")]
    [InlineData("00.000.000/E08G-12")]
    [InlineData("00000000e08g12")]
    public void ParseAcceptsAlphanumericCnpjRepresentations(string value)
    {
        CpfCnpj cpfCnpj = CpfCnpj.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(TipoCpfCnpj.Cnpj, cpfCnpj.Tipo);
        Assert.Equal("00000000E08G12", cpfCnpj.Value);
        Assert.Equal("00.000.000/E08G-12", cpfCnpj.Formatted);
    }

    [Fact]
    public void FromCreatesTypeSafeValues()
    {
        Cpf cpf = Cpf.Parse("52998224725", CultureInfo.InvariantCulture);
        Cnpj cnpj = Cnpj.Parse("11222333000181", CultureInfo.InvariantCulture);

        CpfCnpj fromCpf = CpfCnpj.From(cpf);
        CpfCnpj fromCnpj = CpfCnpj.From(cnpj);

        Assert.Equal(TipoCpfCnpj.Cpf, fromCpf.Tipo);
        Assert.Equal(TipoCpfCnpj.Cnpj, fromCnpj.Tipo);
        Assert.True(fromCpf.TryGetCpf(out Cpf recoveredCpf));
        Assert.True(fromCnpj.TryGetCnpj(out Cnpj recoveredCnpj));
        Assert.Equal(cpf, recoveredCpf);
        Assert.Equal(cnpj, recoveredCnpj);
        Assert.False(fromCpf.TryGetCnpj(out _));
        Assert.False(fromCnpj.TryGetCpf(out _));
    }

    [Theory]
    [InlineData("52998224725", "529.982.247-25")]
    [InlineData("11222333000181", "11.222.333/0001-81")]
    [InlineData("00000000E08G12", "00.000.000/e08g-12")]
    public void EqualityUsesDelegatedCanonicalValue(string left, string right)
    {
        CpfCnpj first = CpfCnpj.Parse(left, CultureInfo.InvariantCulture);
        CpfCnpj second = CpfCnpj.Parse(right, CultureInfo.InvariantCulture);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("52998224724")]
    [InlineData("11222333000180")]
    [InlineData("00000000E08G13")]
    [InlineData("abc")]
    [InlineData(" 52998224725")]
    [InlineData("52998224725 ")]
    public void TryParseReturnsFalseWhenBothParsersRejectInput(string? value)
    {
        bool parsed = CpfCnpj.TryParse(value, out CpfCnpj result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("52998224724")]
    [InlineData("11222333000180")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => CpfCnpj.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => CpfCnpj.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "00.000.000/e08g-12".AsSpan();

        CpfCnpj parsed = CpfCnpj.Parse(value, CultureInfo.InvariantCulture);
        bool success = CpfCnpj.TryParse(value, CultureInfo.InvariantCulture, out CpfCnpj tryParsed);

        Assert.True(success);
        Assert.Equal("00000000E08G12", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void ToStringThrowsFormatExceptionForUnsupportedFormat()
    {
        CpfCnpj cpfCnpj = CpfCnpj.Parse("52998224725", CultureInfo.InvariantCulture);

        Assert.Throws<FormatException>(() => cpfCnpj.ToString("X", formatProvider: null));
    }

    [Fact]
    public void DefaultCpfCnpjValueThrows()
    {
        CpfCnpj cpfCnpj = default;

        Assert.Throws<InvalidOperationException>(() => cpfCnpj.Value);
        Assert.Throws<InvalidOperationException>(() => cpfCnpj.Formatted);
    }
}
