# PlacaVeiculo

`PlacaVeiculo` models only the seven-character Brazilian vehicle plate identifier.

| Concept | Can be inferred from the code? | Responsibility of `PlacaVeiculo` |
|---|---:|---:|
| Previous national vs Mercosur sequence pattern | Yes | Yes |
| Category/use and visual color | No | No |
| Physical dimensions | No | No |
| Vehicle type/group | No | No |
| Front/rear PIV quantity | No | No |
| Existence/assignment | No | No |
| Regularity/QR Code | No | No |

Supported inputs:

- previous national pattern: `ABC1234` or `ABC-1234`, canonicalized to `ABC1234`;
- Mercosur/PIV pattern: `ABC1D23`, canonicalized to uppercase without separators.

ASCII lowercase letters are accepted and normalized to uppercase. Unicode lookalikes, accents, arbitrary punctuation,
spaces, embedded text, and invented Mercosur masks such as `ABC-1D23` are rejected.

`ConverterParaPadraoMercosul` applies only the official algorithmic sequence table `0..9 -> A..J`, for example
`ABC1234 -> ABC1C34`. It does not query SENATRAN/DETRAN and does not prove that the converted sequence was officially
assigned to a vehicle.

Resolution CONTRAN 969/2022 defines visual categories such as private, commercial, official/representation,
diplomatic/consular, special experience/manufacturer, and collection plates, with physical layout and color rules.
Those visual categories are not encoded in the seven-character identifier and are therefore outside this primitive.
Collection-vehicle rules analyzed from CONTRAN 957/2022 do not create a distinct identifier sequence parser here.

References consulted on 2026-08-23:

- CONTRAN Resolucao 969/2022, PIV system: `https://www.gov.br/transportes/pt-br/assuntos/transito/conteudo-contran/resolucoes/resolucao9692022.pdf`
- DETRAN-SP PIV page, conversion table and visual category explanation:
  `https://operacoes.sp.gov.br/DetranWeb/faces/pages/conteudos/empresasEstampadorasDePlacasPIV/saibaMaisSobreNovoModeloDePlacas.xhtml`
- CONTRAN Resolucao 957/2022, collection vehicles.
