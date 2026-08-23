# Primitive Inventory

This inventory describes the current public domain API. It intentionally excludes roadmap ideas and external registry
lookups.

## Public Types

| Type | Responsibility | Canonical value | Accepted input | Validation mode |
| --- | --- | --- | --- | --- |
| [`Cpf`](cpf.md) | Individual taxpayer number | 11 digits | `52998224725`, `529.982.247-25` | modulo-11 check digits |
| [`Cnpj`](cnpj.md) | Company taxpayer number | 14 chars, uppercase letters allowed in first 12 positions | unmasked or `AA.AAA.AAA/AAAA-DD` | Receita Federal ASCII-minus-48 modulo-11 |
| [`CpfCnpj`](cpf-cnpj.md) | Union for fields accepting CPF or CNPJ | delegated from `Cpf` or `Cnpj` | any accepted CPF or CNPJ | delegated |
| [`Cep`](cep.md) | Postal-code shape | 8 digits | `01311000`, `01311-000` | structural |
| [`Email`](email.md) | Strict email syntax subset | local part preserved, domain lowercase/Punycode | `local@domain` | syntax only |
| [`MobilePhone`](mobile-phone.md) | Brazilian mobile number | DDD + 9-digit subscriber | national, formatted, `+55`, E.164 | numbering-plan structural |
| [`LandlinePhone`](landline-phone.md) | Brazilian geographic landline | DDD + 8-digit subscriber | national, formatted, `+55`, E.164 | numbering-plan structural |
| [`TelefoneBrasileiro`](telefone-brasileiro.md) | Union of mobile or landline | delegated national value | any accepted mobile or landline | delegated |
| [`ChavePix`](chave-pix.md) | Pix key | CPF, CNPJ, E.164 phone, lowercase email, or UUID | five Pix key kinds | delegated plus Pix rules |
| [`Rg`](rg.md) | Legacy state identity document | state-specific text | canonical and selected masks by state | SP checksum; other states format-only |
| [`Cnh`](cnh.md) | CNH National Registration Number | 11 digits | unmasked only | dual modulo-11 check digits |
| [`Cns`](cns.md) | National health card number | 15 digits | unmasked only | family-specific algorithm |
| [`TituloEleitoral`](titulo-eleitoral.md) | Voter registration number | 12 digits | unmasked only | origin code plus two check digits |
| [`Nit`](nit.md) | Worker identification number | 11 digits | unmasked only | structural |
| [`PisPasep`](pis-pasep.md) | PIS/PASEP registration | 11 digits | unmasked only | modulo-11 check digit |
| [`InscricaoEstadual`](inscricao-estadual.md) | State ICMS taxpayer registration | state-specific digits | unmasked only | format-only by state length |
| [`PlacaVeiculo`](placa-veiculo.md) | Vehicle plate sequence | 7 uppercase chars | previous national or Mercosur pattern | structural pattern |
| [`Renavam`](renavam.md) | Vehicle registry code | 11 digits | unmasked only | modulo-11 check digit |
| [`Ispb`](ispb.md) | Brazilian Payments System participant id | 8 digits | unmasked only | structural |
| [`CodigoCompe`](codigo-compe.md) | COMPE institution code | 3 digits | unmasked only | structural, rejects `999` |

Related public enums are `BrazilianState`, `TipoCpfCnpj`, `TipoChavePix`, `TipoTelefoneBrasileiro`, and
`PadraoPlacaVeiculo`.

## Shared Semantics

All primitives are immutable value objects. Equality uses the canonical value and any explicit context carried by the
type. `Parse` throws for invalid input, `TryParse` returns `false`, and `IsValid` is local validation only.
