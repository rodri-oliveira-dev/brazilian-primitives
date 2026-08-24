using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="Cpf"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class CpfValueConverter : ValueConverter<Cpf, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CpfValueConverter"/> class.
    /// </summary>
    public CpfValueConverter()
        : base(value => value.Value, value => Cpf.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}

/// <summary>
/// Converts <see cref="Cnpj"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class CnpjValueConverter : ValueConverter<Cnpj, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CnpjValueConverter"/> class.
    /// </summary>
    public CnpjValueConverter()
        : base(value => value.Value, value => Cnpj.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(14))
    {
    }
}

/// <summary>
/// Converts <see cref="CpfCnpj"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class CpfCnpjValueConverter : ValueConverter<CpfCnpj, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CpfCnpjValueConverter"/> class.
    /// </summary>
    public CpfCnpjValueConverter()
        : base(value => value.Value, value => CpfCnpj.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(14))
    {
    }
}

/// <summary>
/// Converts <see cref="Cep"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class CepValueConverter : ValueConverter<Cep, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CepValueConverter"/> class.
    /// </summary>
    public CepValueConverter()
        : base(value => value.Value, value => Cep.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(8))
    {
    }
}

/// <summary>
/// Converts <see cref="Email"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class EmailValueConverter : ValueConverter<Email, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailValueConverter"/> class.
    /// </summary>
    public EmailValueConverter()
        : base(value => value.Value, value => Email.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(254))
    {
    }
}

/// <summary>
/// Converts <see cref="MobilePhone"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class MobilePhoneValueConverter : ValueConverter<MobilePhone, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MobilePhoneValueConverter"/> class.
    /// </summary>
    public MobilePhoneValueConverter()
        : base(value => value.Value, value => MobilePhone.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}

/// <summary>
/// Converts <see cref="LandlinePhone"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class LandlinePhoneValueConverter : ValueConverter<LandlinePhone, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LandlinePhoneValueConverter"/> class.
    /// </summary>
    public LandlinePhoneValueConverter()
        : base(value => value.Value, value => LandlinePhone.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(10))
    {
    }
}

/// <summary>
/// Converts <see cref="TelefoneBrasileiro"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class TelefoneBrasileiroValueConverter : ValueConverter<TelefoneBrasileiro, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TelefoneBrasileiroValueConverter"/> class.
    /// </summary>
    public TelefoneBrasileiroValueConverter()
        : base(value => value.Value, value => TelefoneBrasileiro.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}

/// <summary>
/// Converts <see cref="ChavePix"/> values to and from their canonical SQL Server string representation.
/// </summary>
/// <remarks>
/// Pix mobile keys are persisted in E.164 form. This lets canonical persistence distinguish a mobile key from a CPF
/// even when the original national phone digits are also a mathematically valid CPF.
/// </remarks>
public sealed class ChavePixValueConverter : ValueConverter<ChavePix, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChavePixValueConverter"/> class.
    /// </summary>
    public ChavePixValueConverter()
        : base(value => value.Value, value => ChavePixCanonicalValueParser.Parse(value), SqlServerValueConverterMappingHints.Ascii(77))
    {
    }
}

/// <summary>
/// Converts <see cref="Cnh"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class CnhValueConverter : ValueConverter<Cnh, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CnhValueConverter"/> class.
    /// </summary>
    public CnhValueConverter()
        : base(value => value.Value, value => Cnh.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}

/// <summary>
/// Converts <see cref="Cns"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class CnsValueConverter : ValueConverter<Cns, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CnsValueConverter"/> class.
    /// </summary>
    public CnsValueConverter()
        : base(value => value.Value, value => Cns.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(15))
    {
    }
}

/// <summary>
/// Converts <see cref="TituloEleitoral"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class TituloEleitoralValueConverter : ValueConverter<TituloEleitoral, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TituloEleitoralValueConverter"/> class.
    /// </summary>
    public TituloEleitoralValueConverter()
        : base(value => value.Value, value => TituloEleitoral.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(12))
    {
    }
}

/// <summary>
/// Converts <see cref="Nit"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class NitValueConverter : ValueConverter<Nit, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NitValueConverter"/> class.
    /// </summary>
    public NitValueConverter()
        : base(value => value.Value, value => Nit.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}

/// <summary>
/// Converts <see cref="PisPasep"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class PisPasepValueConverter : ValueConverter<PisPasep, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PisPasepValueConverter"/> class.
    /// </summary>
    public PisPasepValueConverter()
        : base(value => value.Value, value => PisPasep.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}

/// <summary>
/// Converts <see cref="PlacaVeiculo"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class PlacaVeiculoValueConverter : ValueConverter<PlacaVeiculo, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlacaVeiculoValueConverter"/> class.
    /// </summary>
    public PlacaVeiculoValueConverter()
        : base(value => value.Value, value => PlacaVeiculo.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(7))
    {
    }
}

/// <summary>
/// Converts <see cref="Renavam"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class RenavamValueConverter : ValueConverter<Renavam, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RenavamValueConverter"/> class.
    /// </summary>
    public RenavamValueConverter()
        : base(value => value.Value, value => Renavam.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}

/// <summary>
/// Converts <see cref="Ispb"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class IspbValueConverter : ValueConverter<Ispb, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IspbValueConverter"/> class.
    /// </summary>
    public IspbValueConverter()
        : base(value => value.Value, value => Ispb.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(8))
    {
    }
}

/// <summary>
/// Converts <see cref="CodigoCompe"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class CodigoCompeValueConverter : ValueConverter<CodigoCompe, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodigoCompeValueConverter"/> class.
    /// </summary>
    public CodigoCompeValueConverter()
        : base(value => value.Value, value => CodigoCompe.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(3))
    {
    }
}

internal static class SqlServerValueConverterMappingHints
{
    public static ConverterMappingHints Ascii(int size) => new(size: size, unicode: false);
}

internal static class ChavePixCanonicalValueParser
{
    public static ChavePix Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.StartsWith("+55", StringComparison.Ordinal))
        {
            return ChavePix.From(MobilePhone.Parse(value, CultureInfo.InvariantCulture));
        }

        if (value.Contains('@'))
        {
            return ChavePix.From(Email.Parse(value, CultureInfo.InvariantCulture));
        }

        if (value.Length == 11)
        {
            return ChavePix.From(Cpf.Parse(value, CultureInfo.InvariantCulture));
        }

        if (value.Length == 14)
        {
            return ChavePix.From(Cnpj.Parse(value, CultureInfo.InvariantCulture));
        }

        if (value.Length == 36)
        {
            return ChavePix.FromChaveAleatoria(value);
        }

        throw new FormatException("Persisted Pix key is not in a supported canonical representation.");
    }
}
