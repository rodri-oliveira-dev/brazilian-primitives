# ADR 0002: Usar representação canônica em string para identificadores

- **Status:** Aceita
- **Data:** 2026-09-02

## Contexto

Vários valores tratados pela biblioteca parecem números, mas não são números.

CPF, CNPJ, CEP, códigos COMPE e outros identificadores podem conter zeros à esquerda significativos. Tratá-los como valores numéricos tornaria esses zeros fáceis de perder e ainda sugeriria uma semântica aritmética que não existe no domínio.

A biblioteca também aceita alguns formatos mascarados. Se o texto original fornecido pelo chamador fosse mantido internamente, igualdade e persistência passariam a depender da forma de apresentação.

## Decisão

Um primitive válido expõe um único valor canônico em `string`.

O parsing pode aceitar os formatos documentados, mas, depois que a construção é concluída com sucesso, o valor usado para igualdade e persistência é determinístico. Formatação é apresentação; o valor canônico é identidade.

Quando um primitive também exige contexto explícito, esse contexto participa da identidade separadamente da string canônica.

## Alternativa

A principal alternativa seria armazenar a entrada original e normalizar apenas na formatação ou persistência. Isso preservaria exatamente o que o chamador digitou, mas permitiria que identificadores equivalentes carregassem representações internas diferentes.

Armazenamento numérico foi descartado porque perder um zero à esquerda é um erro de correção, não uma diferença de formatação.

## Consequências

- Zeros à esquerda são preservados sem tratamentos especiais.
- `529.982.247-25` e seu equivalente aceito sem máscara resultam no mesmo valor de CPF.
- Os adapters de persistência recebem um único valor estável para armazenar.
- Regras de formatação podem evoluir sem alterar o contrato de identidade.
