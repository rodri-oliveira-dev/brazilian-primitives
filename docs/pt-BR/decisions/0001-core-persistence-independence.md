# ADR 0001: Manter o Core independente de frameworks de persistência

- **Status:** Aceita
- **Data:** 2026-09-02

## Contexto

O repositório começou com os próprios primitives e depois passou a ter integrações separadas para Entity Framework Core e Dapper.

Nesse momento surgiu uma decisão real de arquitetura: o suporte a persistência poderia entrar em `Brazilian.PrimitivesTypes`, ou o pacote principal poderia continuar focado em comportamento de domínio enquanto as integrações ficariam ao redor dele.

A segunda opção corresponde melhor à forma como os pacotes são usados. Uma aplicação que precisa apenas de `Cpf`, `Cnpj`, `Cep` ou `ChavePix` não deveria receber uma dependência de ORM ou SQL client só porque outro consumidor usa uma dessas tecnologias.

## Decisão

`Brazilian.PrimitivesTypes` permanece como o Core independente de persistência.

O suporte a EF Core e Dapper continua em pacotes adapters próprios:

- `Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer`
- `Brazilian.PrimitivesTypes.Dapper.SqlServer`

Os adapters podem referenciar o Core e o framework correspondente. O Core não pode referenciar os adapters, e os adapters não podem se referenciar entre si.

## Alternativas consideradas

Foi considerado colocar o suporte a persistência diretamente no Core e também distribuir as três responsabilidades em um único pacote. As duas opções simplificariam a instalação no papel, mas tornariam dependências de infraestrutura obrigatórias para consumidores que não precisam delas.

## Consequências

- O Core continua pequeno e utilizável sozinho.
- As integrações podem evoluir sem alterar a direção de dependência do Core.
- Um adapter futuro não exige transformar o Core em uma camada de abstração de persistência.
- O limite de dependência é verificado por testes e não depende apenas de convenção.
