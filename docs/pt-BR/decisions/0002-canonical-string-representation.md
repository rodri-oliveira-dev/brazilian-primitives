# ADR 0002: Usar representação canônica em string para identificadores

- **Status:** Aceita
- **Data:** 2026-09-02

## Contexto

Identificadores brasileiros frequentemente possuem zeros à esquerda significativos e são identificadores, não quantidades. Armazenamento numérico pode remover esses zeros e sugere semântica aritmética inexistente no domínio.

## Decisão

Os primitives usam uma representação canônica em `string` como contrato de valor. O parsing pode aceitar formatos explicitamente documentados, mas uma construção válida produz um único valor canônico determinístico. A igualdade usa esse valor normalizado e o contexto explícito quando necessário.

## Alternativas consideradas

- Armazenar identificadores em tipos numéricos.
- Preservar qualquer representação textual fornecida pelo chamador.

O armazenamento numérico foi rejeitado porque pode perder zeros significativos. Preservar texto arbitrário foi rejeitado porque enfraquece igualdade e persistência determinísticas.

## Consequências

- Zeros à esquerda são preservados.
- Igualdade e persistência usam representação estável.
- Formatação permanece separada da identidade.
- Adapters de persistência consomem o valor canônico do domínio em vez de criar representações específicas de provider.
