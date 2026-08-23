namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Identifies which Brazilian tax registration domain is represented by a <see cref="CpfCnpj"/>.
/// </summary>
public enum TipoCpfCnpj
{
    /// <summary>
    /// Cadastro de Pessoas Fisicas (CPF).
    /// </summary>
    Cpf = 1,

    /// <summary>
    /// Cadastro Nacional da Pessoa Juridica (CNPJ).
    /// </summary>
    Cnpj,
}
