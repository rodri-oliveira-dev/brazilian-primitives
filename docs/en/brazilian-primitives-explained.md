# Brazilian Primitives Explained

Many Brazilian business systems use national or state identifiers that combine formatting conventions, check digits,
and government context. Brazilian.Primitives models the textual identifier contract, not the external registry behind
it.

## Common Terms

| Term | What it is |
| --- | --- |
| CPF | Individual taxpayer number used broadly as a personal identifier in Brazil. |
| CNPJ | Company taxpayer registration. The current contract supports numeric and alphanumeric CNPJ values. |
| CEP | Brazilian postal code. It is an address code shape, not a live Correios lookup. |
| Pix key | Alias used by Brazil's instant payment system. A key can be CPF, CNPJ, mobile phone, email, or random EVP UUID. |
| DDD | Two-digit geographic area code used with Brazilian phone numbers. |
| RG | Legacy state-issued identity document. It has no single national format. |
| CIN | New national identity card. Its national number is CPF; this library does not model CIN as RG. |
| CNH | Driver license context. `Cnh` models only the National Registration Number. |
| RENAVAM | National vehicle registry code. |
| Placa Mercosul/PIV | Current Brazilian vehicle plate sequence pattern. |
| Inscricao Estadual | State ICMS taxpayer registration. It needs an explicit Brazilian state. |
| CNS | National health card number. |
| Titulo Eleitoral | Voter registration number. |
| NIT and PIS/PASEP | Labor or social-security related identifiers; they are intentionally modeled separately. |
| ISPB and COMPE | Banking and payment-system identifiers maintained in Banco Central contexts. |

## How To Read Validation Results

The library answers questions like:

- does the input match a supported representation?
- what is the canonical value?
- do embedded check digits pass, when this primitive implements that algorithm?
- which explicit type or state context does the value belong to?

It does not answer questions like:

- was this identifier issued?
- is it currently active?
- who owns it?
- does it authorize a transaction, tax operation, benefit, license, or service?

Those questions require official systems or domain-specific integrations outside this package.
