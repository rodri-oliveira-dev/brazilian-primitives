namespace Brazilian.Primitives;

/// <summary>
/// Identifies the Pix key type represented by a <see cref="ChavePix"/>.
/// </summary>
public enum TipoChavePix
{
    /// <summary>CPF Pix key.</summary>
    Cpf = 1,

    /// <summary>CNPJ Pix key.</summary>
    Cnpj,

    /// <summary>Brazilian mobile phone Pix key in E.164 format.</summary>
    Celular,

    /// <summary>Email Pix key.</summary>
    Email,

    /// <summary>Random EVP Pix key.</summary>
    Aleatoria,
}
