using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;

internal sealed class AllConventionRecord : IRecordWithId
{
    public int Id
    {
        get;
        set;
    }

    public Cpf Cpf
    {
        get;
        set;
    }

    public Cnpj Cnpj
    {
        get;
        set;
    }

    public CpfCnpj CpfCnpj
    {
        get;
        set;
    }

    public Cep Cep
    {
        get;
        set;
    }

    public Email Email
    {
        get;
        set;
    }

    public Email? OptionalEmail
    {
        get;
        set;
    }

    public MobilePhone MobilePhone
    {
        get;
        set;
    }

    public LandlinePhone LandlinePhone
    {
        get;
        set;
    }

    public TelefoneBrasileiro TelefoneBrasileiro
    {
        get;
        set;
    }

    public ChavePix ChavePix
    {
        get;
        set;
    }

    public Cnh Cnh
    {
        get;
        set;
    }

    public Cns Cns
    {
        get;
        set;
    }

    public TituloEleitoral TituloEleitoral
    {
        get;
        set;
    }

    public Nit Nit
    {
        get;
        set;
    }

    public PisPasep PisPasep
    {
        get;
        set;
    }

    public PlacaVeiculo PlacaVeiculo
    {
        get;
        set;
    }

    public Renavam Renavam
    {
        get;
        set;
    }

    public Ispb Ispb
    {
        get;
        set;
    }

    public CodigoCompe CodigoCompe
    {
        get;
        set;
    }
}
