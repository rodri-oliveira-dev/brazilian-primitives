# ADR 0003: Preferir parsing estrito a sanitização silenciosa

- **Status:** Aceita
- **Data:** 2026-09-02

## Contexto

É tentador para uma biblioteca de identificadores remover qualquer caractere que não reconhece e validar o que sobrar.

Isso parece conveniente até uma entrada como `"CPF: 529.982.247-25"` ser aceita silenciosamente como se o chamador tivesse fornecido um formato suportado de CPF. Nesse ponto a biblioteca deixa de apenas interpretar um identificador e passa a adivinhar qual trecho de um texto arbitrário o chamador queria usar.

Esse comportamento também dificulta perceber dados ruins chegando de sistemas anteriores.

## Decisão

O parsing é intencionalmente estrito.

Cada primitive aceita apenas os formatos documentados para aquele tipo. Pontuação é normalizada quando a representação mascarada faz parte do contrato, mas a biblioteca não procura valores em texto livre nem tenta corrigir entradas não suportadas.

`Parse`, `TryParse` e `IsValid` seguem as mesmas regras.

## O que não faremos

Não haverá uma etapa genérica de pré-processamento para “remover tudo que não seja dígito/letra”. Aplicações que precisem higienizar texto digitado por usuário podem fazê-lo antes de construir o primitive, deixando essa política visível na própria aplicação.

## Consequências

Dados inválidos tendem a falhar mais perto do ponto onde entraram no sistema, em vez de serem transformados silenciosamente em outro valor.

O custo é intencional: aceitar um novo formato de entrada exige uma mudança explícita na biblioteca e uma decisão de compatibilidade, em vez de passar a funcionar por acidente.
