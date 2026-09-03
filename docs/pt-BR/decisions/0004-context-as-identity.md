# ADR 0004: Tornar contexto obrigatório parte da identidade

- **Status:** Aceita
- **Data:** 2026-09-02

## Contexto

Nem todo identificador brasileiro pode ser compreendido apenas pelo texto do próprio identificador.

`Rg` e `InscricaoEstadual` são os exemplos mais claros neste repositório: as regras de validação podem depender da UF emissora. Para RG, São Paulo atualmente possui validação de dígito verificador, enquanto outros estados suportados podem ter apenas validação de formato. Inferir a UF pelo valor transformaria uma heurística de implementação em dado de domínio.

As integrações de persistência expuseram o mesmo problema por outro ângulo. O adapter de EF Core consegue preservar mapeamentos state-aware, enquanto a integração Dapper atualmente documenta `Rg` e `InscricaoEstadual` como cenários value-only.

## Decisão

Quando o contexto é necessário para preservar o significado ou o contrato de validação de um primitive, esse contexto é explícito.

Para identificadores state-aware, a UF não é inferida pela string. Primitives compostos como `CpfCnpj`, `ChavePix` e `TelefoneBrasileiro` também expõem um discriminador, em vez de exigir que o consumidor redescubra o subtipo selecionado a partir do valor canônico.

## Alternativas consideradas

Manter apenas a string e exigir que o chamador controle o contexto separadamente deixaria o tipo mais simples de construir, mas também mais fácil de usar de forma incorreta. Inferir contexto pelo formato do valor foi descartado porque não existe uma regra geral confiável para isso.

## Consequências

- Dois valores visualmente iguais podem continuar distintos quando o contexto de domínio obrigatório for diferente.
- A validação não precisa depender de estado oculto ou heurísticas.
- Adapters de persistência precisam declarar explicitamente se preservam contexto.
- Persistência value-only continua possível onde estiver documentada, mas não é apresentada como equivalente à persistência state-aware.
