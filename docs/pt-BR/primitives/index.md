# Inventário de Primitivos

Este inventário descreve a API pública de domínio existente. Ele não documenta ideias de roadmap nem consultas a
cadastros externos.

## Tipos Públicos

| Tipo | Responsabilidade | Valor canônico | Entrada aceita | Modo de validação |
| --- | --- | --- | --- | --- |
| [`Cpf`](cpf.md) | CPF de pessoa física | 11 dígitos | `52998224725`, `529.982.247-25` | DVs módulo 11 |
| [`Cnpj`](cnpj.md) | CNPJ de pessoa jurídica | 14 posições, letras maiúsculas nas 12 primeiras | sem máscara ou `AA.AAA.AAA/AAAA-DD` | módulo 11 ASCII menos 48 |
| [`CpfCnpj`](cpf-cnpj.md) | União para campos que aceitam CPF ou CNPJ | delegado de `Cpf` ou `Cnpj` | qualquer CPF ou CNPJ aceito | delegado |
| [`Cep`](cep.md) | Estrutura de CEP | 8 dígitos | `01311000`, `01311-000` | estrutural |
| [`Email`](email.md) | Subconjunto estrito de email | local preservado, domínio lowercase/Punycode | `local@dominio` | sintático |
| [`MobilePhone`](mobile-phone.md) | Celular brasileiro | DDD + assinante de 9 dígitos | nacional, formatado, `+55`, E.164 | plano de numeração |
| [`LandlinePhone`](landline-phone.md) | Telefone fixo geográfico | DDD + assinante de 8 dígitos | nacional, formatado, `+55`, E.164 | plano de numeração |
| [`TelefoneBrasileiro`](telefone-brasileiro.md) | União de fixo ou celular | valor nacional delegado | qualquer fixo ou celular aceito | delegado |
| [`ChavePix`](chave-pix.md) | Chave Pix | CPF, CNPJ, telefone E.164, email lowercase ou UUID | cinco tipos de chave Pix | delegado mais regras Pix |
| [`Rg`](rg.md) | RG legado estadual | texto específico por UF | canônico e máscaras selecionadas por UF | checksum em SP; demais UFs format-only |
| [`Cnh`](cnh.md) | Número do Registro Nacional da CNH | 11 dígitos | somente sem máscara | dois DVs módulo 11 |
| [`Cns`](cns.md) | Cartão Nacional de Saúde | 15 dígitos | somente sem máscara | algoritmo por família |
| [`TituloEleitoral`](titulo-eleitoral.md) | Título eleitoral | 12 dígitos | somente sem máscara | origem mais dois DVs |
| [`Nit`](nit.md) | Número de Identificação do Trabalhador | 11 dígitos | somente sem máscara | estrutural |
| [`PisPasep`](pis-pasep.md) | Registro PIS/PASEP | 11 dígitos | somente sem máscara | DV módulo 11 |
| [`InscricaoEstadual`](inscricao-estadual.md) | Inscrição estadual de ICMS | dígitos por UF | somente sem máscara | format-only por tamanho estadual |
| [`PlacaVeiculo`](placa-veiculo.md) | Sequência de placa veicular | 7 caracteres maiúsculos | nacional anterior ou Mercosul | padrão estrutural |
| [`Renavam`](renavam.md) | Registro veicular RENAVAM | 11 dígitos | somente sem máscara | DV módulo 11 |
| [`Ispb`](ispb.md) | Identificador do SPB | 8 dígitos | somente sem máscara | estrutural |
| [`CodigoCompe`](codigo-compe.md) | Código COMPE de instituição | 3 dígitos | somente sem máscara | estrutural, rejeita `999` |

Enums públicos relacionados: `BrazilianState`, `TipoCpfCnpj`, `TipoChavePix`, `TipoTelefoneBrasileiro` e
`PadraoPlacaVeiculo`.

## Semântica Compartilhada

Todos os primitivos são value objects imutáveis. A igualdade usa o valor canônico e qualquer contexto explícito levado
pelo tipo. `Parse` lança para entrada inválida, `TryParse` retorna `false` e `IsValid` faz somente validação local.
