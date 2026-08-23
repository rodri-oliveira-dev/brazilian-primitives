namespace Brazilian.Primitives;

/// <summary>
/// Identifies the Brazilian vehicle plate sequence pattern.
/// </summary>
public enum PadraoPlacaVeiculo
{
    /// <summary>
    /// Previous national pattern: three letters followed by four digits.
    /// </summary>
    NacionalAnterior = 1,

    /// <summary>
    /// Mercosur/PIV pattern: three letters, one digit, one letter, and two digits.
    /// </summary>
    Mercosul,
}
