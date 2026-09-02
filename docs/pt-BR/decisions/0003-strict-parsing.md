# ADR 0003: Preferir parsing estrito a sanitização silenciosa

- **Status:** Aceita
- **Data:** 2026-09-02

## Contexto

Sanitização permissiva pode transformar texto malformado ou não relacionado em um identificador aparentemente válido. Isso esconde problemas de qualidade de entrada e reduz a confiabilidade de logs e da intenção do chamador.

## Decisão

O parsing aceita somente formatos e regras de normalização documentados. Pontuação é removida apenas quando aquele formato específico faz parte do contrato público. A biblioteca não procura dígitos em texto arbitrário nem corrige silenciosamente entradas não suportadas.

`Parse`, `TryParse` e `IsValid` compartilham a mesma semântica de validação.

## Alternativas consideradas

- Remover todos os caracteres não alfanuméricos antes da validação.
- Tentar corrigir automaticamente valores malformados.

As duas foram rejeitadas porque a conveniência reduziria a previsibilidade.

## Consequências

- Entrada inválida falha cedo e de forma previsível.
- Valores aceitos representam melhor o que o chamador realmente forneceu.
- Novos formatos aceitos exigem decisão explícita de compatibilidade.
- O parsing permanece determinístico e testável.
