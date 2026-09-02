# ADR 0001: Manter o Core independente de frameworks de persistência

- **Status:** Aceita
- **Data:** 2026-09-02

## Contexto

Identificadores brasileiros são conceitos de domínio. Consumidores podem usar EF Core, Dapper, outra tecnologia de persistência ou nenhum framework. Colocar dependências de ORM ou provider SQL no Core imporia escolhas de infraestrutura a todos os consumidores.

## Decisão

`Brazilian.PrimitivesTypes` permanece independente de frameworks de persistência. Integrações são pacotes adapters separados. Um adapter pode depender do Core e de seu próprio framework, mas o Core não pode referenciar adapters, EF Core, Dapper ou bibliotecas de SQL client. Os adapters não podem se referenciar.

## Alternativas consideradas

- Colocar suporte a EF Core e Dapper diretamente no Core.
- Distribuir Core e todas as integrações em um único pacote.

As duas alternativas foram rejeitadas porque aumentam dependências transitivas e enfraquecem o limite domínio/infraestrutura.

## Consequências

- O Core pode ser consumido sem dependências de persistência.
- Integrações podem evoluir de forma independente.
- Novos adapters podem ser adicionados sem inverter a direção de dependência do Core.
- O CI deve proteger esse limite contra acoplamento acidental.
