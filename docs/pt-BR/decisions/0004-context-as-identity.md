# ADR 0004: Tornar contexto obrigatório parte da identidade

- **Status:** Aceita
- **Data:** 2026-09-02

## Contexto

Alguns identificadores brasileiros não possuem uma única regra nacional de validação. Seu significado ou validação depende de contexto explícito, como a unidade federativa emissora. Inferir esse contexto pela string é ambíguo.

## Decisão

Quando o contexto altera o significado ou o contrato de validação de um primitive, esse contexto é representado explicitamente e participa da identidade quando necessário.

Exemplos incluem `Rg` e `InscricaoEstadual` em modo state-aware. Primitives compostos como `CpfCnpj`, `ChavePix` e `TelefoneBrasileiro` expõem discriminadores em vez de exigir que o chamador infira o domínio pela string canônica.

## Alternativas consideradas

- Inferir estado ou subtipo pelo formato do valor.
- Armazenar somente o valor textual e deixar o consumidor controlar o contexto necessário separadamente.

As duas foram rejeitadas quando o contexto é necessário para preservar o significado de domínio.

## Consequências

- O significado de domínio permanece explícito.
- A igualdade pode distinguir valores cujo contexto obrigatório difere.
- Adapters de persistência devem documentar se preservam contexto ou operam em modo somente-valor.
- Inferências não suportadas são evitadas.
