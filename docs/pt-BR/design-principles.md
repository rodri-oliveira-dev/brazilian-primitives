# Princípios de Design

Brazilian.PrimitivesTypes privilegia primitivos de domínio previsíveis em vez de limpeza permissiva de strings.

## Contrato de Value Object

- A construção valida a entrada.
- `Value` é a representação canônica usada na igualdade.
- Zeros à esquerda são preservados porque identificadores são armazenados como `string`.
- `Parse` lança `FormatException` para entrada inválida.
- `TryParse` retorna `false` e `default` para entrada inválida.
- `IsValid` delega ao parsing e não consulta serviços externos.
- Instâncias `default` de structs lançam ao ler `Value`.

## Entrada Estrita

A biblioteca aceita apenas formatos documentados. Ela rejeita texto misturado, pontuação solta, espaços nas bordas,
dígitos Unicode semelhantes aos ASCII e máscaras que não estejam explicitamente suportadas pelo primitivo.

Essa escolha evita normalizações silenciosas que podem esconder erro de entrada ou criar um identificador diferente do
valor realmente informado pelo usuário.

## Modos de Validação

Alguns primitivos são apenas estruturais, como `Cep`, `Ispb`, `CodigoCompe`, `Nit` e a matriz atual de
`InscricaoEstadual`. Outros incluem dígitos verificadores locais, como `Cpf`, `Cnpj`, `Cnh`, `Renavam`, `PisPasep`,
`TituloEleitoral` e `Cns`.

Em `Rg`, a validação depende da UF: São Paulo possui checksum implementado; as demais UFs nesta versão são
`format-only`.

## Contexto Faz Parte da Identidade

Quando um identificador brasileiro não tem uma regra nacional única, o contexto é explícito:

- `Rg` inclui a UF emissora por `BrazilianState`;
- `InscricaoEstadual` inclui a UF de cadastro;
- `CpfCnpj`, `ChavePix`, `TelefoneBrasileiro` e placas expõem discriminadores em vez de exigir inferência por string.
